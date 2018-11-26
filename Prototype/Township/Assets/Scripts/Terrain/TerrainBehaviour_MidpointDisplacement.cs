using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Midpoint Displacement Behaviour", menuName = "Terrain/Midpoint Displacement Behaviour", order = 4)]
public class TerrainBehaviour_MidpointDisplacement : TerrainBehaviour {

    [SerializeField] [Range(-25.0f, -0.1f)] protected float MpDHeightMin = -5f;
    [SerializeField] [Range(0.1f, 25.0f)] protected float MpDHeightMax = -5f;
    [SerializeField] [Range(0.01f, 10.0f)] protected float MpDHeightDampPower = 2.0f;
    [SerializeField] [Range(0.01f, 10.0f)] protected float MpDRoughness = 2.0f;

    public override void Activate(TerrainBehaviourGroup _group, Terrain _terrain, TerrainData _terrainData)
    {
        base.Activate(_group, _terrain, _terrainData);

        float[,] heightMap = GetHeightData();
        int width = heightMap.GetLength(0) - 1;
        int squareSize = width;

        float heightMin = MpDHeightMin;
        float heightMax = MpDHeightMax;
        float heightDamp = (float)Mathf.Pow(MpDHeightDampPower, -1.0f * MpDRoughness);

        int cornerX, cornerY;
        int midX, midY;
        int pMidXL, pMidXR, pMidYU, pMidYD;

        while (squareSize > 0)
        {
            for (int x = 0; x < width; x += squareSize)
            {
                for (int y = 0; y < width; y += squareSize)
                {
                    cornerX = (x + squareSize);
                    cornerY = (y + squareSize);

                    midX = (int)(x + squareSize / 2.0f);
                    midY = (int)(y + squareSize / 2.0f);

                    heightMap[midX, midY] = (float)((heightMap[x, y] + heightMap[cornerX, y] + heightMap[x, cornerY] + heightMap[cornerX, cornerY]) / 4.0f + UnityEngine.Random.Range(heightMin, heightMax));
                }
            }

            for (int x = 0; x < width; x += squareSize)
            {
                for (int y = 0; y < width; y += squareSize)
                {
                    cornerX = (x + squareSize);
                    cornerY = (y + squareSize);

                    midX = (int)(x + squareSize / 2.0f);
                    midY = (int)(y + squareSize / 2.0f);

                    pMidXR = (int)(midX + squareSize);
                    pMidYU = (int)(midY + squareSize);
                    pMidXL = (int)(midX - squareSize);
                    pMidYD = (int)(midY - squareSize);

                    if (pMidXL <= 0 || pMidYD <= 0 || pMidXR >= width - 1 || pMidYU >= width - 1) continue;

                    heightMap[midX, y] = (float)((heightMap[midX, midY] + heightMap[x, y] + heightMap[midX, pMidYD] + heightMap[cornerX, y]) / 4.0f + UnityEngine.Random.Range(heightMin, heightMax));
                    heightMap[midX, cornerY] = (float)((heightMap[x, cornerY] + heightMap[midX, midY] + heightMap[cornerX, cornerY] + heightMap[midX, pMidYU]) / 4.0f + UnityEngine.Random.Range(heightMin, heightMax));
                    heightMap[x, midY] = (float)((heightMap[x, y] + heightMap[pMidXL, midY] + heightMap[x, cornerY] + heightMap[midX, midY]) / 4.0f + UnityEngine.Random.Range(heightMin, heightMax));
                    heightMap[cornerX, midY] = (float)((heightMap[midX, y] + heightMap[midX, midY] + heightMap[cornerX, cornerY] + heightMap[pMidXR, midY]) / 4.0f + UnityEngine.Random.Range(heightMin, heightMax));
                }
            }

            squareSize = (int)(squareSize / 2.0f);
            heightMin *= heightDamp;
            heightMax *= heightDamp;
        }
        terrainData.SetHeights(0, 0, heightMap);

        OnFinished();
    }

    protected override void OnFinished()
    {
        base.OnFinished();
    }
}
