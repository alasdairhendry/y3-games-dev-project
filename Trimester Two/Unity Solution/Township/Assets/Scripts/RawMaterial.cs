using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RawMaterial : MonoBehaviour
{
    [SerializeField] private int resourceProvided;
    [SerializeField] private float quantityProvided;
    [SerializeField] private float timeToRemove = 10.0f;
    [SerializeField] private string removalDescription = "Cut Down";

    //[SerializeField] private bool removableByBuilding = true;
    //public bool RemovableByBuilding { get { return removableByBuilding; } }

    [ContextMenu ( "CreateRemovalJob" )]
    public void CreateRemovalJob ()
    {
        if (GetComponent<JobEntity> ().HasNonNullJob ()) return;
        GetComponent<JobEntity> ().CreateJob_GatherResource ( "Gather Resource", true, timeToRemove, null, resourceProvided, quantityProvided, this );
    }

    protected virtual void Start ()
    {
        SnowController.Instance.SetObjectMaterial ( GetComponentsInChildren<MeshRenderer> (), false );
        SetInspectable ();
    }

    public void SetValues (int rID, float rQuantity, float removeTime, string description)
    {
        this.resourceProvided = rID;
        this.quantityProvided = rQuantity;
        this.timeToRemove = removeTime;
        this.removalDescription = description;
    }

    public virtual void OnGathered ()
    {
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

    public virtual void RemoveOnBuildingPlaced ()
    {
        GetComponent<JobEntity> ().DestroyJobs ();
        OnGathered ();
    }

    private void SetInspectable ()
    {
        GetComponent<Inspectable> ().SetAdditiveAction ( () =>
        {
            HUD_EntityInspection_Citizen_Panel panel = FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ();

            panel.AddButtonData ( () =>
            {
                if (this == null) return;

                this.CreateRemovalJob ();
                panel.Hide ();

            }, removalDescription, "Overview" );

            panel.AddButtonData ( () =>
            {
                if (this == null) return;

                this.RemoveOnBuildingPlaced ();
                panel.Hide ();

            }, "Remove", "Overview" );

        } );
    }
}
