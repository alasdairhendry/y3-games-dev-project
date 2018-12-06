using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DEBUG_TerrainMeshShaderUpdater : MonoBehaviour {    
    
    void Update () {
        if (Application.isPlaying) return;
        Do ();
    }

    [ContextMenu ( "Update" )]
    public void Do ()
    {
        FindObjectOfType<World> ().DEBUG_UpdateShaderParams ();
    }
}
