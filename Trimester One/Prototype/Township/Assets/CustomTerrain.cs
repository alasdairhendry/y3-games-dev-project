using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CustomTerrain : MonoBehaviour {

    public Vector2 randomHeightRange = new Vector2(0.0f, 0.1f);
    public Texture2D heightMapImage;
    public Vector3 heightMapScale = new Vector3(1.0f, 1.0f, 1.0f);

    public float perlinXScale = 0.01f;
    public float perlinYScale = 0.01f;

    public int perlinOffsetX = 0;
    public int perlinOffsetY = 0;

    public int perlinOctaves = 4;
    public float perlinPersistence = 8;
    public float perlinHeightScale = 0.09f;

    public float fallOff = 0.25f;
    public float dropOff = 1.25f;

    public float splatOffset = 0.005f;
    public float splatNoiseXScale = 0.01f;
    public float splatNoiseYScale = 0.01f;
    public float splatNoiseScaler = 0.005f;

    //EROSION ------------------------------------------------
    public enum ErosionType
    {
        Rain = 0, Thermal = 1, Tidal = 2,
        River = 3, Wind = 4, Canyon = 5
    }
    public ErosionType erosionType = ErosionType.Rain;
    public float erosionStrength = 0.1f;
    public float erosionAmount = 0.01f;
    public int springsPerRiver = 5;
    public float solubility = 0.01f;
    public int droplets = 10;
    public int erosionSmoothAmount = 5;

    [System.Serializable]
    public class PerlinParameters
    {
        public float xScale = 0.01f;
        public float yScale = 0.01f;

        public int octaves = 4;
        public float persistence = 8;
        public float heightScale = 0.09f;

        public int xOffset = 0;
        public int yOffset = 0;

        public bool remove = false;
    }

    public List<PerlinParameters> perlinParameters = new List<PerlinParameters>() { new PerlinParameters() };

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
        public bool remove = false;
    }

    public List<SplatLayer> splatLayers = new List<SplatLayer>() { new SplatLayer() };

    public Terrain terrain;
    public TerrainData terrainData;

    private void OnEnable()
    {
        Debug.Log("Initialising Terrain Data");
        terrain = this.GetComponent<Terrain>();
        terrainData = terrain.terrainData;        
    }

    private void Reset()
    {
        Debug.Log("Initialising Terrain Data");
        terrain = this.GetComponent<Terrain>();
        terrainData = terrain.terrainData;
    }

    private void Awake()
    {
        SerializedObject tagManger = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManger.FindProperty("tags");

        AddTag(tagsProp, "Terrain");
        AddTag(tagsProp, "Cloud");
        AddTag(tagsProp, "Shore");

        tagManger.ApplyModifiedProperties();
        this.gameObject.tag = "Terrain";
    }

    private void AddTag(SerializedProperty tagsProp, string newTag)
    {
        bool found = false;

        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(newTag)) { found = true; break; }
        }

        if (!found)
        {
            tagsProp.InsertArrayElementAtIndex(0);
            SerializedProperty newTagProp = tagsProp.GetArrayElementAtIndex(0);
            newTagProp.stringValue = newTag;
        }
    }

    public void RandomiseTerrain(bool blend)
    {
        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);

        for (int x = 0; x < heightMap.GetLength(1); x++)
        {
            for (int y = 0; y < heightMap.GetLength(0); y++)
            {
                if (blend)
                    heightMap[x, y] += UnityEngine.Random.Range(randomHeightRange.x, randomHeightRange.y);
                else
                    heightMap[x, y] = UnityEngine.Random.Range(randomHeightRange.x, randomHeightRange.y);
            }
        }

        terrainData.SetHeights(0, 0, heightMap);
    }

    public void LoadTexture(bool blend)
    {
        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);

        for (int x = 0; x < heightMap.GetLength(0); x++)
        {
            for (int z = 0; z < heightMap.GetLength(1); z++)
            {
                if (blend)
                    heightMap[z, x] += heightMapImage.GetPixel((int)(x * heightMapScale.x), (int)(z * heightMapScale.z)).grayscale * heightMapScale.y;
                else
                    heightMap[z, x] = heightMapImage.GetPixel((int)(x * heightMapScale.x), (int)(z * heightMapScale.z)).grayscale * heightMapScale.y;
            }
        }

        terrainData.SetHeights(0, 0, heightMap);
    }

    public void GeneratePerlinTerrain(bool blend)
    {
        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);

        for (int y = 0; y < heightMap.GetLength(1); y++)
        {
            for (int x = 0; x < heightMap.GetLength(0); x++)
            {
                if (blend)
                    heightMap[x, y] += Mathf.PerlinNoise((x + perlinOffsetX) * perlinXScale, (y + perlinOffsetY) * perlinYScale);
                else
                    heightMap[x, y] = Mathf.PerlinNoise((x + perlinOffsetX) * perlinXScale, (y + perlinOffsetY) * perlinYScale);
            }
        }

        terrainData.SetHeights(0, 0, heightMap);
    }

    public void GenerateFractalBrownian(bool blend)
    {
        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);

        for (int y = 0; y < heightMap.GetLength(1); y++)
        {
            for (int x = 0; x < heightMap.GetLength(0); x++)
            {
                if (blend)
                    heightMap[x, y] += Utilities.fBM((x + perlinOffsetX) * perlinXScale, (y + perlinOffsetY) * perlinYScale, perlinOctaves, perlinPersistence) * perlinHeightScale;
                else
                    heightMap[x, y] = Utilities.fBM((x + perlinOffsetX) * perlinXScale, (y + perlinOffsetY) * perlinYScale, perlinOctaves, perlinPersistence) * perlinHeightScale;
            }
        }

        terrainData.SetHeights(0, 0, heightMap);
    }

    public void GenerateLayeredFractalBrownian(bool blend)
    {
        float[,] heightMap;

        if (blend)
            heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);
        else
            heightMap = new float[terrainData.heightmapWidth, terrainData.heightmapHeight];

        for (int y = 0; y < heightMap.GetLength(1); y++)
        {
            for (int x = 0; x < heightMap.GetLength(0); x++)
            {
                foreach (PerlinParameters p in perlinParameters)
                {
                    heightMap[x, y] += Utilities.fBM((x + p.xOffset) * p.xScale, (y + p.yOffset) * p.yScale, p.octaves, p.persistence) * p.heightScale;
                }
            }
        }

        terrainData.SetHeights(0, 0, heightMap);
    }

    public void Voronoi(bool blend)
    {
        float[,] heightMap = GetHeightData(blend);

        Vector3 peak = new Vector3(UnityEngine.Random.Range(0, heightMap.GetLength(0)), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0, heightMap.GetLength(1)));
        //Vector3 peak = new Vector3(heightMap.GetLength(0) / 2, 1.0f, heightMap.GetLength(1) / 2);

        if (heightMap[(int)peak.x, (int)peak.z] < peak.y)
            heightMap[(int)peak.x, (int)peak.z] = peak.y;
        else return;

        Vector2 peakLocation = new Vector2(peak.x, peak.z);
        float maxDistance = Vector2.Distance(new Vector2(0.0f, 0.0f), new Vector2(heightMap.GetLength(0), heightMap.GetLength(1)));

        for (int y = 0; y < heightMap.GetLength(1); y++)
        {
            for (int x = 0; x < heightMap.GetLength(0); x++)
            {
                if(!(x== peak.x && y == peak.z))
                {
                    float distanceToPeak = Vector2.Distance(peakLocation, new Vector2(x, y)) / maxDistance;
                    //float fallOffHeight = peak.y - Mathf.Pow(distanceToPeak, 0.8f);
                    float fallOffHeight = peak.y - distanceToPeak * fallOff - Mathf.Pow(distanceToPeak, dropOff);

                    if (heightMap[x, y] < fallOffHeight)
                        heightMap[x, y] = fallOffHeight;
                }
            }
        }

        terrainData.SetHeights(0, 0, heightMap);
    }

    public float MpDHeightMin = -5f;
    public float MpDHeightMax = -5f;
    public float MpDHeightDampPower = 2.0f;
    public float MpDRoughness = 2.0f;

    public void MidpointDisplacement(bool blend)
    {
        float[,] heightMap = GetHeightData(blend);
        int width = heightMap.GetLength(0) - 1;
        int squareSize = width;

        float heightMin = MpDHeightMin;
        float heightMax = MpDHeightMax;
        float heightDamp = (float)Mathf.Pow(MpDHeightDampPower, -1.0f * MpDRoughness);

        //float height = (float)squareSize / 2.0f * 0.01f;
        ////float roughness = 2.0f;
        //float heightDamp = (float)Mathf.Pow(MpDHeightDampPower, -1 * MpDRoughness);

        int cornerX, cornerY;
        int midX, midY;
        int pMidXL, pMidXR, pMidYU, pMidYD;

        //float mixRange = 0.15f;
        //float maxRange = 0.35f;

        //heightMap[0, 0] = DebugRandomRange("First: ", mixRange, maxRange);
        //heightMap[0, heightMap.GetLength(1) - 2] = DebugRandomRange("Second: ", mixRange, maxRange);
        //heightMap[heightMap.GetLength(0) - 2, 0] = DebugRandomRange("Third: ", mixRange, maxRange);
        //heightMap[heightMap.GetLength(0) - 2, heightMap.GetLength(1) - 2] = DebugRandomRange("Fourth: ", mixRange, maxRange);

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
    }

    public void Erode()
    {
        if (erosionType == ErosionType.Rain)
            Rain();
        else if (erosionType == ErosionType.Tidal)
            Tidal();
        else if (erosionType == ErosionType.Thermal)
            Thermal();
        else if (erosionType == ErosionType.River)
            River();
        else if (erosionType == ErosionType.Wind)
            Wind();
        else if (erosionType == ErosionType.Canyon)
            DigCanyon();

        for (int i = 0; i < erosionSmoothAmount; i++)
        {
            Smooth();
        }
    }

    public void Rain()
    {
        float[,] heightMap = GetHeightData(true);

        for (int i = 0; i < droplets; i++)
        {
            heightMap[UnityEngine.Random.Range(0, heightMap.GetLength(0)), UnityEngine.Random.Range(0, heightMap.GetLength(1))] -= erosionStrength;
        }

        terrainData.SetHeights(0, 0, heightMap);
    }

    public void Tidal()
    {

    }

    public void Thermal()
    {

    }

    public void River()
    {
        float[,] heightMap = GetHeightData(true);
        float[,] erosionMap = GetHeightData(false);

        for (int i = 0; i < droplets; i++)
        {
            Vector2 dropletPosition = new Vector2(UnityEngine.Random.Range(0, terrainData.heightmapWidth),
                                                  UnityEngine.Random.Range(0, terrainData.heightmapHeight));
            erosionMap[(int)dropletPosition.x, (int)dropletPosition.y] = erosionStrength;
            for (int j = 0; j < springsPerRiver; j++)
            {
                erosionMap = RunRiver(dropletPosition, heightMap, erosionMap,
                                   terrainData.heightmapWidth,
                                   terrainData.heightmapHeight);
            }
        }

        for (int y = 0; y < terrainData.heightmapHeight; y++)
        {
            for (int x = 0; x < terrainData.heightmapWidth; x++)
            {
                if (erosionMap[x, y] > 0)
                {
                    heightMap[x, y] -= erosionMap[x, y];
                }
            }
        }

        terrainData.SetHeights(0, 0, heightMap);
    }

    float[,] RunRiver(Vector2 dropletPosition, float[,] heightMap, float[,] erosionMap, int width, int height)
    {
        while (erosionMap[(int)dropletPosition.x, (int)dropletPosition.y] > 0)
        {
            List<Vector2> neighbours = GenerateNeighbours(dropletPosition, width, height);
            neighbours.Shuffle();
            bool foundLower = false;
            foreach (Vector2 n in neighbours)
            {
                if (heightMap[(int)n.x, (int)n.y] < heightMap[(int)dropletPosition.x, (int)dropletPosition.y])
                {
                    erosionMap[(int)n.x, (int)n.y] = erosionMap[(int)dropletPosition.x,
                                                                (int)dropletPosition.y] - solubility;
                    dropletPosition = n;
                    foundLower = true;
                    break;
                }
            }
            if (!foundLower)
            {
                erosionMap[(int)dropletPosition.x, (int)dropletPosition.y] -= solubility;
            }
        }
        return erosionMap;
    }

    public void Wind()
    {

    }

    public void DigCanyon()
    {

    }

    public void Smooth()
    {
        float[,] heightMap = GetHeightData(true);

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

    public void AddFractalBrownianLayer()
    {
        perlinParameters.Add(new PerlinParameters());
    }

    public void RemoveFractalBrownianLayer()
    {
        List<PerlinParameters> keptPerlinParameters = new List<PerlinParameters>();

        for (int i = 0; i < perlinParameters.Count; i++)
        {
            if (!perlinParameters[i].remove)
            {
                keptPerlinParameters.Add(perlinParameters[i]);
            }
        }

        if(keptPerlinParameters.Count == 0)
        {
            keptPerlinParameters.Add(perlinParameters[0]);
        }

        perlinParameters = keptPerlinParameters;
    }

    public void AddSplatLayer()
    {
        splatLayers.Add(new SplatLayer());
    }

    public void RemoveSplatLayer()
    {
        List<SplatLayer> _splatLayers = new List<SplatLayer>();
        for (int i = 0; i < splatLayers.Count; i++)
        {
            if (!splatLayers[i].remove)
                _splatLayers.Add(splatLayers[i]);
        }

        if (_splatLayers.Count == 0)
            _splatLayers.Add(splatLayers[0]);

        splatLayers = _splatLayers;
    }

    public void ApplySplatmap()
    {
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

        float[,] heightMap = GetHeightData(true);
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

                    //float steepness = GetSteepness(heightMap, x, y, terrainData.heightmapWidth, terrainData.heightmapHeight);
                    float steepness = terrainData.GetSteepness(y / (float)terrainData.alphamapHeight, x / (float)terrainData.alphamapWidth);

                    if((heightMap[x,y] >= thisHeightStart && heightMap[x,y] <= thisHeightStop))
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

        terrainData.SetAlphamaps(0, 0, splatMapData);        
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

    private float GetSteepness(float[,] heightmap, int x, int y, int width, int height)
    {
        float h = heightmap[x, y];
        int nx = x + 1;
        int ny = y + 1;

        //if on the upper edge of the map find gradient by going backward.
        if (nx > width - 1) nx = x - 1;
        if (ny > height - 1) ny = y - 1;

        float dx = heightmap[nx, y] - h;
        float dy = heightmap[x, ny] - h;
        Vector2 gradient = new Vector2(dx, dy);

        float steep = gradient.magnitude;

        return steep;
    }

    public void ResetTerrain()
    {
        float[,] heightMap = new float[terrainData.heightmapWidth, terrainData.heightmapHeight];
        terrainData.SetHeights(0, 0, heightMap);
    }

    private float[,] GetHeightData(bool blend)
    {
        if (blend)
        {
            return terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);
        }
        else
        {
            return new float[terrainData.heightmapWidth, terrainData.heightmapHeight];            
        }
    }

    private float DebugRandomRange(string prefix, float min, float max)
    {
        float f = UnityEngine.Random.Range(min, max);
        Debug.Log(prefix + f + " (" + min + ", " + max + ")");
        return f;
    }
}
