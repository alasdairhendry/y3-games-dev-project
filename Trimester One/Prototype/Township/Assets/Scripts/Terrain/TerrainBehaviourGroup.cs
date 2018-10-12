using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Terrain Behaviour Group", menuName = "Terrain/Behaviour Group", order = 1)]
public class TerrainBehaviourGroup : ScriptableObject {

    [SerializeField] private int terrainDimensions = 1024;
    [SerializeField] private int terrainHeight = 100;
    [SerializeField] private int detailResolution = 100;

    [SerializeField] private List<TerrainBehaviour> terrainBehaviours = new List<TerrainBehaviour>();
    [SerializeField] private TerrainBehaviour_Splat terrainSplat;

    public int TerrainDimensions { get { return terrainDimensions; } }
    public int TerrainHeight { get { return terrainHeight; } }
    public int DetailResolution { get { return detailResolution; } }

    public List<TerrainBehaviour> TerrainBehaviours { get { return terrainBehaviours; } }
    public TerrainBehaviour_Splat TerrainSplat { get { return terrainSplat; } }
}
