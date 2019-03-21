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

        if (Instance == this)
        {
            CreateProps ();
        }
    }

    [SerializeField] public List<PropData> propData = new List<PropData> ();
    public Dictionary<PropCategory, List<PropData>> propDataCategorised = new Dictionary<PropCategory, List<PropData>> ();

    //public List<GameObject> worldProps = new List<GameObject> ();
    //public Dictionary<Type, List<GameObject>> worldPropsDict = new Dictionary<Type, List<GameObject>> (); 

    private void Start ()
    {
        GenerateDictionaries ();
    }

    private void CreateProps ()
    {
        propData.Clear ();
        CreateProps_Storage ();
        CreateProps_Housing ();
        CreateProps_Gathering ();
        CreateProps_Production ();
        CreateProps_Misc ();
        //propData.Add ( new PropData (
        //    "",
        //    "",
        //    "",
        //    PropCategory.Gathering,
        //    PlacementType.Plopable,
        //    PlacementArea.Ground,
        //    3.0f,
        //    new List<RequiredBuildMaterial> ()
        //    {
        //            { new RequiredBuildMaterial(0, 6) },
        //            { new RequiredBuildMaterial(1, 2) }
        //    }
        //) );
    }

    private void CreateProps_Housing ()
    {
        propData.Add ( new PropData (
          "House",
          "A basic, wooden house",
          "Basic_House",
          PropCategory.Housing,
          PlacementType.Plopable,
          PlacementArea.Ground,
          3.0f,
          new List<RequiredBuildMaterial> ()
          {
                { new RequiredBuildMaterial(0, 6) },
                { new RequiredBuildMaterial(1, 2) }
          }
      ) );
    }

    private void CreateProps_Storage ()
    {
        propData.Add ( new PropData (
          "Warehouse",
          "Stores 40 tons of any resource",
          "Basic_Warehouse",
          PropCategory.Storage,
          PlacementType.Plopable,
          PlacementArea.Ground,
          3.0f,
          new List<RequiredBuildMaterial> ()
          {

          }
      ) );
    }

    private void CreateProps_Gathering ()
    {
        propData.Add ( new PropData (
           "Lumberjack's Hut",
           "Allows a lumberjack to chop logs",
           "LumberjackHut",
           PropCategory.Gathering,
           PlacementType.Plopable,
           PlacementArea.Ground,
           3.0f,
           new List<RequiredBuildMaterial> ()
           {
                    { new RequiredBuildMaterial(0, 12) },
                    { new RequiredBuildMaterial(1, 4) }
           }
       ) );

        propData.Add ( new PropData (
          "Fishing Hut",
          "Allows a fisherman to catch fish",
          "FishingHut",
          PropCategory.Gathering,
          PlacementType.Plopable,
          PlacementArea.Waterside,
          3.0f,
          new List<RequiredBuildMaterial> ()
          {
                            { new RequiredBuildMaterial(0, 4) },
                            { new RequiredBuildMaterial(1, 4) }
          }
        ) );

        propData.Add ( new PropData (
            "Quarry",
            "Allows a quarryman to mine bricks",
            "Quarry",
            PropCategory.Gathering,
            PlacementType.Plopable,
            PlacementArea.Ground,
            3.0f,
            new List<RequiredBuildMaterial> ()
            {
                    { new RequiredBuildMaterial(0, 12) },
                    { new RequiredBuildMaterial(1, 24) }
            }
        ) );

        propData.Add ( new PropData (
          "Mine",
          "Allows a miner to gather iron ore",
          "Mine",
          PropCategory.Gathering,
          PlacementType.Plopable,
          PlacementArea.Ground,
          3.0f,
          new List<RequiredBuildMaterial> ()
          {
                    { new RequiredBuildMaterial(0, 16) },
                    { new RequiredBuildMaterial(1, 32) },
                    { new RequiredBuildMaterial(2, 10) }
          }
      ) );

        propData.Add ( new PropData (
          "Vineyard",
          "Allows a Vintner to grow apples",
          "Orchard",
          PropCategory.Gathering,
          PlacementType.Plopable,
          PlacementArea.Ground,
          3.0f,
          new List<RequiredBuildMaterial> ()
          {
                    { new RequiredBuildMaterial(0, 16) },
                    { new RequiredBuildMaterial(1, 14) }
          }
      ) );
    }

    private void CreateProps_Production ()
    {
        propData.Add ( new PropData (
           "Stonemason's Hut",
           "Allows a stonemason to craft bricks into stone",
           "StonemasonHut",
           PropCategory.Production,
           PlacementType.Plopable,
           PlacementArea.Ground,
           3.0f,
           new List<RequiredBuildMaterial> ()
           {
                    { new RequiredBuildMaterial(0, 12) },
                    { new RequiredBuildMaterial(1, 24) }
           }
       ) );



        propData.Add ( new PropData (
           "Coal Burner Hut",
           "Allows a Charcoal Burner to turn wood into charcoal",
           "CharcoalBurnerHut",
           PropCategory.Production,
           PlacementType.Plopable,
           PlacementArea.Ground,
           3.0f,
           new List<RequiredBuildMaterial> ()
           {
                    { new RequiredBuildMaterial(0, 12) },
                    { new RequiredBuildMaterial(1, 8) }
           }
       ) );

        propData.Add ( new PropData (
           "Smithery",
           "Allows a blacksmith to smelt iron ore into iron bars",
           "Smithery",
           PropCategory.Production,
           PlacementType.Plopable,
           PlacementArea.Ground,
           3.0f,
           new List<RequiredBuildMaterial> ()
           {
                            { new RequiredBuildMaterial(0, 16) },
                            { new RequiredBuildMaterial(1, 10) }
           }
        ) );

        propData.Add ( new PropData (
           "Winery",
           "Allows a Winemaker to ferment grapes into wine",
           "Brewery",
           PropCategory.Production,
           PlacementType.Plopable,
           PlacementArea.Ground,
           3.0f,
           new List<RequiredBuildMaterial> ()
           {
                    { new RequiredBuildMaterial(0, 16) },
                    { new RequiredBuildMaterial(1, 10) }
           }
       ) );
    }

    private void CreateProps_Misc ()
    {
        propData.Add ( new PropData (
   "Campfire",
   "Provide a place for your citizens to relax",
   "Campfire",
   PropCategory.Misc,
   PlacementType.Plopable,
   PlacementArea.Ground,
   3.0f,
   new List<RequiredBuildMaterial> ()
   {
                    { new RequiredBuildMaterial(0, 4) },
                    { new RequiredBuildMaterial(1, 4) }
   }
) );
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

    public PropData GetPropDataByName(string name)
    {
        for (int i = 0; i < propData.Count; i++)
        {
            if (propData[i].name == name) return propData[i];
        }

        return null;
    }

    //public List<GameObject> GetWorldPropsByType(Type type)
    //{
    //    if (!worldPropsDict.ContainsKey ( type ))
    //    {
    //        return new List<GameObject> ();
    //    }
    //    else
    //    {
    //        return worldPropsDict[type];
    //    }
    //}

    //public void OnPropBuilt (GameObject prop)
    //{
    //    if (!worldProps.Contains ( prop ))
    //    {
    //        worldProps.Add ( prop );

    //        if (!worldPropsDict.ContainsKey ( prop.GetComponent<Prop>().GetType() ))
    //        {
    //            worldPropsDict.Add ( prop.GetComponent<Prop> ().GetType(), new List<GameObject> () { prop } );
    //        }
    //        else
    //        {
    //            worldPropsDict[prop.GetComponent<Prop> ().GetType ()].Add ( prop );
    //        }
    //    }
    //}

    //public void OnPropDestroyed(GameObject prop)
    //{
    //    if (worldProps.Contains ( prop ))
    //    {
    //        worldProps.Remove ( prop );

    //        if (!worldPropsDict.ContainsKey ( prop.GetComponent<Prop> ().GetType () ))
    //        {
    //            // Do nothing, this should never happen
    //            Debug.LogError ( "This should never happen" );
    //        }
    //        else
    //        {
    //            if(worldPropsDict[prop.GetComponent<Prop> ().GetType ()].Contains(prop))
    //            {
    //                worldPropsDict[prop.GetComponent<Prop> ().GetType ()].Remove ( prop );
    //            }
    //        }
    //    }
    //}
}
