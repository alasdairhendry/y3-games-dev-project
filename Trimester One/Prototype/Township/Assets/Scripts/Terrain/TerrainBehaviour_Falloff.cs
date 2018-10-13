using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New FallOff Behaviour", menuName = "Terrain/FallOff Behaviour", order = 4)]
public class TerrainBehaviour_Falloff : TerrainBehaviour {

    [SerializeField] private Texture2D fallOffMap;

    public override void Activate(TerrainBehaviourGroup _group, Terrain _terrain, TerrainData _terrainData)
    {
        base.Activate(_group, _terrain, _terrainData);

        float[,] heightMap = GetHeightData();

        for (int x = 0; x < heightMap.GetLength(0); x++)
        {
            for (int y = 0; y < heightMap.GetLength(1); y++)
            {                
                float fallOffheight = fallOffMap.GetPixel(x, y).grayscale;

                if (x == 0)
                {
                    Debug.Log(fallOffheight);
                }

                float currHeight = heightMap[x, y];

                if(currHeight > fallOffheight)
                {
                    heightMap[x, y] = fallOffheight;
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
