using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapGenerator
{
    private static int terrainChunkSize = 97;
    private static float[,] falloffMap;

    public static IEnumerator GenerateMap(WorldData worldData, NoiseData noiseData, TextureData textureData, Material terrainMaterial, World sender)
    {
        float[,] heightMap = Noise.GenerateNoise(terrainChunkSize, terrainChunkSize, noiseData.noiseScale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, noiseData.offset);

        if (worldData.useFalloffMap)
        {
            falloffMap = FalloffGenerator.GenerateFalloffMap(terrainChunkSize, worldData.falloffMapParamA, worldData.falloffMapParamB);

            for (int y = 0; y < terrainChunkSize; y++)
            {
                for (int x = 0; x < terrainChunkSize; x++)
                {
                    heightMap[x, y] = Mathf.Clamp01(heightMap[x, y] - falloffMap[x, y]);
                }
            }
        }

        textureData.UpdateMeshHeights(terrainMaterial, worldData.GetMinHeight(), worldData.GetMaxHeight());
        textureData.ApplyToMaterial(terrainMaterial);
        sender.SetHeightMap(heightMap);
        yield return null;
    }
}
