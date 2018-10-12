using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Smooth Behaviour", menuName = "Terrain/Smooth Behaviour", order = 4)]
public class TerrainBehaviour_Smooth : TerrainBehaviour {

    public override void Activate(TerrainBehaviourGroup _group, Terrain _terrain, TerrainData _terrainData)
    {
        base.Activate(_group, _terrain, _terrainData);

        float[,] heightMap = GetHeightData();

        for (int y = 0; y < heightMap.GetLength(1); y++)
        {
            for (int x = 0; x < heightMap.GetLength(0); x++)
            {
                float averageHeight = heightMap[x, y];
                List<Vector2> neighbours = GenerateNeighbours(new Vector2(x, y), heightMap.GetLength(0), heightMap.GetLength(1));

                foreach (Vector2 n in neighbours)
                {
                    averageHeight += heightMap[(int)n.x, (int)n.y];
                }

                heightMap[x, y] = averageHeight / ((float)neighbours.Count + 1);
            }
        }

        terrainData.SetHeights(0, 0, heightMap);

        OnFinished();
    }

    private List<Vector2> GenerateNeighbours(Vector2 pos, int width, int height)
    {
        List<Vector2> neighbours = new List<Vector2>();
        for (int y = -1; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {
                if (!(x == 0 && y == 0))
                {
                    Vector2 nPos = new Vector2(Mathf.Clamp(pos.x + x, 0, width - 1),
                                                Mathf.Clamp(pos.y + y, 0, height - 1));
                    if (!neighbours.Contains(nPos))
                        neighbours.Add(nPos);
                }
            }
        }
        return neighbours;
    }

    protected override void OnFinished()
    {
        base.OnFinished();
    }
}
