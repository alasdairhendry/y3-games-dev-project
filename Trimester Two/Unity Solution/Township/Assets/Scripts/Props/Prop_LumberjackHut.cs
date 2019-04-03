using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop_LumberjackHut : Prop_Profession {

    [SerializeField] private List<GameObjectList> trees = new List<GameObjectList> ();
    [SerializeField] private List<GameObject> stumps = new List<GameObject> ();

    protected override void OnPlaced ()
    {
        base.OnPlaced ();

        for (int i = 0; i < PropManager.Instance.propData.Count; i++)
        {
            PropManager.Instance.propData[i].Unlock ();
        }
    }

    protected override void SetResources ()
    {
        resourceIDToGive = 0;
        resourceIDToConsume = -1;

        giveAmount = 1;
        consumeAmount = 0;

        ProductionRequired = 20.0f;
    }

    // Called from Prop_Profession on built "MaxJobs" times
    protected override void CreateProfessionJobs (int index)
    {
        Job_Lumberjack job = GetComponent<JobEntity> ().CreateJob_Lumberjack ( "Lumberjacking", !HaltProduction, 5.0f, null, this, trees[index].Objects, stumps[index] );
        professionJobs.Add ( job );
    }

    protected override void SetInspectable ()
    {
        base.SetInspectable ();     
    }
}

[System.Serializable]
public class GameObjectList
{
    [SerializeField] private List<GameObject> objects = new List<GameObject> ();
    public List<GameObject> Objects { get { return objects; } }

    public GameObject this[int key]
    {
        get
        {
            return objects[key];
        }
        set
        {
            objects[key] = value;
        }
    }
}
