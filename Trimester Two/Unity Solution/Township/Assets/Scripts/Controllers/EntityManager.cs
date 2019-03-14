using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityManager : MonoBehaviour {

    public static EntityManager Instance;

    private List<GameObject> worldEntities = new List<GameObject> ();
    private Dictionary<Type, List<GameObject>> worldEntitiesDictionary = new Dictionary<Type, List<GameObject>> ();
    private List<GameObject> propEntities = new List<GameObject> ();

    private void Awake ()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy ( this.gameObject );
    }

    /// <summary>
    /// Warning: This contains Props that have not yet been built.
    /// </summary>
    /// <param name="type">The list of entity types you want, e.g Prop_Warehouse or CitizenBase</param>
    /// <returns></returns>
    public List<GameObject> GetEntitiesByType (Type type)
    {
        if (!worldEntitiesDictionary.ContainsKey ( type ))
        {
            return new List<GameObject> ();
        }
        else
        {
            return worldEntitiesDictionary[type];
        }
    }

    public List<GameObject> GetPropEntites ()
    {
        return propEntities;
    }

    public CitizenBase GetCitizenBaseByID(int id)
    {
        if (!worldEntitiesDictionary.ContainsKey ( typeof ( CitizenBase ) ))
        {
            Debug.LogError ( "No Citizen Bases Created Yet." );
            return null;
        }
        else
        {
            return worldEntitiesDictionary[typeof ( CitizenBase )].First ( x => x.GetComponent<CitizenBase> ().ID == id ).GetComponent<CitizenBase> ();
        }
    }

    public void OnEntityCreated (GameObject gameObject, Type type)
    {
        if (!worldEntities.Contains ( gameObject ))
        {
            worldEntities.Add ( gameObject );

            if (!worldEntitiesDictionary.ContainsKey ( type ))
            {
                worldEntitiesDictionary.Add ( type, new List<GameObject> () { gameObject } );
            }
            else
            {
                worldEntitiesDictionary[type].Add ( gameObject );
            }
        }

        if (gameObject.GetComponent<Prop> () != null)
        {
            if (!propEntities.Contains ( gameObject ))
            {
                propEntities.Add ( gameObject );
            }
        }
    }

    public void OnEntityDestroyed (GameObject gameObject, Type type)
    {
        if (worldEntities.Contains ( gameObject ))
        {
            worldEntities.Remove ( gameObject );

            if (!worldEntitiesDictionary.ContainsKey ( type ))
            {
                // Do nothing, this should never happen
                Debug.LogError ( "This should never happen" );
            }
            else
            {
                if (worldEntitiesDictionary[type].Contains ( gameObject ))
                {
                    worldEntitiesDictionary[type].Remove ( gameObject );
                }
            }
        }

        if (gameObject.GetComponent<Prop> () != null)
        {
            if (propEntities.Contains ( gameObject ))
            {
                propEntities.Remove ( gameObject );
            }
        }
    }
}
