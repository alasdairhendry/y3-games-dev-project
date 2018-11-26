using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class BiomeData : UpdatableScriptableObject {

    public BiomeLayer[] layers;

    [System.Serializable]
    public class BiomeLayer
    {
        public string biomeName;
        public float startHeight;
        public float endHeight;
    }
}
