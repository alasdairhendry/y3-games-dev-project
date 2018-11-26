using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Seed {

    private static bool created = false;
    private static System.Random prng;

	public static void CreateSeed(int seed)
    {
        prng = new System.Random(seed);
        created = true;
    }

    public static int Next(int min, int max)
    {
        if (!created) { Debug.LogError("Error creating next seed: No Seed Created"); return 0; }
        return prng.Next(min, max);
    }
}
