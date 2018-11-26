using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SpatialCollider : MonoBehaviour {

    public bool IsColliding { get { if (spatialColliders.Count > 0) return true; else return false; } }

    private List<SpatialCollider> spatialColliders = new List<SpatialCollider> ();
    private List<RawMaterialCollider> environmentCollider = new List<RawMaterialCollider> ();
    public List<RawMaterialCollider> EnvironmentColliders { get { return environmentCollider; } }

    private void Awake ()
    {
        if (!GetComponent<Collider> ())
        {
            gameObject.AddComponent<BoxCollider> ();
        }

        GetComponent<Collider> ().isTrigger = true;
    }

    private void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.GetComponent<SpatialCollider> () != null)
            other.gameObject.GetComponent<SpatialCollider> ().AddCollision ( this );

        if (other.gameObject.GetComponent<RawMaterialCollider> () != null)
        {
            environmentCollider.Add ( other.gameObject.GetComponent<RawMaterialCollider> () );
            other.gameObject.GetComponentInParent<PropVisualiser> ().Visualise ( Resources.Load<Material> ( "PropOutline_Red_material" ) );
        }
    }

    private void OnTriggerExit (Collider other)
    {
        if (other.gameObject.GetComponent<SpatialCollider> () != null)
            other.gameObject.GetComponent<SpatialCollider> ().RemoveCollision ( this );

        if (other.gameObject.GetComponent<RawMaterialCollider> () != null)
        {
            environmentCollider.Remove ( other.gameObject.GetComponent<RawMaterialCollider> () );
            other.gameObject.GetComponentInParent<PropVisualiser> ().TurnOff ();
        }
    }

    private void OnDestroy ()
    {
        for (int i = 0; i < environmentCollider.Count; i++)
        {
            if (environmentCollider[i] == null) continue;
            environmentCollider[i].gameObject.GetComponentInParent<PropVisualiser> ().TurnOff ();
        }

        for (int i = 0; i < spatialColliders.Count; i++)
        {
            spatialColliders[i].RemoveCollision ( this );
        }
    }

    public void AddCollision(SpatialCollider from)
    {
        spatialColliders.Add ( from );
    }

    public void RemoveCollision(SpatialCollider from)
    {
        spatialColliders.Remove ( from );
    }
}
