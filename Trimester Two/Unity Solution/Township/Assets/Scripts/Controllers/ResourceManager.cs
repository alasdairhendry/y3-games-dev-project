using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour {

    public static ResourceManager Instance;

    private Dictionary<int, Resource> indexDictionary = new Dictionary<int, Resource> ();
    private Dictionary<string, Resource> nameDictionary = new Dictionary<string, Resource> ();
    [SerializeField] private List<Resource> resourceList = new List<Resource> ();
    //private int currentCreateIndex = 0;

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
        /// DO NOT CHANGE THE ORDER

        CreateResource ( 0, "Wood", "A lightweight building material" );
        CreateResource ( 1, "Brick", "A heavy building material" );

        CreateResource ( 2, "Stone", "A decorative building" );

        CreateResource ( 3, "Iron Ore", "A strong mineral that can be harvested from the dirt" );
        CreateResource ( 4, "Iron Bar", "A durable building material" );
        CreateResource ( 5, "Charcoal", "Used in advanced metal production" );

        CreateResource ( 6, "Apple", "Can be fermented into alcohol" );
        CreateResource ( 7, "Cider", "Increase happiness for citizens" );

        CreateResource ( 8, "Tool", "Increases production for many professions" );

        CreateResource ( 9, "Meat", "Provides energy to your citizens" );
        CreateResource ( 10, "Fish", "Provides energy to your citizens" );
    }

    private void CreateResource(int id, string name, string description)
    {
        Resource r = new Resource ( id, name, description );

        indexDictionary.Add ( id, r );
        nameDictionary.Add ( r.name, r );
        resourceList.Add ( r );

        //currentCreateIndex++;
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
