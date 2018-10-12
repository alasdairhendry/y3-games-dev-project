using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MeshDuplicator : MonoBehaviour {

    List<Vector3> verts = new List<Vector3>();
    List<int> tris = new List<int>();
    [SerializeField] Material mat;
    [SerializeField] private int maxChunkSize = 128;
    private float chunkSize = 0;
    [SerializeField] private List<Chunk> chunks = new List<Chunk>();

    [ContextMenu("Generate")]
	public void GenerateMesh()
    {        
        Terrain terrain = GetComponent<Terrain>();
        CreateChunks(terrain);
        
        Texture2D heightMap = GetHeightMap(terrain);

        StartCoroutine(GenerateChunks(terrain, heightMap));

        return;

        for (int i = 0; i < chunks.Count; i++)
        {
            //Bottom left section of the map, other sections are similar
            for (int x = (int)chunks[i].chunkPosition.x; x < (int)chunks[i].chunkPosition.x + chunkSize; x++)
            {
                for (int y = (int)chunks[i].chunkPosition.y; y < (int)chunks[i].chunkPosition.y + chunkSize; y++)
                {
                    //Add each new vertex in the plane
                    verts.Add(new Vector3(x, heightMap.GetPixel(x, y).grayscale * 100, y));
                    //Skip if a new square on the plane hasn't been formed
                    if (x == 0 || y == 0) continue;
                    //Adds the index of the three vertices in order to make up each of the two tris
                    tris.Add((int)chunkSize * x + y); //Top right
                    tris.Add((int)chunkSize * x + y - 1); //Bottom right
                    tris.Add((int)chunkSize * (x - 1) + y - 1); //Bottom left - First triangle
                    tris.Add((int)chunkSize * (x - 1) + y - 1); //Bottom left 
                    tris.Add((int)chunkSize * (x - 1) + y); //Top left
                    tris.Add((int)chunkSize * x + y); //Top right - Second triangle
                }
            }

            Vector2[] uvs = new Vector2[verts.Count];
            for (var j = 0; j < uvs.Length; j++) //Give UV coords X,Z world coords
                uvs[j] = new Vector2(verts[j].x, verts[j].z);

            chunks[i].mesh = new Mesh();
            Mesh mesh = chunks[i].mesh;

            GameObject plane = new GameObject("Chunk" + chunks[i].chunkPosition); //Create GO and add necessary components
            plane.transform.SetParent(this.transform.GetChild(0).GetChild(0));
            plane.AddComponent<MeshFilter>();
            plane.AddComponent<MeshRenderer>();

            mesh.vertices = verts.ToArray(); //Assign verts, uvs, and tris to the mesh
            mesh.uv = uvs;
            mesh.triangles = tris.ToArray();
            mesh.RecalculateNormals(); //Determines which way the triangles are facing
            plane.GetComponent<MeshFilter>().mesh = mesh; //Assign Mesh object to MeshFilter    
            plane.GetComponent<MeshRenderer>().material = mat;

            chunks[i].gameObject = plane;
        }
    }

    private void CreateChunks(Terrain terrain)
    {
        TerrainData data = terrain.terrainData;

        if((data.heightmapWidth - 1) % 2 != 0 || (data.heightmapHeight - 1) % 2 != 0 || maxChunkSize % 2 != 0) Debug.LogError("Terrain or Max Chunk Size is not a division of 2");

        if (data.heightmapWidth != data.heightmapHeight) Debug.LogError("Terrain is rectangular");        

        chunkSize = data.heightmapWidth - 1;

        while(chunkSize > maxChunkSize)
        {
            chunkSize = chunkSize / 2.0f;
        }

        Debug.Log("Chunk Size is " + chunkSize + " - - Max Chunk size is " + maxChunkSize);

        for (int x = 0; x < data.heightmapWidth - 1; x+= (int)chunkSize)
        {
            for (int y = 0; y < data.heightmapHeight - 1; y += (int)chunkSize)
            {
                Chunk c = new Chunk();
                c.chunkPosition = new Vector2(x, y);
                chunks.Add(c);
            }
        }
    }

    private IEnumerator GenerateChunks(Terrain terrain, Texture2D heightMap)
    {
        int c = 0;
        int x = 0;
        int y = 0;

        while(c < chunks.Count)
        {
            x = (int)chunks[c].chunkPosition.x;
            y = (int)chunks[c].chunkPosition.y;
            verts.Clear();
            tris.Clear();

            Debug.Log("Generating Chunk " + c);

            while(x < chunks[c].chunkPosition.x + chunkSize)
            {

                while(y < chunks[c].chunkPosition.y + chunkSize)
                {
                    //Add each new vertex in the plane
                    verts.Add(new Vector3(x, heightMap.GetPixel(x, y).grayscale * 100, y));
                    //Skip if a new square on the plane hasn't been formed
                    if (x == 0 || y == 0) continue;
                    //Adds the index of the three vertices in order to make up each of the two tris
                    tris.Add((int)chunkSize * x + y); //Top right
                    tris.Add((int)chunkSize * x + y - 1); //Bottom right
                    tris.Add((int)chunkSize * (x - 1) + y - 1); //Bottom left - First triangle
                    tris.Add((int)chunkSize * (x - 1) + y - 1); //Bottom left 
                    tris.Add((int)chunkSize * (x - 1) + y); //Top left
                    tris.Add((int)chunkSize * x + y); //Top right - Second triangle

                    y += (int)chunkSize;
                    yield return null;
                }

                x += (int)chunkSize;
                yield return null;
            }


            Vector2[] uvs = new Vector2[verts.Count];
            for (var j = 0; j < uvs.Length; j++) //Give UV coords X,Z world coords
                uvs[j] = new Vector2(verts[j].x, verts[j].z);

            chunks[c].mesh = new Mesh();
            Mesh mesh = chunks[c].mesh;

            GameObject plane = new GameObject("Chunk" + chunks[c].chunkPosition); //Create GO and add necessary components
            plane.transform.SetParent(this.transform.GetChild(0).GetChild(0));
            plane.AddComponent<MeshFilter>();
            plane.AddComponent<MeshRenderer>();

            mesh.vertices = verts.ToArray(); //Assign verts, uvs, and tris to the mesh
            mesh.uv = uvs;
            mesh.triangles = tris.ToArray();
            mesh.RecalculateNormals(); //Determines which way the triangles are facing
            plane.GetComponent<MeshFilter>().mesh = mesh; //Assign Mesh object to MeshFilter    
            plane.GetComponent<MeshRenderer>().material = mat;

            chunks[c].gameObject = plane;
            c++;
            yield return null;
        }
    }


    private Texture2D GetHeightMap(Terrain data)
    {
        float[,] map = data.terrainData.GetHeights(0, 0, data.terrainData.heightmapWidth, data.terrainData.heightmapHeight);
        Texture2D tex = new Texture2D(map.GetLength(0), map.GetLength(1));
        Color[] cols = new Color[map.GetLength(0) * map.GetLength(1)];
        List<Color> col = new List<Color>();

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                col.Add(new Color(map[x, y], map[x, y], map[x, y]));
            }
        }

        tex.SetPixels(col.ToArray());
        tex.Apply(false, false);

        byte[] bytes = tex.EncodeToPNG();
        System.IO.Directory.CreateDirectory(Application.dataPath + "/SavedTextures");
        Debug.Log("Tex saved to " + Application.dataPath + "/SavedTextures");
        File.WriteAllBytes(Application.dataPath + "/SavedTextures/" + "ProcTex" + ".png", bytes);

        return tex;
    }

    [System.Serializable]
    public class Chunk
    {
        public Mesh mesh;
        public GameObject gameObject;
        public Vector2 chunkPosition;
    }
}
