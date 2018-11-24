using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Prop : MonoBehaviour {

    [SerializeField] protected Vector3 citizenInteractionPoint;
    public Vector3 CitizenInteractionPointGlobal { get { return transform.Find ( "citizenInteractionPlaceholder" ).position; } }

    public PropData data { get; protected set; }
    protected Buildable buildable;

	public virtual void Place (PropData data)
    {
        this.data = data;
        buildable = GetComponent<Buildable> ().Begin ();
        GetComponent<NavMeshObstacle> ().enabled = true;

        GameObject citizenInteractionPlaceholder = new GameObject ( "citizenInteractionPlaceholder" );
        citizenInteractionPlaceholder.transform.SetParent ( this.transform );
        citizenInteractionPlaceholder.transform.localRotation = Quaternion.identity;
        citizenInteractionPlaceholder.transform.localPosition = citizenInteractionPoint;

        //citizenInteractionPlaceholder.transform.position = transform.forward * (transform.position + citizenInteractionPoint);

        if(citizenInteractionPoint == null || citizenInteractionPoint == Vector3.zero)
        {
            Debug.LogError ( "You may have forgot to assign the interaction point in the inspector!", this );
            citizenInteractionPoint = Vector3.zero;
        }
    }

    private void OnDrawGizmosSelected ()
    {
        if (citizenInteractionPoint == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere ( transform.position + citizenInteractionPoint, 0.5f );
    }
}
