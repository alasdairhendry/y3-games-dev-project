using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropManager : MonoBehaviour {

    public static PropManager Instance;

    private void Awake ()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy ( this.gameObject );
    }

    [SerializeField] public List<PropData> propData = new List<PropData> ();
    public Dictionary<PropCategory, List<PropData>> propDataCategorised = new Dictionary<PropCategory, List<PropData>> ();
    //public Dictionary<Type, List<GameObject>> worldProps = new Dictionary<Type, List<GameObject>> ();
    public List<GameObject> worldProps = new List<GameObject> ();

    private void Start ()
    {
        GenerateDictionaries ();
    }

    private void GenerateDictionaries ()
    {
        for (int i = 0; i < propData.Count; i++)
        {
            if (propDataCategorised.ContainsKey ( propData[i].category ))
            {
                propDataCategorised[propData[i].category].Add ( propData[i] );
            }
            else
            {
                propDataCategorised.Add ( propData[i].category, new List<PropData> () { propData[i] } );
            }            
        }
    }

    public List<PropData> GetPropsByCategory(PropCategory category)
    {
        if (!propDataCategorised.ContainsKey ( category )) { Debug.LogError ( "Category " + category + " does not exist in dictionary" ); return null; }
        return propDataCategorised[category];
    }

    public void OnPropBuilt (GameObject prop)
    {
        if (!worldProps.Contains ( prop ))
        {
            worldProps.Add ( prop );
        }
    }
}
