﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TerrainGenerator : MonoBehaviour {

    [SerializeField] private TerrainBehaviourGroup behaviourGroup;
    public TerrainBehaviourGroup BehaviourGroup { get { return behaviourGroup; } }

    private Terrain terrain;
    private TerrainData terrainData;

    private bool isGenerating = false;

    public Action onBeginGenerate;
    public Action onEndGenerate;

    [SerializeField] private int seed;

    [ContextMenu("Generate")]
    public void Generate()
    {
        UnityEngine.Random.InitState(seed);
        if(behaviourGroup == null)
        {
            Debug.LogError("No behaviour group assigned");
            return;
        }
        
        CheckTerrain();       
        ApplyTerrainValues();        
        StartCoroutine(IGenerate());        
    }

    private void CheckTerrain()
    {
        if (!GetComponent<Terrain>())
        {
            terrain = this.gameObject.AddComponent<Terrain>();
        }
        if (!GetComponent<TerrainCollider>())
        {
            this.gameObject.AddComponent<TerrainCollider>();
        }

        terrain = GetComponent<Terrain>();

        if (terrain.terrainData == null)
            terrain.terrainData = new TerrainData();

        terrainData = terrain.terrainData;

        if (terrainData == null) Debug.LogError("ERROR");

        this.GetComponent<TerrainCollider>().terrainData = terrain.terrainData;

        //if (!GetComponent<Terrain>())
        //{
        //    terrain = this.gameObject.AddComponent<Terrain>();
        //    terrain.terrainData = new TerrainData();
        //    terrainData = terrain.terrainData;
        //}
        //else
        //{
        //    terrain = GetComponent<Terrain>();

        //    if (terrain.terrainData == null)
        //    {
        //        terrain.terrainData = new TerrainData();
        //        terrainData = terrain.terrainData;
        //    }
        //}

        //if (!GetComponent<TerrainCollider>())
        //    this.gameObject.AddComponent<TerrainCollider>();
    }

    private void ApplyTerrainValues()
    {        
        terrainData.heightmapResolution = behaviourGroup.TerrainDimensions;
        terrainData.baseMapResolution = terrainData.heightmapResolution;
        terrainData.baseMapResolution = behaviourGroup.TerrainDimensions;
        terrainData.SetDetailResolution(behaviourGroup.DetailResolution, 16);
        terrainData.size = new Vector3(behaviourGroup.TerrainDimensions, behaviourGroup.TerrainHeight, behaviourGroup.TerrainDimensions);
    }

    private IEnumerator IGenerate()
    {
        yield return ResetTerrain();

        Debug.Log("Beginning Terrain Generation");
        isGenerating = true;

        if (onBeginGenerate != null)
            onBeginGenerate();

        Queue<TerrainBehaviourLayer> behaviours = new Queue<TerrainBehaviourLayer>(behaviourGroup.TerrainBehaviours);

        foreach (TerrainBehaviourLayer tb in behaviours)
        {
            tb.terrainBehaviour.ResetStatus();
        }

        while(behaviours.Count > 0)
        {
            TerrainBehaviourLayer activeBehaviour = behaviours.Dequeue();
            Debug.Log("Obtained Active Behaviour: " + activeBehaviour.terrainBehaviour.name);
            int iteration = 0;

            while (iteration < activeBehaviour.terrainBehaviour.Iterations)
            {                
                while (activeBehaviour.terrainBehaviour.IsRunning)
                {                    
                    yield return null;
                }                

                Debug.Log("Generating Behaviour: " + activeBehaviour.terrainBehaviour.name + "  -  Iteration " + iteration + " of " + activeBehaviour.terrainBehaviour.Iterations);

                if (activeBehaviour.enabled)
                    activeBehaviour.terrainBehaviour.Activate(behaviourGroup, terrain, terrainData);

                iteration++;

                yield return null;
            }

            Debug.Log("Behaviour Generated Successfully: " + activeBehaviour.terrainBehaviour.name);

            yield return null;
        }

        isGenerating = false;

        if (onEndGenerate != null)
            onEndGenerate();

        Debug.Log("Terrain Generation Complete");
    }

    private IEnumerator ResetTerrain()
    {
        Debug.Log("Resetting Terrain Maps");

        if(terrainData == null)
        {
            Debug.LogError("Terrain data does not exist");
            yield break;
        }

        float[,] heightMap = new float[terrainData.heightmapWidth, terrainData.heightmapHeight];
        terrainData.SetHeights(0, 0, heightMap);

        yield return null;
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Generate"))
        {
            Generate();
        }
    }
}
