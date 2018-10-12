using System.Collections;
using UnityEngine;

public class TerrainBehaviour : ScriptableObject {

    protected TerrainBehaviourGroup group;

    protected Terrain terrain;
    protected TerrainData terrainData;

    [SerializeField] [Range(1,10)] protected int iterations = 1;
    public int Iterations { get { return iterations; } }

    protected bool isRunning = false;
    public bool IsRunning { get { return isRunning; } }

    public virtual void Activate(TerrainBehaviourGroup _group, Terrain _terrain, TerrainData _terrainData)
    {
        group = _group;
        terrain = _terrain;
        terrainData = _terrainData;
        isRunning = true;
    }

    protected virtual void OnFinished() { isRunning = false; }

    protected float[,] GetHeightData()
    {
        return terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);
    }

    public void ResetStatus()
    {
        isRunning = false;
    }
}
