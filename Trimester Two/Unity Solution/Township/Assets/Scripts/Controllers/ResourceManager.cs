using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour {

    public static ResourceManager Instance;

    private Dictionary<int, Resource> indexDictionary = new Dictionary<int, Resource> ();
    private Dictionary<string, Resource> nameDictionary = new Dictionary<string, Resource> ();
    [SerializeField] private List<Resource> resourceList = new List<Resource> ();
    private int currentCreateIndex = 0;

    private void Awake ()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy ( this.gameObject );

        CreateResources ();
    }

    private void CreateResources ()
    {
        CreateResource ( "Wood", "A lightweight building material." );
        CreateResource ( "Brick", "A heavy building material." );

        CreateResource ( "Stone", "A decorative building." );

        CreateResource ( "Iron Ore", "A strong mineral that can be harvested from the dirt." );
        CreateResource ( "Iron Bar", "A durable building material." );
        CreateResource ( "Charcoal", "Used in advanced metal production." );

        CreateResource ( "Apples", "Can be fermented into alcohol." );
        CreateResource ( "Cider", "Increase happiness for citizens." );

        CreateResource ( "Tool", "Increases production for many professions." );

        CreateResource ( "Meat", "Increases production for many professions." );
    }

    private void CreateResource(string name, string description)
    {
        Resource r = new Resource ( currentCreateIndex, name, description );

        indexDictionary.Add ( currentCreateIndex, r );
        nameDictionary.Add ( r.name, r );
        resourceList.Add ( r );

        currentCreateIndex++;
    }

    public Resource GetResourceByID(int id)
    {
        if (indexDictionary.ContainsKey ( id ))
            return indexDictionary[id];
        else { Debug.LogError ( "No resource with this ID" ); return null; }
    }

    public Resource GetResourceByName(string name)
    {
        if (nameDictionary.ContainsKey ( name ))
            return nameDictionary[name];
        else { Debug.LogError ( "No resource with this I" +
            "D" ); return null; }
    }

    public List<Resource> GetResourceList () { return resourceList; }
}
