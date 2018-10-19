using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    public enum DrawMode { NoiseMap, Mesh, FalloffMap }
    public DrawMode drawMode;

    public FilterMode filterMode;

    [Tooltip("Something with good factors, ie 1, 2, 4, 6 then +1")] [SerializeField] private int terrainChunkSize = 97;
    [Tooltip("Something with good factors, ie 1, 2, 4, 6 then +1")] [SerializeField] private int waterChunkSize = 97;
    [Range(0, 6)] public int terrainLevelOfDetail;
    [Range(0, 6)] public int waterLevelOfDetail;

    public TerrainData terrainData;
    public NoiseData noiseData;
    public TextureData textureData;
    public Material terrainMaterial;

    [Range(0.001f, 10.0f)] public float falloffMapParamA;
    [Range(1.0f, 50.0f)] public float falloffMapParamB;

    public bool autoUpdate;
    public bool generateMeshOnAwake;    

    float[,] falloffMap;

    private void Awake()
    {
        falloffMap = FalloffGenerator.GenerateFalloffMap(terrainChunkSize, falloffMapParamA, falloffMapParamB);
        Debug.Log("MapGenerator");
    }

    private void Start()
    {
        if (generateMeshOnAwake)
        {
            drawMode = DrawMode.Mesh;
            GenerateMap();
        }
    }

    private void OnValuesUpdated()
    {
        if (!Application.isPlaying)
        {
            GenerateMap();
        }
    }

    private void OnTextureValuesUpdated()
    {
        textureData.ApplyToMaterial(terrainMaterial);
    }

    public void GenerateMap()
    {
        if (Application.isPlaying)
        {
            GameState.Instance.worldLoadState.AddStage("generate-terrain", false);
            GameState.Instance.worldLoadState.AddStage("generate-water", false);
        }

        float[,] terrainNoiseMap = Noise.GenerateNoise(terrainChunkSize, terrainChunkSize, noiseData.seed, noiseData.noiseScale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, noiseData.offset);
        float[,] waterNoiseMap = new float[waterChunkSize, waterChunkSize];

        if (terrainData.useFalloffMap)
        {
            if (falloffMap == null)
                falloffMap = FalloffGenerator.GenerateFalloffMap(terrainChunkSize, falloffMapParamA, falloffMapParamB);

            for (int y = 0; y < terrainChunkSize; y++)
            {
                for (int x = 0; x < terrainChunkSize; x++)
                {
                    terrainNoiseMap[x, y] = Mathf.Clamp01(terrainNoiseMap[x, y] - falloffMap[x, y]);
                }
            }
        }

        textureData.UpdateMeshHeights(terrainMaterial, terrainData.minHeight, terrainData.maxHeight);
        textureData.ApplyToMaterial(terrainMaterial);
        DisplayMeshes(terrainNoiseMap, waterNoiseMap);
    }

    private void DisplayMeshes(float[,] terrainNoiseMap, float[,] waterNoiseMap)
    {
        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawTerrainMesh(MeshGenerator.GenerateTerrainMesh(terrainNoiseMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, terrainLevelOfDetail, terrainData.useFlatShading));
        display.DrawWaterMesh(MeshGenerator.GenerateTerrainMesh(waterNoiseMap, 1.0f, terrainData.meshHeightCurve, waterLevelOfDetail, terrainData.useFlatShading));
        if (Application.isPlaying)
        {
            GameState.Instance.worldLoadState.UpdateStage("generate-terrain", true);
            GameState.Instance.worldLoadState.UpdateStage("generate-water", true);
            GetComponent<NavMeshGenerator>().GenerateNavMesh();
        }
    }

    private void OnValidate()
    {
        if (terrainData != null)
        {
            terrainData.OnValuesUpdated -= OnValuesUpdated;
            terrainData.OnValuesUpdated += OnValuesUpdated;
        }

        if (noiseData != null)
        {
            noiseData.OnValuesUpdated -= OnValuesUpdated;
            noiseData.OnValuesUpdated += OnValuesUpdated;
        }

        if (textureData != null)
        {
            textureData.OnValuesUpdated -= OnTextureValuesUpdated;
            textureData.OnValuesUpdated += OnTextureValuesUpdated;
        }

        falloffMap = FalloffGenerator.GenerateFalloffMap(terrainChunkSize, falloffMapParamA, falloffMapParamB);
    }
}
