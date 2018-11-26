using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Brownian Subgroup", menuName = "Terrain/Brownian Sub Group", order = 2)]
public class TerrainBehaviour_Subgroup_Brownian : TerrainBehaviourSubGroup {

    [SerializeField] private List<TerrainBehaviour_Brownian> behaviours = new List<TerrainBehaviour_Brownian>();

    public override void Activate(TerrainBehaviourGroup _group, Terrain _terrain, TerrainData _terrainData)
    {
        base.Activate(_group, _terrain, _terrainData);

        float[,] heightMap = GetHeightData();

        for (int y = 0; y < heightMap.GetLength(1); y++)
        {
            for (int x = 0; x < heightMap.GetLength(0); x++)
            {
                foreach (TerrainBehaviour_Brownian b in behaviours)
                {
                    heightMap[x, y] += Utilities.fBM((x + b.xOffset) * b.xScale, (y + b.yOffset) * b.yScale, b.octaves, b.persistence) * b.heightScale;
                }
            }
        }

        terrainData.SetHeights(0, 0, heightMap);
        OnFinished();
    }

    protected override void OnFinished()
    {
        base.OnFinished();
    }
}
