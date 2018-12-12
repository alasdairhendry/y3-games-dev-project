using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Prop : MonoBehaviour {

    [SerializeField] protected Vector3 citizenInteractionPoint;
    public Vector3 CitizenInteractionPointGlobal { get { return transform.Find ( "citizenInteractionPlaceholder" ).position; } }

    public PropData data { get; protected set; }
    protected Buildable buildable;
    private bool isBlueprint = true;
    public bool IsBluePrint { get { return isBlueprint; } }
    public System.Action<Prop> onDestroy;

    protected virtual void Awake () { isBlueprint = true; }

    protected virtual void Start () { }

    protected virtual void Update () { }

    protected virtual void SetInspectable ()
    {
        GetComponent<Inspectable> ().SetAdditiveAction ( () =>
        {
            HUD_EntityInspection_Citizen_Panel panel = FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ();

            panel.AddButtonData ( () =>
            {
                if (this == null) return;
                if (this.gameObject == null) return;
                DestroyProp ();

            }, "Destroy", "Any" );
        } );
    }

	public virtual void Place (PropData data)
    {
        this.data = data;
        isBlueprint = false;
        buildable = GetComponent<Buildable> ().Begin ();
        GetComponent<NavMeshObstacle> ().enabled = true;

        GameObject citizenInteractionPlaceholder = new GameObject ( "citizenInteractionPlaceholder" );
        citizenInteractionPlaceholder.transform.SetParent ( this.transform );
        citizenInteractionPlaceholder.transform.localRotation = Quaternion.identity;
        citizenInteractionPlaceholder.transform.localPosition = citizenInteractionPoint;

        GetComponent<Inspectable> ().enabled = true;

        if(citizenInteractionPoint == null || citizenInteractionPoint == Vector3.zero)
        {
            Debug.LogError ( "You may have forgot to assign the interaction point in the inspector!", this );
            citizenInteractionPoint = Vector3.zero;
        }

        FindObjectOfType<SnowController> ().SetObjectMaterial ( GetComponentsInChildren<MeshRenderer> ( true ), false );
    }

    public virtual void DestroyProp ()
    {
        if (onDestroy != null) onDestroy ( this );

        if (this == null) return;
        if (this.gameObject == null) return;

        PropManager.Instance.OnPropDestroyed ( this.gameObject );
        GetComponent<JobEntity> ().DestroyJobs ();
        FindObjectOfType<SnowController> ().SetObjectMaterial ( GetComponentsInChildren<MeshRenderer> ( true ), true );
        Destroy ( this.gameObject );
    }

    private void OnDestroy ()
    {
           
    }

    private void OnDrawGizmosSelected ()
    {
        if (citizenInteractionPoint == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere ( transform.position + citizenInteractionPoint, 0.5f );
    }
}
