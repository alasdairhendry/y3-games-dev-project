using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InspectionController : MonoBehaviour {

    private PropVisualiser highlightedProp;
    [SerializeField] private Material highlightedMaterial;

    private void Update ()
    {
        CheckInput ();
    }

    private void CheckInput ()
    {
        if (FindObjectOfType<BuildMode> ().isActive) { Unhighlight (); return; }

        if (Input.GetMouseButtonDown ( 0 ) && !Input.GetKey ( KeyCode.LeftControl ))
        {
            if (EventSystem.current.IsPointerOverGameObject ()) return;
            FindInspectables ();
        }

        if (VisualiserController.Instance.IsOn) return;      

        if (!Input.GetKey ( KeyCode.LeftControl ))
        {
            if (EventSystem.current.IsPointerOverGameObject ()) { Unhighlight (); return; }
            HighlightInspectables ();
        }
        else
        {
            Unhighlight ();
        }
    }

    private void HighlightInspectables ()
    {
        Ray ray = Camera.main.ScreenPointToRay ( Input.mousePosition );
        RaycastHit hit;

        if (Physics.Raycast ( ray, out hit ))
        {
            Inspectable inspectable = hit.collider.gameObject.GetComponentInParent<Inspectable> ();
            if (inspectable == null) { Unhighlight (); return; }

            PropVisualiser propVisualiser = hit.collider.gameObject.GetComponentInParent<PropVisualiser> ();
            if (propVisualiser == null) { Unhighlight (); return; }

            if (highlightedProp != propVisualiser) Unhighlight ();

            //Unhighlight ();

            highlightedProp = propVisualiser;
            highlightedProp.Visualise ( highlightedMaterial );            
        }
        else
        {
            Unhighlight ();
        }
    }

    private void Unhighlight ()
    {
        if (highlightedProp != null)
        {
            highlightedProp.TurnOff ();
            highlightedProp = null;
        }
    }

    private void FindInspectables ()
    {
        Ray ray = Camera.main.ScreenPointToRay ( Input.mousePosition );
        RaycastHit hit;

        if (Physics.Raycast ( ray, out hit ))
        {
            if (hit.collider.gameObject.GetComponentInParent<Inspectable> () != null)
            {
                if (!hit.collider.gameObject.GetComponentInParent<Inspectable> ().enabled) return;

                hit.collider.gameObject.GetComponentInParent<Inspectable> ().Inspect ();
            }
        }
    }
}
