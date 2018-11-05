using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BiomeGenerator {

    public static Dictionary<Vector2, BiomeData.BiomeLayer> biomeLayers = new Dictionary<Vector2, BiomeData.BiomeLayer>();
    public static List<Biome> biomes = new List<Biome>();

    public class Biome
    {
        public string name;
        public List<Vector3> entries = new List<Vector3>();

        public Biome(string name)
        {
            this.name = name;            
        }
    }

    public static IEnumerator CreateBiomes(float[,] heightMap, BiomeData data, World sender)
    {
        biomeLayers.Clear();
        biomes.Clear();
        //Vector3[] v = mesh.vertices;

        //float min = float.MaxValue;
        //float max = float.MinValue;

        //for (int b = 0; b < data.layers.Length; b++)
        //{
        //    for (int i = 0; i < v.Length; i++)
        //    {
        //        v[i].x *= 10.0f;
        //        v[i].z *= 10.0f;

        //        if (CheckContainedWithin(Mathf.InverseLerp(0.0f, 100.0f, v[i].y), data.layers[b].startHeight, data.layers[b].endHeight))
        //        {                                 
        //            sender.biomeLayers.Add(new Vector2(v[i].x, v[i].z), data.layers[b]);
        //        }
        //}
        //}


        //Debug.Log(min + " - " + max);
        for (int y = 0; y < heightMap.GetLength(1); y++)
        {
            for (int x = 0; x < heightMap.GetLength(0); x++)
            {
                for (int b = 0; b < data.layers.Length; b++)
                {
                    if (CheckContainedWithin(heightMap[x, y], data.layers[b].startHeight, data.layers[b].endHeight))
                    {
                        biomeLayers.Add(new Vector2(x, y), data.layers[b]);

                        if(!biomes.Exists((k => k.name == data.layers[b].biomeName)))
                        {
                            Biome biome = new Biome(data.layers[b].biomeName);
                            biome.entries.Add(new Vector3(x, heightMap[x, y], y));
                            biomes.Add(biome);
                        }
                        else
                        {
                            Biome biome = biomes.Find(k => k.name == data.layers[b].biomeName);
                            biome.entries.Add(new Vector3(x, heightMap[x, y], y));
                        }
                    }
                }
            }
        }

        yield return null;
    }

    private static bool CheckContainedWithin(float v, float min, float max)
    {
        if (v >= min && v <= max) return true;
        else return false;
    }

}
