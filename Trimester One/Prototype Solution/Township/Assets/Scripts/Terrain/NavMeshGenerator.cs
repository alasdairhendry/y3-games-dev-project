using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshSurface))]
public class NavMeshGenerator : MonoBehaviour {

    private NavMeshSurface navMeshSurface;

    private void Start()
    {
        navMeshSurface = GetComponent<NavMeshSurface>();
    }

    public void GenerateNavMesh()
    {
        if (Application.isPlaying)
        {
            //GameState.Instance.worldLoadState.AddStage("generate-nav-mesh", false);            
        }

        navMeshSurface.BuildNavMesh();
        navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
    }
}
