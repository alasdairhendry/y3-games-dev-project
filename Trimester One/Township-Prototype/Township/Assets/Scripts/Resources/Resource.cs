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

    public Resource(int id, string name, string description)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        imagePath = "Images/Resources/Resource_" + name;
        prefabPath = "Resource/Resource_" + name + "_Prefab";
    }
}