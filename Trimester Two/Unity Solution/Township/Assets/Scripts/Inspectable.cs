using UnityEditor;
using UnityEngine;

public class Inspectable : MonoBehaviour {

    protected System.Action action;
    protected System.Action destroyAction;
    protected System.Action focusAction;
    protected string destroyDescription;

    [SerializeField] private Vector3 lockOffset = Vector3.zero;
    [SerializeField] private float lockDistance = 3.0f;

    public Vector3 LockOffset { get { return lockOffset; } }
    public float LockDistance { get { return lockDistance; } }

    private Inspectable overrideLockTo = null;
    public bool isInspected { get; protected set; } = false;

    [SerializeField] protected string defaultTab = "Overview";

    protected virtual void Start () { }

    public void InspectAndLockCamera ()
    {
        if (overrideLockTo == null)
        {
            FindObjectOfType<CameraMovement> ().LockTo ( this.transform, lockOffset, lockDistance );
        }
        else
        {
            FindObjectOfType<CameraMovement> ().LockTo ( overrideLockTo.transform, overrideLockTo.LockOffset, overrideLockTo.LockDistance );
        }
        Inspect ();
    }

    public void OverrideLockTo(Inspectable ins)
    {
        overrideLockTo = ins;
    }

	public virtual void Inspect ()
    {
        if (action == null) { Debug.Log ( "Inspectable action null" ); return; }
        FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ().Hide ();
        isInspected = true;
        FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ().ShowPanel ( this.gameObject );
        FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ().SetAction ( action, destroyAction, focusAction, destroyDescription );        
        FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ().AddOnCloseAction ( () => { isInspected = false; } );        
        FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ().SetActiveTab ( defaultTab );
    }

    public virtual void Refresh ()
    {
        //if (isInspected)
        //{
        //    string currTab = FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ().currentTabName;
        //    FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ().Hide ();
        //    isInspected = true;
        //    FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ().ShowPanel ( this.gameObject );
        //    //FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ().SetAction ( action, destroyAction, focusAction, destroyDescription );
        //    FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ().AddOnCloseAction ( () => { isInspected = false; } );
        //    FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ().SetActiveTab ( currTab );
        //}
    }

    public void SetAction (System.Action action)
    {
        if (action != null) this.action = action;
    }

    public void SetAdditiveAction (System.Action action)
    {
        if (action != null) this.action += action;
    }

    public void SetDestroyAction(System.Action action, bool additive, string destroyDescription)
    {
        if (!string.IsNullOrEmpty ( destroyDescription )) this.destroyDescription = destroyDescription;

        if (action == null) return;

        if (additive)
            destroyAction += action;
        else destroyAction = action;
    }

    public void SetFocusAction(System.Action action, bool additive)
    {
        if (action == null) return;

        if (additive)
            focusAction += action;
        else focusAction = action;
    }

    private void OnDrawGizmosSelected ()
    {
        Gizmos.color = new Color ( 1, 0, 0, 0.9f );
        Gizmos.DrawCube ( transform.TransformPoint ( lockOffset ), Vector3.one );
    }
}
