using System.Collections;
using UnityEngine;

public static class MeshGenerator {

    public static IEnumerator GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, int levelOfDetail, bool useFlatShading, World sender, float meshTransformScale)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float topLeftX = (width - 1) / -2.0f;
        float topLeftZ = (height - 1) / 2.0f;

        int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
        int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;

        MeshData meshData = new MeshData(verticesPerLine, verticesPerLine, useFlatShading);        
        int vertexIndex = 0;

        int y = 0;      

        while(y<= height)
        {
            for (int x = 0; x < width; x+=meshSimplificationIncrement)
            {
                Vector3 vertexPosition = new Vector3(/*topLeftX + */x, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, /*topLeftZ -*/ y);
                meshData.vertices[vertexIndex] = vertexPosition;                

                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);
                if (x < width - 1 && y < height - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine, vertexIndex + verticesPerLine + 1);
                    meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex + 1, vertexIndex);
                }

                vertexIndex++;
            }            
            y += meshSimplificationIncrement;           
        }
        
        sender.SetMesh(meshData.CreateMesh());
        yield return null;
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

        vertices = flatShadedVertices;
        uvs = flatShadedUvs;
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
                
        return mesh;
    }
}