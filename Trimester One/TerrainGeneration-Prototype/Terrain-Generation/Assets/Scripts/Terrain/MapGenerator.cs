using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    //public enum DrawMode { NoiseMap, Mesh, FalloffMap }
    //public DrawMode drawMode;

    public FilterMode filterMode;

    [Tooltip("Something with good factors, ie 1, 2, 4, 6 then +1")] private int terrainChunkSize = 97;
    [Tooltip("Something with good factors, ie 1, 2, 4, 6 then +1")] private int waterChunkSize = 241;
    [Range(0, 6)] public int terrainLevelOfDetail;
    [Range(0, 6)] public int waterLevelOfDetail;

    [HideInInspector] public WorldData worldData;
    [HideInInspector] public NoiseData noiseData;
    [HideInInspector] public TextureData textureData;
    public Material terrainMaterial;

    public bool autoUpdate;
    public bool generateMeshOnAwake;

    float[,] falloffMap;

    private void Awake()
    {
        falloffMap = FalloffGenerator.GenerateFalloffMap(terrainChunkSize, worldData.falloffMapParamA, worldData.falloffMapParamB);
        Debug.Log("MapGenerator");
    }

    private void Start()
    {
        if (generateMeshOnAwake)
        {
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
            //GameState.Instance.worldLoadState.AddStage("generate-terrain", false);
            //GameState.Instance.worldLoadState.AddStage("generate-water", false);
        }

        float[,] terrainNoiseMap = Noise.GenerateNoise(terrainChunkSize, terrainChunkSize, noiseData.seed, noiseData.noiseScale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, noiseData.offset);
        float[,] waterNoiseMap = new float[waterChunkSize, waterChunkSize];

        if (worldData.useFalloffMap)
        {
            falloffMap = FalloffGenerator.GenerateFalloffMap(terrainChunkSize, worldData.falloffMapParamA, worldData.falloffMapParamB);

            for (int y = 0; y < terrainChunkSize; y++)
            {
                for (int x = 0; x < terrainChunkSize; x++)
                {
                    terrainNoiseMap[x, y] = Mathf.Clamp01(terrainNoiseMap[x, y] - falloffMap[x, y]);
                }
            }
        }

        textureData.UpdateMeshHeights(terrainMaterial, worldData.GetMinHeight(), worldData.GetMaxHeight());
        textureData.ApplyToMaterial(terrainMaterial);
        DisplayMeshes(terrainNoiseMap, waterNoiseMap);
    }

    private void DisplayMeshes(float[,] terrainNoiseMap, float[,] waterNoiseMap)
    {
        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawTerrainMesh(MeshGenerator.GenerateTerrainMesh(terrainNoiseMap, worldData.meshHeightMultiplier, worldData.meshHeightCurve, terrainLevelOfDetail, worldData.useFlatShading));
        display.DrawWaterMesh(MeshGenerator.GenerateTerrainMesh(waterNoiseMap, 1.0f, worldData.meshHeightCurve, waterLevelOfDetail, worldData.useFlatShading));
        if (Application.isPlaying)
        {
            //GameState.Instance.worldLoadState.UpdateStage("generate-terrain", true);
            //GameState.Instance.worldLoadState.UpdateStage("generate-water", true);
            GetComponent<NavMeshGenerator>().GenerateNavMesh();
        }
    }

    private void OnValidate()
    {
        if (worldData != null)
        {
            worldData.OnValuesUpdated -= OnValuesUpdated;
            worldData.OnValuesUpdated += OnValuesUpdated;
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

        if (worldData == null)
        {
            Debug.LogError("WTF");
        }
        falloffMap = FalloffGenerator.GenerateFalloffMap(terrainChunkSize, worldData.falloffMapParamA, worldData.falloffMapParamB);
    }
}
