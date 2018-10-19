using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class MeshGenerator {

    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, int levelOfDetail, bool useFlatShading)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float topLeftX = (width - 1) / -2.0f;
        float topLeftZ = (height - 1) / 2.0f;

        int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
        int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;

        MeshData meshData = new MeshData(verticesPerLine, verticesPerLine, useFlatShading);
        int vertexIndex = 0;

        for (int y = 0; y < height; y += meshSimplificationIncrement)
        {
            for (int x = 0; x < width; x += meshSimplificationIncrement)
            {
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, topLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);
                if (x < width - 1 && y < height - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                    meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return meshData;
    }
}

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    int triangleIndex;

    bool useFlatShading;

    public MeshData(int meshWidth, int meshHeight, bool useFlatShading)
    {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
        this.useFlatShading = useFlatShading;
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public void FlatShade()
    {
        Vector3[] flatShadedVertices = new Vector3[triangles.Length];
        Vector2[] flatShadedUvs = new Vector2[triangles.Length];

        for (int i = 0; i < triangles.Length; i++)
        {
            flatShadedVertices[i] = vertices[triangles[i]];
            flatShadedUvs[i] = uvs[triangles[i]];
            triangles[i] = i;
        }


        if (!Application.isPlaying)
            Debug.Log("Flat Shading Started: " + "(Verts: " + vertices.Length + ") - (Tris: " + triangles.Length + ")");

        vertices = flatShadedVertices;
        uvs = flatShadedUvs;

        if (!Application.isPlaying)
            Debug.Log("Flat Shading Complete: " + "(Verts: " + vertices.Length + ") - (Tris: " + triangles.Length + ")");
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();

        if (useFlatShading)
            FlatShade();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();

        //if (!Application.isPlaying)
        //    Debug.Log("Mesh Created: " + "(Verts: " + vertices.Length + ") - (Tris: " + triangles.Length + ")");

        //if (!Application.isPlaying)
        //{
        //    if (useFlatShading)
        //    {
        //        AssetDatabase.CreateAsset(mesh, "Assets/TerrainAssets/TerrainMesh_Flat.asset");
        //        AssetDatabase.SaveAssets();
        //        Debug.Log("Saved TerrainMesh_Flat.asset");
        //    }
        //    else
        //    {
        //        AssetDatabase.CreateAsset(mesh, "Assets/TerrainAssets/TerrainMesh.asset");
        //        AssetDatabase.SaveAssets();
        //        Debug.Log("Saved TerrainMesh.asset");
        //    }
        //}
                
        return mesh;
    }
}