using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class Job_GatherResource : Job {

    private RawMaterial rawMaterial;
    private int resourceID;
    private float resourceQuantity;
    private float currentTime = 0.0f;

    public enum Stage { TravelTo, Remove, FindWarehouse, TravelFrom }
    private Stage stage = Stage.TravelTo;

    private bool destinationProvided = false;
    private bool givenResourceToCitizen = false;

    private Prop_Warehouse targetWarehouse;

    public Job_GatherResource (JobEntity entity, string name, bool open, float timeRequired, System.Action onComplete, int resourceID, float resourceQuantity, RawMaterial rawMaterial) : base ( entity, name, open, timeRequired, onComplete )
    {
        this.resourceID = resourceID;
        this.resourceQuantity = resourceQuantity;
        this.rawMaterial = rawMaterial;
        this.professionTypes.Add ( ProfessionType.Worker );
    }

    public override void DoJob (float deltaGameTime)
    {
        switch (stage)
        {
            case Stage.TravelTo:
                DoJob_Stage_TravelTo ();
                break;

            case Stage.Remove:
                DoJob_Stage_Remove ( deltaGameTime );
                break;

            case Stage.FindWarehouse:
                DoJob_Stage_FindWarehouse ();
                break;

            case Stage.TravelFrom:
                DoJob_Stage_TravelFrom ();
                break;
        }
    }

    public override void OnCharacterAccept (CitizenBase character)
    {
        base.OnCharacterAccept ( character );        

        if (!base.cBase.Inventory.CheckCanHold(resourceID, resourceQuantity ))
        {
            OnCharacterLeave ( "Citizen can't hold that many resources", true );
        }
    }

    public override void OnCharacterLeave (string reason, bool setOpenToTrue)
    {
        this.cBase.GetComponent<CitizenGraphics> ().SetUsingAxe ( false, CitizenAnimation.AxeUseAnimation.Chopping );
        this.cBase.GetComponent<CitizenGraphics> ().SetUsingCart ( false );

        if (stage == Stage.FindWarehouse || stage == Stage.TravelFrom)
        {
            OnComplete ();
            return;
        }

        stage = Stage.TravelTo;
        currentTime = 0.0f;
        destinationProvided = false;
        targetWarehouse = null;
        givenResourceToCitizen = false;

        base.OnCharacterLeave ( reason, setOpenToTrue );
    }

    private void DoJob_Stage_TravelTo ()
    {
        if(!destinationProvided)
        {                 
            base.cBase.CitizenMovement.SetDestination ( rawMaterial.gameObject, rawMaterial.transform.position + new Vector3 ( 0.0f, 0.0f, -3.0f ) );
            destinationProvided = true;
        }

        if (!this.cBase.CitizenMovement.ReachedPath ()) return;

        destinationProvided = false;

        stage = Stage.Remove;
    }

    private void DoJob_Stage_Remove (float deltaGameTime)
    {
        this.cBase.GetComponent<CitizenGraphics> ().SetUsingAxe ( true , CitizenAnimation.AxeUseAnimation.Chopping);

        if (this.rawMaterial == null) { OnCharacterLeave ( "Material Was Destroyed", false ); return; }
        if (this.rawMaterial.gameObject == null) { OnCharacterLeave ( "Material Was Destroyed", false ); return; }

        Quaternion lookRot = Quaternion.LookRotation ( this.rawMaterial.transform.position - this.cBase.transform.position, Vector3.up );
        this.cBase.transform.rotation = Quaternion.Slerp ( this.cBase.transform.rotation, lookRot, GameTime.DeltaGameTime * 2.5f );

        currentTime += deltaGameTime;

        if(currentTime>= base.TimeRequired)
        {            
            stage = Stage.FindWarehouse;
            this.cBase.GetComponent<CitizenGraphics> ().SetUsingAxe ( false, CitizenAnimation.AxeUseAnimation.Chopping );
            base.cBase.Inventory.AddItemQuantity ( resourceID, resourceQuantity );
            rawMaterial.OnGathered ();
            givenResourceToCitizen = true;
        }
    }

    private void DoJob_Stage_FindWarehouse ()
    {
        targetWarehouse = FindEligibleWarehouse ();

        if (targetWarehouse == null)
        {
          Debug.Log ( "Null warehouse, goodbye" );
            //if (givenResourceToCitizen)
            //    character.Inventory.RemoveItemQuantity ( resourceID, resourceQuantity );

            OnComplete ();
        }
        else
        {
            stage = Stage.TravelFrom;
            this.cBase.GetComponent<CitizenGraphics> ().SetUsingLogs ( true );
        }
    }

    private void DoJob_Stage_TravelFrom ()
    {
        if (!destinationProvided)
        {
            base.cBase.CitizenMovement.SetDestination ( targetWarehouse.gameObject, targetWarehouse.CitizenInteractionPointGlobal );
            destinationProvided = true;
        }

        if (targetWarehouse == null)
        {
            OnCharacterLeave ( "Warehouse was destroyed", true );
            return;
        }

        if (!this.cBase.CitizenMovement.ReachedPath ()) return;

        targetWarehouse.inventory.AddItemQuantity ( resourceID, resourceQuantity );

        if (givenResourceToCitizen)
            cBase.Inventory.RemoveItemQuantity ( resourceID, resourceQuantity );

        givenResourceToCitizen = false;
        destinationProvided = false;

        OnComplete ();
    }

    protected override void OnComplete ()
    {
        this.cBase.GetComponent<CitizenGraphics> ().SetUsingCart ( false );

        base.OnComplete ();
    }

    private Prop_Warehouse FindEligibleWarehouse ()
    {
        List<GameObject> GOs = PropManager.Instance.worldProps;

        if (GOs == null) { Debug.LogError ( "No Warehouses found. We shouldnt run this every frame" ); return null; }

        List<Prop_Warehouse> eligibleWarehouses = new List<Prop_Warehouse> ();

        for (int i = 0; i < GOs.Count; i++)
        {
            if (GOs[i] == null) continue;
            Prop_Warehouse w = GOs[i].GetComponent<Prop_Warehouse> ();

            if (w == null) { continue; }
            if (w.inventory == null) { Debug.LogError ( "Warehouse inventory is null" ); continue; }

            if (w.inventory.CheckCanHold ( resourceID, resourceQuantity ))
            {
                eligibleWarehouses.Add ( w );
            }
            else
            {
                Debug.LogError ( "Warehouse inventory can't hold that many resources" ); continue;
            }
        }

        // TODO: Maybe add some sort of tick system so this doesnt clog up the players client
        if (eligibleWarehouses.Count <= 0) { Debug.LogError ( "No Eligible Warehouses. We shouldnt run this every frame" ); return null; }

        int fastestPath = -1;
        float bestDistance = float.MaxValue;

        for (int i = 0; i < eligibleWarehouses.Count; i++)
        {
            NavMeshPath path = new NavMeshPath ();

            if (base.cBase == null) continue;
            if (eligibleWarehouses[i] == null) continue;

            NavMesh.CalculatePath ( base.cBase.transform.position, eligibleWarehouses[i].transform.position, 0, path );

            float f = GetPathLength ( path );
            if (f <= bestDistance)
            {
                bestDistance = f;
                fastestPath = i;
            }
        }

        if (fastestPath == -1) { Debug.LogError ( "No Eligible Path. We also shouldnt run this every frame." ); return null; }

        return eligibleWarehouses[fastestPath];
    }
}
