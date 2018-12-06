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

    public virtual void Gather ()
    {
        Destroy ( this.gameObject );
    }

    public virtual void Remove ()
    {
        GetComponent<JobEntity> ().DestroyJobs ();

        FindObjectOfType<SnowController> ().SetObjectMaterial ( GetComponentsInChildren<MeshRenderer> (), true );

        Destroy ( this.gameObject );
    }

}
