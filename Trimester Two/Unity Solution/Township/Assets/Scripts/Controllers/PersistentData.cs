using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class PersistentData {

    private static string path = Path.Combine ( Application.persistentDataPath, "SaveFiles" );

    public static void Save (List<CitizenBase> citizens, List<Prop> props, List<World.SerializedEnvironment> entities, string saveName)
    {       
        if (!Directory.Exists ( path )) Directory.CreateDirectory ( path );
        if (string.IsNullOrEmpty ( saveName )) { Debug.LogError ( "Save Name Was Null - GAME WAS NOT SAVED!" ); return; }

        SaveData existingData = Load ( saveName );

        BinaryFormatter bf = new BinaryFormatter ();
        FileStream stream = new FileStream ( Path.Combine ( path, saveName + ".township" ), FileMode.OpenOrCreate );

        SaveData data = new SaveData ();
        data.townName = GameData.Instance.CurrentSaveFileName;

        if (existingData == null)
            data.created = System.DateTime.Now;
        else data.created = existingData.created;

        data.lastPlayed = System.DateTime.Now;

        data.startingConditions = GameData.Instance.startingConditions;

        data.money = MoneyController.Instance.money;

        data.taxBands = TaxController.Instance.taxBands;
        data.GlobalTaxModifier = TaxController.Instance.GlobalTaxModifier;

        data.familyIDCounter = CitizenController.Instance.familyIDCounter;
        data.environmentEntities = entities;
        SaveCitizens ( ref data, citizens );
        SaveProps ( ref data, props );
        SaveGameTime ( ref data );

        data.CameraPosition = Camera.main.transform.position.ToFloat3 ();
        data.CameraRotation = Camera.main.GetComponent<CameraMovement> ().currentRotation;

        List<Resource> resourceList = ResourceManager.Instance.GetResourceList ();

        data.worldPresetIndex = WorldPresets.Instance.selectedIndex;

        for (int x = 0; x < resourceList.Count; x++)
        {
            data.WarehouseResourceIDs.Add ( resourceList[x].id );
            data.WarehouseResourceQuantities.Add ( Mathf.Clamp ( WarehouseController.Instance.Inventory.inventoryOverall[resourceList[x].id], 0.0f, float.MaxValue ) );
        }

        data.gamePreferences = GamePreferences.Instance.preferences;

        bf.Serialize ( stream, data );
        Debug.Log ( "Saved to " + path );
        stream.Close ();
    }

    private static void SaveCitizens(ref SaveData data, List<CitizenBase> citizens)
    {
        for (int i = 0; i < citizens.Count; i++)
        {
            CitizenBase cBase = citizens[i];

            CitizenData citizenData = new CitizenData ()
            {
                ID = cBase.ID,
                FirstName = cBase.CitizenFamily.thisMember.firstName,
                FamilyName = cBase.CitizenFamily.familyName,
                SkinColour = cBase.CitizenFamily.thisMember.skinColour,
                Position = cBase.gameObject.transform.position.ToFloat3 (),
                FamilyID = cBase.CitizenFamily.familyID,
                Gender = (int)cBase.CitizenFamily.gender,
                Profession = (int)cBase.CitizenJob.profession,
                Age = cBase.CitizenAge.Age,
                Birthday = cBase.CitizenAge.Birthday,

                isWidowed = cBase.CitizenFamily.isWidowed,
                isPregnant = cBase.CitizenFamily.isPregnant,
                pregnancyDueDate = cBase.CitizenFamily.pregnancyDueDay
            };

            if (cBase.CitizenFamily.Father.citizenBase != null)
                citizenData.FatherID = cBase.CitizenFamily.Father.citizenBase.ID;
            else citizenData.FatherID = -1;

            if (cBase.CitizenFamily.Mother.citizenBase != null)
                citizenData.MotherID = cBase.CitizenFamily.Mother.citizenBase.ID;
            else citizenData.MotherID = -1;

            if (cBase.CitizenFamily.Partner != null)
            {
                if (cBase.CitizenFamily.Partner.citizenBase != null)
                    citizenData.PartnerID = cBase.CitizenFamily.Partner.citizenBase.ID;
                else citizenData.PartnerID = -1;
            }
            else citizenData.PartnerID = -1;

            for (int x = 0; x < cBase.CitizenFamily.Children.Count; x++)
            {
                if (cBase.CitizenFamily.Children[x].citizenBase != null)
                {
                    citizenData.ChildrenIDs.Add ( cBase.CitizenFamily.Children[x].citizenBase.ID );
                }
            }

            List<Resource> resourceList = ResourceManager.Instance.GetResourceList ();

            if (citizens[i].Inventory != null)
            {
                for (int x = 0; x < resourceList.Count; x++)
                {
                    citizenData.ResourceIDs.Add ( resourceList[x].id );
                    citizenData.ResourceQuantities.Add ( Mathf.Clamp ( citizens[i].Inventory.inventoryOverall[resourceList[x].id], 0.0f, float.MaxValue ) );
                }
            }

            data.citizens.Add ( citizenData );
        }
    }

    private static void SaveProps(ref SaveData data, List<Prop> props)
    {
        List<Resource> resourceList = ResourceManager.Instance.GetResourceList ();

        for (int i = 0; i < props.Count; i++)
        {
            Prop prop = props[i];

            PropData propData = new PropData
            {
                PropName = prop.data.name,
                Position = prop.gameObject.transform.position.ToFloat3 (),
                Rotation = prop.gameObject.transform.eulerAngles.ToFloat3 (),
            };

            if (prop.inventory != null)
            {
                for (int x = 0; x < resourceList.Count; x++)
                {
                    propData.PropResourceIDs.Add ( resourceList[x].id );
                    propData.PropResourceQuantities.Add ( Mathf.Clamp ( prop.inventory.inventoryOverall[resourceList[x].id], 0.0f, float.MaxValue ) );
                }

                propData.PropInventoryCapacity = prop.inventory.EntryCapacity;
            }

            if (prop.buildable != null)
            {
                if (prop.buildable.inventory != null)
                {
                    for (int x = 0; x < resourceList.Count; x++)
                    {
                        propData.BuildableResourceIDs.Add ( resourceList[x].id );
                        propData.BuildableResourceQuantities.Add ( Mathf.Clamp ( prop.buildable.inventory.inventoryOverall[resourceList[x].id], 0.0f, float.MaxValue ) );
                    }
                }

                propData.BuildableStage = prop.buildable.currentStage;
                propData.ConstructionPercent = prop.buildable.ConstructionPercent;
                propData.BuildableInventoryCapacity = prop.buildable.inventory.EntryCapacity;
            }

            data.props.Add ( propData );
        }
    }

    private static void SaveGameTime(ref SaveData data)
    {
        data.gameTime.dayOfMonth = GameTime.currentDayOfTheMonth;
        data.gameTime.dayOfYear = GameTime.currentDayOfTheYear;
        data.gameTime.month = GameTime.currentMonth;
        data.gameTime.year = GameTime.currentYear;
        data.gameTime.dayOfGame = GameTime.currentDayOfTheGame;
    }

    public static SaveData Load (string saveName)
    {
        if (!Directory.Exists ( path )) { Debug.LogError ( "Save Directory Does Not Exist" ); return null; }
        if (!File.Exists ( Path.Combine ( path, saveName + ".township" ) )) { Debug.LogError ( "Save File " + saveName + " Does Not Exist" ); return null; }

        BinaryFormatter bf = new BinaryFormatter ();
        FileStream stream = new FileStream ( Path.Combine ( path, saveName + ".township" ), FileMode.OpenOrCreate );

        if(stream.Length > 0)
        {
            SaveData data = (SaveData)bf.Deserialize ( stream );
            stream.Close ();
            return data;
        }
        else
        {
            stream.Close ();
            return null;
        }


        //Debug.Log ( "Loaded from " + path );
        //stream.Close ();
    
        //return data;
    }

    public static bool SaveFileExists(string saveName)
    {
        if (!Directory.Exists ( path )) { Directory.CreateDirectory ( path ); Debug.LogError ( "Save Directory Does Not Exist" ); return false; }
        if (!File.Exists ( Path.Combine ( path, saveName + ".township" ) )) { Debug.LogError ( "Save File Does Not Exist" ); return false; }
        return true;
    }

    public static List<string> FetchSaveFiles ()
    {
        if (!Directory.Exists ( path )) { Directory.CreateDirectory ( path ); Debug.LogError ( "Save Directory Does Not Exist" ); return new List<string> (); }
        List<string> files = Directory.GetFiles ( path ).ToList ();

        for (int i = 0; i < files.Count; i++)
        {
            FileInfo file = new FileInfo ( files[i] );
            files[i] = file.Name;            

            if (files[i].Contains ( ".township" ))
            {
                files[i] = files[i].Replace ( ".township", "" );
            }
            else
            {
                //files.RemoveAt ( i );
                continue;
            }
        }

        return files;
    }

    public static void DeleteSaveFile(string saveName)
    {
        if (!Directory.Exists ( path )) { Debug.LogError ( "Save Directory Does Not Exist" ); }
        if (!File.Exists ( Path.Combine ( path, saveName + ".township" ) )) { Debug.LogError ( "Save File " + saveName + " Does Not Exist" ); }

        File.Delete ( Path.Combine ( path, saveName + ".township" ) );
    }

    [System.Serializable]
    public class SaveData
    {
        public string townName;
        public System.DateTime created;
        public System.DateTime lastPlayed;

        public int familyIDCounter;
        public int money;

        public List<TaxController.TaxBand> taxBands = new List<TaxController.TaxBand> ();
        public float GlobalTaxModifier;

        public List<CitizenData> citizens = new List<CitizenData> ();
        public List<PropData> props = new List<PropData> ();
        public GameTimeData gameTime = new GameTimeData ();

        public int worldPresetIndex;
        public List<World.SerializedEnvironment> environmentEntities = new List<World.SerializedEnvironment> ();

        public List<int> WarehouseResourceIDs = new List<int> ();
        public List<float> WarehouseResourceQuantities = new List<float> ();

        public GamePreferences.Preferences gamePreferences;
        public GameData.StartingCondition startingConditions;

        public float[] CameraPosition;
        public float CameraRotation;

    }

    [System.Serializable]
    public class CitizenData
    {
        public int ID;
        public float[] Position;
        public string FirstName;
        public string FamilyName;
        public int SkinColour;

        public int FatherID;
        public int MotherID;
        public int PartnerID;
        public List<int> ChildrenIDs = new List<int> ();
        public int FamilyID;

        public int Gender;
        public int Profession;
        public int Age;
        public int Birthday;

        public bool isWidowed;
        public bool isPregnant;
        public int pregnancyDueDate;
        //public int pregnancyPartnerID;

        public List<int> ResourceIDs = new List<int> ();
        public List<float> ResourceQuantities = new List<float> ();
    }

    [System.Serializable]
    public class PropData
    {
        public string PropName;
        public float[] Position;
        public float[] Rotation;

        public List<int> PropResourceIDs = new List<int> ();
        public List<float> PropResourceQuantities = new List<float> ();
        public float PropInventoryCapacity;

        public List<int> BuildableResourceIDs = new List<int> ();
        public List<float> BuildableResourceQuantities = new List<float> ();
        public float BuildableInventoryCapacity;

        public int BuildableStage;
        public float ConstructionPercent;
    }

    [System.Serializable]
    public class GameTimeData
    {
        public int dayOfMonth;
        public int dayOfYear;
        public int dayOfGame;
        public int month;
        public int year;
    }
}
