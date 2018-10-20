using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    [HideInInspector] public WorldData worldData;
    [HideInInspector] public NoiseData noiseData;
    [HideInInspector] public TextureData textureData;

    private float[,] heightMap;
    
    /// <summary>
    /// Order of events
    /// 
    /// Generate Mesh
    /// Generate Water
    /// Generate Environment: 
    ///                         Trees
    ///                         Rocks
    ///                         Bushes
    /// Generate NavMesh Surface
    /// </summary>
    private void CreateWorld()
    {

    }
}
