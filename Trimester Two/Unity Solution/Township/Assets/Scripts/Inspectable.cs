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

    [SerializeField] protected string defaultTab = "Overview";

    protected virtual void Start () { }

    public void InspectAndLockCamera ()
    {
        FindObjectOfType<CameraMovement> ().LockTo ( this.transform, lockOffset, lockDistance );
        Inspect ();
    }

	public virtual void Inspect ()
    {
        if (action == null) return;
        FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ().Hide ();
        FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ().ShowPanel ( this.gameObject );
        FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ().SetAction ( action, destroyAction, focusAction, destroyDescription );        
        FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ().SetActiveTab ( defaultTab );
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
