using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RawMaterial : MonoBehaviour
{
    [SerializeField] private int resourceProvided;
    [SerializeField] private float quantityProvided;
    [SerializeField] private float timeToRemove = 10.0f;
    [SerializeField] private string removalDescription = "Cut Down";
    public Inspectable Inspectable { get; protected set; }

    public WorldEntity worldEntity { get; protected set; }

    public System.Action<GameObject> onSetRemoval;

    private void Awake ()
    {
        Inspectable = GetComponent<Inspectable> ();
    }

    protected virtual void Start ()
    {
        SnowController.Instance.SetObjectMaterial ( GetComponentsInChildren<MeshRenderer> (), false );
        worldEntity = GetComponent<WorldEntity> ();
        SetInspectable ();
    }

    public void SetValues (int rID, float rQuantity, float removeTime, string description)
    {
        this.resourceProvided = rID;
        this.quantityProvided = rQuantity;
        this.timeToRemove = removeTime;
        this.removalDescription = description;

        if (worldEntity == null) worldEntity = GetComponent<WorldEntity> ();
        worldEntity?.Setup ( ResourceManager.Instance.GetResourceByID ( rID ).name, WorldEntity.EntityType.Resource );
    }

    public void CreateRemovalJob ()
    {
        if (GetComponent<JobEntity> ().HasNonNullJob ()) return;
        GetComponent<JobEntity> ().CreateJob_GatherResource ( removalDescription + " " + worldEntity.EntityName, true, timeToRemove, null, resourceProvided, quantityProvided, this );
    }

    public virtual void RemoveOnBuildingPlaced ()
    {
        GetComponent<JobEntity> ().DestroyJobs ();
        OnGathered ();
    }

    public virtual void OnGathered ()
    {
        Debug.Log ( "OnGathered" );
        onSetRemoval?.Invoke ( this.gameObject );
        if (GetComponent<Wobblable> () != null)
            GetComponent<Wobblable> ().Break ( DestroyOnGathered );
        else DestroyOnGathered ();

        SnowController.Instance.DrawDepression ( 1000, 3, transform.position );
        SnowController.Instance.SetObjectMaterial ( GetComponentsInChildren<MeshRenderer> (), true );
    }

    protected virtual void DestroyOnGathered ()
    {
        Destroy ( this.gameObject );
    }

    private void SetInspectable ()
    {
        Inspectable.SetDestroyAction ( () => { this.RemoveOnBuildingPlaced (); }, false, "Remove" );
        Inspectable.SetFocusAction ( () => { Inspectable.InspectAndLockCamera (); }, false );

        Inspectable.SetAdditiveAction ( () =>
        {
            HUD_EntityInspection_Citizen_Panel panel = FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ();

            panel.AddButtonData ( () =>
            {
                if (this == null) return;

                this.CreateRemovalJob ();
                panel.Hide ();

            }, removalDescription, "Overview" );

        } );
    }
}
