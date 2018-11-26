using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour {

    public MapObject terrainMesh;
    public MapObject waterMesh;

    public void DrawTerrainMesh(MeshData meshData)
    {
        Mesh mesh = meshData.CreateMesh();

        terrainMesh.meshFilter.sharedMesh = mesh;
        terrainMesh.meshRenderer.sharedMaterial = terrainMesh.meshMaterial;
        terrainMesh.meshCollider.sharedMesh = terrainMesh.meshFilter.sharedMesh;
    }

    public void DrawWaterMesh(MeshData meshData)
    {
        Mesh mesh = meshData.CreateMesh();

        waterMesh.meshFilter.sharedMesh = mesh;
        waterMesh.meshRenderer.sharedMaterial = waterMesh.meshMaterial;
        waterMesh.meshCollider.sharedMesh = waterMesh.meshFilter.sharedMesh;
    }

    [System.Serializable]
    public class MapObject
    {
        public GameObject targetObject;

        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;
        public MeshCollider meshCollider;
        public Material meshMaterial;
    }
}
