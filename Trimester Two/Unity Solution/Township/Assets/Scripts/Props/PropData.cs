using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class PropData {

    public string name;
    public string description;
    public string prefabName;

    public int costToPlace;
    public int dailyUpkeep;

    public PropCategory category;
    public PlacementType placementType;
    public PlacementArea placementArea;
    public float UIOffsetY = 3.0f;

    public bool locked { get; protected set; } = false;
	
    public List<RequiredBuildMaterial> requiredMaterials = new List<RequiredBuildMaterial> ();

    public Sprite buildModeSprite { get { return Resources.Load<Sprite> ( "UI/Props/" + prefabName ) as Sprite; } }
    public GameObject Prefab { get { string path = "Props/" + category.ToString () + "/" + prefabName + "_Prefab"; return Resources.Load<GameObject> ( path ) as GameObject; } }

    public PropData (string name, string description, string prefabName, int costToPlace, PropCategory category, PlacementType placementType, PlacementArea placementArea, float uIOffsetY, List<RequiredBuildMaterial> requiredMaterials, bool locked = true)
    {
        this.name = name;
        this.description = description;
        this.prefabName = prefabName;

        this.costToPlace = costToPlace;
        this.dailyUpkeep = Mathf.FloorToInt ( (float)costToPlace * 0.05f );

        this.category = category;
        this.placementType = placementType;
        this.placementArea = placementArea;
        this.UIOffsetY = uIOffsetY;
        this.requiredMaterials = requiredMaterials;
        this.locked = locked;
    }

    public void Unlock ()
    {
        if (!this.locked) return;
        this.locked = false;
        GameObject.FindObjectOfType<HUD_Build_Panel> ()?.Refresh ();
    }
}

[System.Serializable]
public class RequiredBuildMaterial
{
    public int id;
    public float amount;

    public RequiredBuildMaterial (int id, float amount)
    {
        this.id = id;
        this.amount = amount;
    }
}
