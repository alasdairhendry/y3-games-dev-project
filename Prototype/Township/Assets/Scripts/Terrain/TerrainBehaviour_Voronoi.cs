using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Voronoi Behaviour", menuName = "Terrain/Voronoi Behaviour", order = 4)]
public class TerrainBehaviour_Voronoi : TerrainBehaviour {

    [SerializeField] [Range(0.0f, 1.0f)] private float minHeight = 0.0f;
    [SerializeField] [Range(0.0f, 1.0f)] private float maxHeight = 1.0f;
    [SerializeField] [Range(0.001f, 10.0f)] private float fallOff = 1.0f;
    [SerializeField] [Range(0.001f, 10.0f)] private float dropOff = 1.0f;

    public override void Activate(TerrainBehaviourGroup _group, Terrain _terrain, TerrainData _terrainData)
    {
        base.Activate(_group, _terrain, _terrainData);
        Debug.Log("Boop");
        float[,] heightMap = GetHeightData();

        Vector3 peak = new Vector3(UnityEngine.Random.Range(0, heightMap.GetLength(0)), UnityEngine.Random.Range(minHeight, maxHeight), UnityEngine.Random.Range(0, heightMap.GetLength(1)));

        if (heightMap[(int)peak.x, (int)peak.z] < peak.y)
            heightMap[(int)peak.x, (int)peak.z] = peak.y;
        else { OnFinished(); return; };

        Vector2 peakLocation = new Vector2(peak.x, peak.z);
        float maxDistance = Vector2.Distance(new Vector2(0.0f, 0.0f), new Vector2(heightMap.GetLength(0), heightMap.GetLength(1)));

        for (int y = 0; y < heightMap.GetLength(1); y++)
        {
            for (int x = 0; x < heightMap.GetLength(0); x++)
            {
                if (!(x == peak.x && y == peak.z))
                {
                    float distanceToPeak = Vector2.Distance(peakLocation, new Vector2(x, y)) / maxDistance;                    
                    float fallOffHeight = peak.y - distanceToPeak * fallOff - Mathf.Pow(distanceToPeak, dropOff);

                    if (heightMap[x, y] < fallOffHeight)
                        heightMap[x, y] = fallOffHeight;
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
