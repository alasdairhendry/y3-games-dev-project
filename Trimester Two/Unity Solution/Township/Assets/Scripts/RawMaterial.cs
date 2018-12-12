using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RawMaterial : MonoBehaviour
{
    [SerializeField] private int resourceProvided;
    [SerializeField] private float quantityProvided;    

    [ContextMenu ( "CreateRemovalJob" )]
    public void CreateRemovalJob ()
    {
        if (GetComponent<JobEntity> ().HasNonNullJob ()) return;
        GetComponent<JobEntity> ().CreateJob_GatherResource ( "Gather Resource", true, resourceProvided, quantityProvided, 10.0f, this );
    }

    private void Start ()
    {        
        FindObjectOfType<SnowController> ().SetObjectMaterial ( GetComponentsInChildren<MeshRenderer> (), false );
    }

    public virtual void OnGathered ()
    {
        GetComponent<Wobblable> ().Break (DestroyOnGathered);
        FindObjectOfType<DEBUG_DrawSnowDepressionsWithMouse> ().DrawDepression ( 1000, 3, transform.position );
        FindObjectOfType<SnowController> ().SetObjectMaterial ( GetComponentsInChildren<MeshRenderer> (), true );              
    }

    protected virtual void DestroyOnGathered ()
    {
        Destroy ( this.gameObject );
    }

    public virtual void RemoveOnBuildingPlaced ()
    {
        OnGathered ();
        //FindObjectOfType<DEBUG_DrawSnowDepressionsWithMouse> ().DrawDepression ( 1000, 3, transform.position );
        //GetComponent<JobEntity> ().DestroyJobs ();

        //FindObjectOfType<SnowController> ().SetObjectMaterial ( GetComponentsInChildren<MeshRenderer> (), true );


        //Destroy ( this.gameObject );
    }

}
