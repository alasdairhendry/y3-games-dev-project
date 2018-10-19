using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TerrainData : UpdatableScriptableObject
{
    //public float uniformScale = 1.0f;
    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public bool useFlatShading;
    public bool useFalloffMap;

    public float minHeight
    {
        get
        {
            return meshHeightMultiplier * meshHeightCurve.Evaluate(0);
        }
    }

    public float maxHeight
    {
        get
        {
            return meshHeightMultiplier * meshHeightCurve.Evaluate(1);
        }
    }
}
