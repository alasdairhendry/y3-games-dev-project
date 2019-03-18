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
        BreakForEnergy = false;
    }

    public override void DoJob ()
    {
        base.DoJob ();

        switch (stage)
        {
            case Stage.TravelTo:
                DoJob_Stage_TravelTo ();
                break;

            case Stage.Remove:
                DoJob_Stage_Remove ();
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
            OnCharacterLeave ( "Citizen can't hold that many resources", true, Job.GetCompletableParams ( Job.CompleteIdentifier.None ) );
        }
    }

    public override void OnCharacterLeave (string reason, bool setOpenToTrue, KeyValuePair<bool, string> isCompletable)
    {
        Debug.Log ( reason );
        this.cBase.GetComponent<CitizenGraphics> ().SetUsingAxe ( false, CitizenAnimation.AxeUseAnimation.Chopping );
        ResourceManager.Instance.GetResourceByID ( resourceID ).HoldItem ( this.cBase.GetComponent<CitizenGraphics> (), false );
        //this.cBase.GetComponent<CitizenGraphics> ().SetUsingCart ( false );

        if (givenResourceToCitizen)
        {
            if (base.cBase != null)
                base.cBase.Inventory.RemoveItemQuantity ( resourceID, resourceQuantity, cBase.transform, 2.0f );

            givenResourceToCitizen = false;
            Resource.DropResource ( resourceID, resourceQuantity, base.cBase.transform.position + (base.cBase.transform.forward), base.cBase.transform.eulerAngles );
        }

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


        base.OnCharacterLeave ( reason, setOpenToTrue , isCompletable);
    }

    private void DoJob_Stage_TravelTo ()
    {
        if(!destinationProvided)
        {
            targetPosition = rawMaterial.transform.Find ( "InteractionPoint" ).position;
            SetDestination ( rawMaterial.gameObject );
            destinationProvided = true;              
            return;
        }

        if (!citizenReachedPath) return;

        destinationProvided = false;

        stage = Stage.Remove;
    }

    private void DoJob_Stage_Remove ()
    {
        this.cBase.GetComponent<CitizenGraphics> ().SetUsingAxe ( true , CitizenAnimation.AxeUseAnimation.Chopping);

        if (this.rawMaterial == null)
        {
            OnCharacterLeave ( "Material Was Destroyed", false, Job.GetCompletableParams ( Job.CompleteIdentifier.ResourceDestroyed ) );
            return;
        }
        if (this.rawMaterial.gameObject == null)
        {
            OnCharacterLeave ( "Material Was Destroyed", false, Job.GetCompletableParams ( Job.CompleteIdentifier.ResourceDestroyed ) );
            return;
        }

        Quaternion lookRot = Quaternion.LookRotation ( this.rawMaterial.transform.position - this.cBase.transform.position, Vector3.up );
        this.cBase.transform.rotation = Quaternion.Slerp ( this.cBase.transform.rotation, lookRot, GameTime.DeltaGameTime * 2.5f );

        currentTime += GameTime.DeltaGameTime;

        if(currentTime>= base.TimeRequired)
        {            
            stage = Stage.FindWarehouse;
            this.cBase.GetComponent<CitizenGraphics> ().SetUsingAxe ( false, CitizenAnimation.AxeUseAnimation.Chopping );
            base.cBase.Inventory.AddItemQuantity ( resourceID, resourceQuantity, cBase.transform, 2.0f );
            rawMaterial.OnGathered ();
            givenResourceToCitizen = true;
        }
    }

    private void DoJob_Stage_FindWarehouse ()
    {
        Debug.Log ( "DoJob_Stage_FindWarehouse" );
        targetWarehouse = WarehouseController.Instance.FindWarehouseToStore ( base.cBase.transform, resourceID, resourceQuantity );
        //targetWarehouse = FindEligibleWarehouse ();

        if (targetWarehouse == null)
        {
          Debug.Log ( "Null warehouse, goodbye" );
            //if (givenResourceToCitizen)
            //    character.Inventory.RemoveItemQuantity ( resourceID, resourceQuantity );

            if (givenResourceToCitizen)
            {
                givenResourceToCitizen = false;
                base.cBase.Inventory.RemoveItemQuantity ( resourceID, resourceQuantity, base.cBase.transform, 2.0f );
                Resource.DropResource ( resourceID, resourceQuantity, base.cBase.transform.position + (base.cBase.transform.forward), base.cBase.transform.eulerAngles );
            }

            OnComplete ();
        }
        else
        {
            stage = Stage.TravelFrom;
            ResourceManager.Instance.GetResourceByID ( resourceID ).HoldItem ( this.cBase.GetComponent<CitizenGraphics> (), true );
            //this.cBase.GetComponent<CitizenGraphics> ().SetUsingLogs ( true );
        }
    }

    private void DoJob_Stage_TravelFrom ()
    {
        if (!destinationProvided)
        {
            targetPosition = targetWarehouse.CitizenInteractionPointGlobal;
            SetDestination ( targetWarehouse.gameObject );
            destinationProvided = true;
            return;
        }

        if (targetWarehouse == null)
        {
            destinationProvided = false;
            stage = Stage.FindWarehouse;            
            //OnCharacterLeave ( "Warehouse was destroyed", true, Job.GetCompletableParams ( Job.CompleteIdentifier.None ) );
            return;
        }

        if (!citizenReachedPath) return;

        ResourceManager.Instance.GetResourceByID ( resourceID ).HoldItem ( this.cBase.GetComponent<CitizenGraphics> (), false );

        WarehouseController.Instance.Inventory.AddItemQuantity ( resourceID, resourceQuantity, targetWarehouse.transform, targetWarehouse.data.UIOffsetY );

        if (givenResourceToCitizen)
            cBase.Inventory.RemoveItemQuantity ( resourceID, resourceQuantity, cBase.transform, 2.0f );

        givenResourceToCitizen = false;
        destinationProvided = false;

        OnComplete ();
    }

    protected override void OnComplete ()
    {
        this.cBase.GetComponent<CitizenGraphics> ().SetUsingAxe ( false, CitizenAnimation.AxeUseAnimation.Chopping );
        ResourceManager.Instance.GetResourceByID ( resourceID ).HoldItem ( this.cBase.GetComponent<CitizenGraphics> (), false );

        base.OnComplete ();
    }

    //private Prop_Warehouse FindEligibleWarehouse ()
    //{
    //    List<GameObject> GOs = EntityManager.Instance.GetEntitiesByType ( typeof ( Prop_Warehouse ) );

    //    if (GOs == null) { Debug.LogError ( "No Warehouses found. We shouldnt run this every frame" ); return null; }

    //    List<Prop_Warehouse> eligibleWarehouses = new List<Prop_Warehouse> ();

    //    for (int i = 0; i < GOs.Count; i++)
    //    {
    //        if (GOs[i] == null) continue;
    //        Prop_Warehouse w = GOs[i].GetComponent<Prop_Warehouse> ();

    //        if (w == null) { continue; }
    //        if (w.inventory == null) { Debug.LogError ( "Warehouse inventory is null" ); continue; }
    //        if (w.buildable.IsComplete == false) { continue; }

    //        if (w.inventory.CheckCanHold ( resourceID, resourceQuantity ))
    //        {
    //            eligibleWarehouses.Add ( w );
    //        }
    //        else
    //        {
    //            Debug.LogError ( "Warehouse inventory can't hold that many resources" ); continue;
    //        }
    //    }

    //    // TODO: Maybe add some sort of tick system so this doesnt clog up the players client
    //    if (eligibleWarehouses.Count <= 0) { Debug.LogError ( "No Eligible Warehouses. We shouldnt run this every frame" ); return null; }

    //    int fastestPath = -1;
    //    float bestDistance = float.MaxValue;

    //    for (int i = 0; i < eligibleWarehouses.Count; i++)
    //    {
    //        NavMeshPath path = new NavMeshPath ();

    //        if (base.cBase == null) continue;
    //        if (eligibleWarehouses[i] == null) continue;

    //        NavMesh.CalculatePath ( base.cBase.transform.position, eligibleWarehouses[i].transform.position, 0, path );

    //        float f = GetPathLength ( path );
    //        if (f <= bestDistance)
    //        {
    //            bestDistance = f;
    //            fastestPath = i;
    //        }
    //    }

    //    if (fastestPath == -1) { Debug.LogError ( "No Eligible Path. We also shouldnt run this every frame." ); return null; }

    //    return eligibleWarehouses[fastestPath];
    //}
}
