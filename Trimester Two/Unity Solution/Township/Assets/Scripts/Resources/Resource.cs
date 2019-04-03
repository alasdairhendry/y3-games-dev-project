using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Resource {

    public int id;
    public string name;
    public string description;

    private string imagePath;
    public bool HasImage { get { return !string.IsNullOrEmpty ( imagePath ); } }
    public Sprite image { get { return Resources.Load<Sprite> ( imagePath ) as Sprite; } }

    private string prefabPath;
    public bool HasPrefab { get { return !string.IsNullOrEmpty ( prefabPath ); } }
    public GameObject MaterialPrefab { get { return Resources.Load<GameObject> ( prefabPath ) as GameObject; } }

    /// <summary>
    /// How much does the player have to pay to buy this resource?
    /// </summary>
    public float baseBuyPrice { get; protected set; }

    /// <summary>
    /// How much does the player get for selling this resource?
    /// </summary>
    public float baseSellPrice { get; protected set; }

    public Resource(int id, string name, string description, float baseBuyPrice, float baseSellPrice)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.baseBuyPrice = baseBuyPrice;
        this.baseSellPrice = baseSellPrice;

        imagePath = "UI/Resource/" + name;
        prefabPath = "Resource/Resource_" + name + "_Prefab";
    }

    public void HoldItem (CitizenGraphics graphics, bool state)
    {
        if (id == 0)
        {
            graphics.SetUsingLogs ( state );
        }
        else if (id == 1)
        {
            graphics.SetUsingRocks ( state );
        }
        else
        {
            graphics.SetUsingCrate ( state );
        }
    }

    public static void DropResource (int id, float quantity, Vector3 position, Vector3 rotation)
    {
        GameObject go = GameObject.Instantiate ( Resources.Load<GameObject> ( "RawMaterialBoxed_Prefab" ) as GameObject );
        go.transform.position = position;
        go.transform.eulerAngles = rotation;
        go.GetComponent<RawMaterial> ().SetValues ( id, quantity, 0.0f, "Collect" );
    }

    public static void DropResource (int id, float quantity, Transform tran)
    {
        GameObject go = GameObject.Instantiate ( Resources.Load<GameObject> ( "RawMaterialBoxed_Prefab" ) as GameObject );
        go.transform.position = tran.position + (tran.forward);
        go.transform.eulerAngles = tran.eulerAngles;
        go.GetComponent<RawMaterial> ().SetValues ( id, quantity, 0.0f, "Collect" );
    }
}