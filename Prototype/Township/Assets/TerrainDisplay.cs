using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainDisplay : MonoBehaviour {

    [SerializeField] Material mat;

	public void DrawMesh(ChunkData chunkData)
    {
        GameObject chunk = chunkData.CreateChunk();        
        chunk.transform.SetParent(this.transform.GetChild(0));
        chunk.GetComponent<MeshRenderer>().material = mat;
    }
}
