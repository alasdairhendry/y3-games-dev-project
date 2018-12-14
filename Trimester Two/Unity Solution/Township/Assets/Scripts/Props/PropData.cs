using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class PropData {

    public string name;
    public string description;
    public string prefabName;
    public Sprite sprite;
    public PropCategory category;
    public PlacementType placementType;
    public List<RequiredBuildMaterial> requiredMaterials = new List<RequiredBuildMaterial> ();
	
    public GameObject Prefab { get { string path = "Props/" + category.ToString() + "/" + prefabName + "_Prefab"; return Resources.Load<GameObject> ( path ) as GameObject; } }
}

[System.Serializable]
public class RequiredBuildMaterial
{
    public int id;
    public float amount;
}
