using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEntity : MonoBehaviour {

    public enum EntityType { NotSet, Citizen, Building, Resource }

    [SerializeField] private string entityName;
    public string EntityName { get { return entityName; } }

    [SerializeField] private EntityType type;
    public EntityType Type { get { return type; } }

    private void Start ()
    {
        UpdateGameObjectName ();
    }

    private void UpdateGameObjectName ()
    {
        if(type == EntityType.NotSet) { Debug.LogError ( "Entity type not set", this.gameObject ); }
        if (string.IsNullOrEmpty ( entityName )) { return; }

        gameObject.name = "Entity_" + type.ToString() + "_" + EntityName.ToString ();
    }

    public WorldEntity Setup (string name, EntityType type)
    {
        this.entityName = name;
        this.type = type;
        UpdateGameObjectName ();
        return this;
    }

    public WorldEntity SetType (EntityType type)
    {
        this.type = type;
        UpdateGameObjectName ();
        return this;
    }

}
