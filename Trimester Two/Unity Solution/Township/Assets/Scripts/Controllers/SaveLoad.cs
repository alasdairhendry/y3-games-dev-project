using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoad : MonoBehaviour {

    public static SaveLoad Instance;

    private void Awake ()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy ( this.gameObject );

        DontDestroyOnLoad ( this.gameObject );
    }

    private void Update ()
    {
        if (Input.GetKeyDown ( KeyCode.L ))
        {
            Load ( "" );
        }
    }

    public void Save (string saveName)
    {
        List<GameObject> citizenObjects = EntityManager.Instance.GetEntitiesByType ( typeof ( CitizenBase ) );
        List<GameObject> propObjects = EntityManager.Instance.GetPropEntites ();

        List<CitizenBase> citizenBases = new List<CitizenBase> ();
        List<Prop> props = new List<Prop> ();

        for (int i = 0; i < citizenObjects.Count; i++)
        {
            if (citizenObjects[i].GetComponent<CitizenBase> () != null)
            {
                citizenBases.Add ( citizenObjects[i].GetComponent<CitizenBase> () );
            }
        }

        for (int i = 0; i < propObjects.Count; i++)
        {
            if (propObjects[i].GetComponent<Prop> () != null)
            {
                props.Add ( propObjects[i].GetComponent<Prop> () );
            }
        }

        //PersistentData.Save ( citizenBases, props, saveName );
        PersistentData.Save ( citizenBases, props, "MySaveFile");
    }

    public void Load (string saveName)
    {
        //PersistentData.SaveData data = PersistentData.Load ( saveName );
        PersistentData.SaveData data = PersistentData.Load ( "MySaveFile" );

        if (data == null) return;
        WarehouseController.Instance.Load ( data );
        FindObjectOfType<CameraMovement> ().LOAD_Position ( data );
        GamePreferences.Instance.Load ( data.gamePreferences );

        Load_Citizens ( data );
        Load_Props ( data );
        Load_GameTime ( data.gameTime );
    }

    private void Load_Citizens (PersistentData.SaveData data)
    {
        for (int i = 0; i < data.citizens.Count; i++)
        {
            DEBUG_CHARACTER_SPAWNER.Instance.LOAD_Citizen ( data.citizens[i] );
        }

        DEBUG_CHARACTER_SPAWNER.Instance.LOAD_CitizenFamilies ();
    }

    private void Load_Props(PersistentData.SaveData data)
    {
        for (int i = 0; i < data.props.Count; i++)
        {
            PropData propData = PropManager.Instance.GetPropDataByName ( data.props[i].PropName );

            GameObject go = Instantiate ( propData.Prefab );
            go.transform.position = data.props[i].Position.ToVector3 () ;
            go.transform.eulerAngles = data.props[i].Rotation.ToVector3();
            go.transform.name = "PlacedObject: " + propData.name;

            Prop prop = go.GetComponent<Prop> ();
            prop.LOAD_Place ( data.props[i], propData );
        }
    }

    private void Load_GameTime (PersistentData.GameTimeData data)
    {
        GameTime.Load ( data );
    }
}
