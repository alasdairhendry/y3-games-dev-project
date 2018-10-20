using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class WorldData : UpdatableScriptableObject {

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public bool useFlatShading;
    public bool useFalloffMap;

    [Range(0.001f, 10.0f)] public float falloffMapParamA;
    [Range(1.0f, 50.0f)] public float falloffMapParamB;

    public float GetMinHeight()
    {
        return meshHeightMultiplier * meshHeightCurve.Evaluate(0);
    }

    public float GetMaxHeight()
    {
        return meshHeightMultiplier * meshHeightCurve.Evaluate(1);
    }
}
