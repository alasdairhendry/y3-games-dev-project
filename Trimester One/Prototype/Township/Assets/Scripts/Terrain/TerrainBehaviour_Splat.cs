using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Splat Map", menuName = "Terrain/Splat Map", order = 5)]
public class TerrainBehaviour_Splat : TerrainBehaviour {

    [SerializeField] private List<SplatLayer> splatLayers = new List<SplatLayer>();

    [Range(0.0f, 0.25f)] public float splatOffset = 0.005f;
    [Range(0.0001f, 0.50f)] public float splatNoiseXScale = 0.01f;
    [Range(0.0001f, 0.50f)] public float splatNoiseYScale = 0.01f;
    [Range(0.0001f, 0.50f)] public float splatNoiseScaler = 0.005f;

    public override void Activate(TerrainBehaviourGroup _group, Terrain _terrain, TerrainData _terrainData)
    {
        base.Activate(_group, _terrain, _terrainData);

        //SplatPrototype[] newSplatPrototypes;
        //newSplatPrototypes = new SplatPrototype[splatHeights.Count];
        //int spIndex = 0;

        //foreach (SplatHeight sh in splatHeights)
        //{
        //    newSplatPrototypes[spIndex] = new SplatPrototype();
        //    newSplatPrototypes[spIndex].texture = sh.texture;
        //    newSplatPrototypes[spIndex].tileOffset = sh.offset;
        //    newSplatPrototypes[spIndex].tileSize = sh.tiling;
        //    newSplatPrototypes[spIndex].texture.Apply();
        //    spIndex++;
        //}

        //terrainData.splatPrototypes = newSplatPrototypes;

        //float[,] heightMap = GetHeightData();
        //float[,,] splatMapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        //for (int y = 0; y < terrainData.alphamapHeight; y++)
        //{
        //    for (int x = 0; x < terrainData.alphamapWidth; x++)
        //    {
        //        float[] splat = new float[terrainData.alphamapLayers];
        //        for (int i = 0; i < splatHeights.Count; i++)
        //        {
        //            float thisHeightStart = splatHeights[i].minHeight;
        //            float thisHeightStop = splatHeights[i].maxHeight;

        //            if ((heightMap[x, y] >= thisHeightStart && heightMap[x, y] <= thisHeightStop))
        //            {
        //                splat[i] = 1;
        //            }

        //            NormaliseSplatArray(splat);

        //            for (int j = 0; j < splatHeights.Count; j++)
        //            {
        //                splatMapData[x, y, j] = splat[j];
        //            }
        //        }
        //    }
        //}

        SplatPrototype[] splatPrototypes;
        splatPrototypes = new SplatPrototype[splatLayers.Count];
        int index = 0;
        foreach (SplatLayer layer in splatLayers)
        {
            splatPrototypes[index] = new SplatPrototype();
            splatPrototypes[index].texture = layer.texture;
            if (layer.normal != null)
                splatPrototypes[index].normalMap = layer.normal;
            splatPrototypes[index].tileSize = layer.tiling;
            splatPrototypes[index].tileOffset = layer.offset;
            splatPrototypes[index].texture.Apply(true);
            index++;
        }

        terrainData.splatPrototypes = splatPrototypes;

        float[,] heightMap = GetHeightData();
        float[,,] splatMapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                float[] splat = new float[terrainData.alphamapLayers];

                for (int i = 0; i < splatLayers.Count; i++)
                {
                    float noise = Mathf.PerlinNoise(x * splatNoiseXScale, y * splatNoiseYScale) * splatNoiseScaler;
                    float offset = splatOffset + noise;
                    float thisHeightStart = splatLayers[i].minHeight - offset;
                    float thisHeightStop = splatLayers[i].maxHeight + offset;

                    float steepness = terrainData.GetSteepness(y / (float)terrainData.alphamapHeight, x / (float)terrainData.alphamapWidth);

                    if ((heightMap[x, y] >= thisHeightStart && heightMap[x, y] <= thisHeightStop))
                    {
                        if (steepness >= splatLayers[i].minSlope && steepness <= splatLayers[i].maxSlope)
                            splat[i] = 1;
                    }
                }

                NormaliseSplatArray(splat);
                for (int j = 0; j < splatLayers.Count; j++)
                {
                    splatMapData[x, y, j] = splat[j];
                }
            }
        }

        //terrainData.SetAlphamaps(0, 0, splatMapData);

        terrainData.SetAlphamaps(0, 0, splatMapData);
        OnFinished();
    }

    private void NormaliseSplatArray(float[] _splat)
    {
        float total = 0;
        for (int i = 0; i < _splat.Length; i++)
        {
            total += _splat[i];
        }

        for (int i = 0; i < _splat.Length; i++)
        {
            _splat[i] /= total;
        }
    }

    protected override void OnFinished()
    {
        base.OnFinished();
    }

    [System.Serializable]
    public class SplatLayer
    {
        public Texture2D texture = null;
        public Texture2D normal = null;
        public float minHeight = 0.1f;
        public float maxHeight = 0.2f;
        public float minSlope = 0.0f;
        public float maxSlope = 1.5f;
        public Vector2 tiling = new Vector2(64, 64);
        public Vector2 offset = Vector2.zero;
    }
}
