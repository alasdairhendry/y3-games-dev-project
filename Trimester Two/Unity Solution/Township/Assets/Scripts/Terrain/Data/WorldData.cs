using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class WorldData : UpdatableScriptableObject {

    public float heightMultiplier;
    public AnimationCurve heightCruve;

    public bool useFlatShading;
    public bool useFalloffMap;

    public float temperatureMin = -4.0f;
    public float temperatureMax = 30.0f;

    [Range(0.001f, 10.0f)] public float falloffMapParamA;
    [Range(1.0f, 50.0f)] public float falloffMapParamB;

    public float GetMinHeight()
    {
        return heightMultiplier * heightCruve.Evaluate(0);
    }

    public float GetMaxHeight()
    {        
        return heightMultiplier * heightCruve.Evaluate(1);
    }
}
