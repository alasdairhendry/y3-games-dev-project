using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour {

    public MeshDuplicator.ChunkData chunk;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDrawGizmosSelected()
    {
        //for (int i = 0; i < chunk.mesh.vertices.Length; i++)
        //{
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawSphere(chunk.mesh.vertices[i], 0.25f);
        //}
    }
}
