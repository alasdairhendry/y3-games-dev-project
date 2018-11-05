using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SpatialCollider : MonoBehaviour {

    public bool IsColliding { get { if (colliders.Count > 0) return true; else return false; } }

    private List<SpatialCollider> colliders = new List<SpatialCollider> ();

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
    }

    private void OnTriggerExit (Collider other)
    {
        if (other.gameObject.GetComponent<SpatialCollider> () != null)
            other.gameObject.GetComponent<SpatialCollider> ().RemoveCollision ( this );
    }

    private void OnDestroy ()
    {
        for (int i = 0; i < colliders.Count; i++)
        {
            colliders[i].RemoveCollision ( this );
        }
    }

    public void AddCollision(SpatialCollider from)
    {
        colliders.Add ( from );
    }

    public void RemoveCollision(SpatialCollider from)
    {
        colliders.Remove ( from );
    }
}
