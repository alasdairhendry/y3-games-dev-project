using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[ExecuteInEditMode]
public class TerrainController : MonoBehaviour {

    [SerializeField] private bool autoGenerateTerrain = false;
    [SerializeField] private bool autoGenerateMesh = false;
    [SerializeField] private bool autoGenerateNavMesh = false;

    const int chunkSize = 128;
    [SerializeField] [Range(0, 10)] private int levelOfDetail;

    [SerializeField] List<ChunkData> chunkData = new List<ChunkData>();

    private void Awake()
    {
        GetComponent<TerrainGenerator>().onBeginGenerate += OnTerrainBeginGenerate;
        GetComponent<TerrainGenerator>().onEndGenerate += OnTerrainEndGenerate;
    }

    public void GenerateTerrain()
    {
        GetComponent<TerrainGenerator>().Generate();
    }

    public void GenerateMesh()
    {
        Terrain terrain = GetComponent<Terrain>();
        terrain.drawHeightmap = false;
        chunkData = TerrainMeshGenerator.GenerateTerrainMesh(terrain.terrainData.GetHeights(0, 0, terrain.terrainData.heightmapWidth, terrain.terrainData.heightmapHeight), terrain.terrainData.size.y, chunkSize);

        for (int i = 0; i < chunkData.Count; i++)
        {
            GetComponent<TerrainDisplay>().DrawMesh(chunkData[i]);
        }

        if (autoGenerateNavMesh) GenerateNavMesh();
    }

    public void GenerateNavMesh()
    {
        for (int i = 0; i < chunkData.Count; i++)
        {
            chunkData[i].meshObject.layer = 9;
        }

        GetComponent<NavMeshSurface>().BuildNavMesh();
    }

    private void OnTerrainBeginGenerate()
    {

    }

    private void OnTerrainEndGenerate()
    {
        if (autoGenerateMesh)
            GenerateMesh();
    }
}
