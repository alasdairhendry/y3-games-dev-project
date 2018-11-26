using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Brownian Behaviour", menuName = "Terrain/Brownian Behaviour", order = 3)]
public class TerrainBehaviour_Brownian : TerrainBehaviour {

    public float xScale = 0.01f;
    public float yScale = 0.01f;

    public int octaves = 4;
    public float persistence = 8;
    public float heightScale = 0.09f;

    public int xOffset = 0;
    public int yOffset = 0;

    public override void Activate(TerrainBehaviourGroup _group, Terrain _terrain, TerrainData _terrainData)
    {
        base.Activate(_group, _terrain, _terrainData);

        float[,] heightMap = GetHeightData();

        for (int y = 0; y < heightMap.GetLength(1); y++)
        {
            for (int x = 0; x < heightMap.GetLength(0); x++)
            {
                heightMap[x, y] += Utilities.fBM((x + xOffset) * xScale, (y + yOffset) * yScale, octaves, persistence) * heightScale;               
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
