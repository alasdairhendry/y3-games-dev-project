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
        if (Input.GetKeyDown ( KeyCode.L ) && !Hotkey.InputFieldIsActive ())
        {
            Load ( "" );
        }
    }

    public void Save (/*string saveName*/)
    {
        List<GameObject> citizenObjects = EntityManager.Instance.GetEntitiesByType ( typeof ( CitizenBase ) );
        List<GameObject> propObjects = EntityManager.Instance.GetPropEntites ();

        List<CitizenBase> citizenBases = new List<CitizenBase> ();
        List<Prop> props = new List<Prop> ();
        List<World.SerializedEnvironment> entities = FindObjectOfType<World> ().environmentEntities;

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
        PersistentData.Save ( citizenBases, props, entities, GameData.Instance.CurrentSaveFileName );
    }

    public void Load (string saveName)
    {
        //PersistentData.SaveData data = PersistentData.Load ( saveName );
        PersistentData.SaveData data = PersistentData.Load ( saveName );

        if (data == null) return;

        GameData.Instance.SetGameDataType ( GameData.GameDataType.Loaded, saveName );

        World world = FindObjectOfType<World> ();
        WorldPresets.Instance.SetPresets ( data.worldPresetIndex, world );
        world.CreateFrom_Load ( data.environmentEntities, saveName );

        // DO NOT ADD MORE LOGIC HERE - USE PostWorldLoad().
    }

    public void PostWorldLoad (string saveName)
    {
        PersistentData.SaveData data = PersistentData.Load ( saveName );
        if (data == null) { Debug.Log ( "PostWorldLoad Null - " + saveName ); return; } 

        MoneyController.Instance.Start_Loaded ( data );

        WarehouseController.Instance.Load ( data );
        TaxController.Instance.Start_Loaded ( data );
        CitizenController.Instance.LoadFamilyIDCounter ( data.familyIDCounter );
        FindObjectOfType<CameraMovement> ().LOAD_Position ( data );
        GamePreferences.Instance.Load ( data.gamePreferences );
        GameData.Instance.Load_StartingConditionsAsCustom ( data.startingConditions );

        Load_Citizens ( data );
        Load_Props ( data );
        Load_GameTime ( data.gameTime );
    }

    private void Load_Citizens (PersistentData.SaveData data)
    {
        for (int i = 0; i < data.citizens.Count; i++)
        {
            CitizenController.Instance.LOAD_Citizen ( data.citizens[i] );
        }

        CitizenController.Instance.LOAD_CitizenFamilies ();
        //DEBUG_CHARACTER_SPAWNER.Instance.LOAD_CitizenPregnancy ();
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
