using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TerrainMeshGenerator
{

    public static List<ChunkData> GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, int chunkSize)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        int chunkWidth = (width / chunkSize);

        ChunkData[,] chunkData = new ChunkData[chunkWidth, chunkWidth];
        List<ChunkData> chunkList = new List<ChunkData>();

        int vertexIndex = 0;

        for (int y = 0; y < chunkData.GetLength(1); y++)
        {
            for (int x = 0; x < chunkData.GetLength(0); x++)
            {
                ChunkData _c = new ChunkData(chunkSize, height, new Vector2(x, y));
                chunkData[x, y] = _c;
                chunkList.Add(_c);
            }
        }

        for (int c = 0; c < chunkList.Count; c++)
        {
            vertexIndex = 0;

            for (int x = 0; x < chunkSize + 1; x++)
            {
                for (int y = 0; y < chunkSize + 1; y++)
                {
                    chunkList[c].meshData.vertices[vertexIndex] = new Vector3(x, heightMap[y + (int)chunkList[c].worldPosition.z, x + (int)chunkList[c].worldPosition.x] * heightMultiplier, y);
                    float uvX = ((x / (float)chunkSize) / (float)chunkWidth) + ((Mathf.Lerp(0.0f, 1.0f, (float)chunkSize / (float)width) * chunkList[c].coordinate.x));
                    float uvY = ((y / (float)chunkSize) / (float)chunkWidth) + ((Mathf.Lerp(0.0f, 1.0f, (float)chunkSize / (float)width) * chunkList[c].coordinate.y));
                    chunkList[c].meshData.uvs[vertexIndex] = new Vector2(uvX, uvY);

                    if (x == 0 && c == 1)
                        Debug.Log(uvX);

                    if (x < chunkSize && y < chunkSize)
                    {
                        chunkList[c].meshData.AddTriangle(vertexIndex, vertexIndex + (chunkSize + 1) + 1, vertexIndex + (chunkSize + 1));
                        chunkList[c].meshData.AddTriangle(vertexIndex + (chunkSize + 1) + 1, vertexIndex, vertexIndex + 1);
                    }

                    vertexIndex++;
                }
            }
        }

        return chunkList;
    }
}

[System.Serializable]
public class ChunkData
{
    public MeshData meshData;
    public Vector2 coordinate;
    public Vector3 worldPosition;
    public GameObject meshObject;

    public ChunkData(int chunkSize, int height, Vector2 _coordinate)
    {
        meshData = new MeshData(chunkSize, height);
        coordinate = _coordinate;
        worldPosition = new Vector3(coordinate.x * chunkSize, 0.0f, coordinate.y * chunkSize);
    }

    public GameObject CreateChunk()
    {
        meshObject = new GameObject("Chunk " + coordinate);
        meshObject.transform.position = worldPosition;

        meshObject.gameObject.AddComponent<MeshFilter>();
        meshObject.gameObject.AddComponent<MeshRenderer>();

        meshObject.GetComponent<MeshFilter>().mesh = meshData.CreateMesh();
        Debug.Log("Creating Chunk " + worldPosition + " - " + coordinate);
        return meshObject;
    }
}

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    int triangleIndex;

    public MeshData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}