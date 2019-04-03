using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EnvironmentGenerator {

    public static List<EnvironmentEntityData> environmentEntities = new List<EnvironmentEntityData>();

    public static IEnumerator GenerateEnvironment(EnvironmentData data, World sender)
    {
        environmentEntities.Clear ();
        int chance;

        switch (sender.richness)
        {
            case Richness.Sparse:

                chance = Seed.Next(25, 50);

                break;

            case Richness.Abundant:

                chance = Seed.Next(50, 100);

                break;

            case Richness.Plentiful:

                chance = Seed.Next(150, 300);

                break;

            default:

                chance = Seed.Next(25, 50);

                break;
        }

        //for (int i = 0; i < data.detailData.Length; i++)
        //{
        //    BiomeGenerator.Biome b = BiomeGenerator.biomes.Find(x => x.name == data.detailData[i].biomeName);

        //    if (b == null) continue;
        //    if (b.entries == null) continue;

        //    for (int x = 0; x < b.entries.Count; x++)
        //    {
        //        if(Seed.Next(0, 1000) <= chance)
        //        {
        //            Vector3 position = position = new Vector3 ( b.entries[x].x * 10.0f, 150.0f, b.entries[x].z * 10.0f ) - new Vector3 ( 480.0f, 0.0f, 480.0f );

        //            //if (environmentEntities.First ( e => e.position == position ) != null) continue;
        //            if (environmentEntities.Exists ( e => e.position == position )) continue;

        //            environmentEntities.Add(new EnvironmentEntityData()
        //            {
        //                prefabName = data.detailData[i].prefabPath,
        //                position = new Vector3(b.entries[x].x * 10.0f,150.0f, b.entries[x].z * 10.0f) - new Vector3(480.0f, 0.0f, 480.0f)
        //            });
        //        }
        //    }
        //}

        for (int i = 0; i < data.detailGroups.Count; i++)
        {
           
            BiomeGenerator.Biome targetBiome = BiomeGenerator.biomes.Find ( x => x.name == data.detailGroups[i].BiomeName );

            if (targetBiome == null) continue;
            if (targetBiome.entries == null) continue;

            EnvironmentData.DetailGroup group = new EnvironmentData.DetailGroup ();

            // Create basically a drop table with X amount of entries per ratio.
            // Could probably do this a much more efficient way, but, Maths...
            for (int x = 0; x < data.detailGroups[i].group.Count; x++)
            {
                for (int y = 0; y < data.detailGroups[i].group[x].ratio; y++)
                {
                    group.group.Add ( data.detailGroups[i].group[x] );
                }
            }

            for (int x = 0; x < targetBiome.entries.Count; x++)
            {
                if (Seed.Next ( 0, 1000 ) <= (chance * data.detailGroups[i].overallModifier))
                {
                    Vector3 position = position = new Vector3 ( targetBiome.entries[x].x * 10.0f, 150.0f, targetBiome.entries[x].z * 10.0f ) - new Vector3 ( 480.0f, 0.0f, 480.0f );

                    //if (environmentEntities.First ( e => e.position == position ) != null) continue;
                    if (environmentEntities.Exists ( e => e.position == position )) continue;

                    int chosenDataIndex = 0;
                    chosenDataIndex = Seed.Next ( 0, group.group.Count );

                    environmentEntities.Add ( new EnvironmentEntityData ()
                    {
                        prefabName = group.group[chosenDataIndex].data.prefabPath,
                        position = new Vector3 ( targetBiome.entries[x].x * 10.0f, 150.0f, targetBiome.entries[x].z * 10.0f ) - new Vector3 ( 480.0f, 0.0f, 480.0f )
                    } );
                }
            }

            //targetBiome.entries
        }

        yield return CreateGraphics(sender);
    }

    private static IEnumerator CreateGraphics(World sender)
    {
        sender.ClearEnvEntitiyList ();

        int x = 0;

        while(x < environmentEntities.Count)
        {
            EnvironmentEntityData e = environmentEntities[x];
            sender.SpawnEnvironmentEntity(e.prefabName, e.position, Quaternion.identity, null);

            x++;            
        }

        yield return null;
    }

}
