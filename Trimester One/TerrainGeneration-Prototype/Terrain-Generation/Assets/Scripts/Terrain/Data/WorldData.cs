using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class WorldData : UpdatableScriptableObject {

    public float heightMultiplier;
    public AnimationCurve heightCruve;

    public bool useFlatShading;
    public bool useFalloffMap;

    [Range(0.001f, 10.0f)] public float falloffMapParamA;
    [Range(1.0f, 50.0f)] public float falloffMapParamB;

    public float GetMinHeight()
    {
        Debug.Log("Got Min Height: " + heightMultiplier * heightCruve.Evaluate(0));
        return heightMultiplier * heightCruve.Evaluate(0);
    }

    public float GetMaxHeight()
    {
        Debug.Log("Got Max Height: " + heightMultiplier * heightCruve.Evaluate(1));
        return heightMultiplier * heightCruve.Evaluate(1);
    }
}
