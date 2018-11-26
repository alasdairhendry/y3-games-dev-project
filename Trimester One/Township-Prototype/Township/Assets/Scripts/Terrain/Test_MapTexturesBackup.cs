using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_MapTexturesBackup : MonoBehaviour {

    [SerializeField] private TerrainType[] regions;
	
    public void SetRegionBackup(TerrainType[] regions)
    {
        this.regions = regions;
    }

    [System.Serializable]
    public struct TerrainType
    {
        public string name;
        public float height;
        public Color colour;
    }
}
