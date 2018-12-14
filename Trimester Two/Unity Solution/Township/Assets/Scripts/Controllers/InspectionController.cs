using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InspectionController : MonoBehaviour {

    [SerializeField] private Material highlightedMaterial;
    [SerializeField] private LayerMask layerMask;
    private PropVisualiser highlightedProp;

    private Vector3 previousMousePosition;
    private bool mouseHasMoved = false;

    private void Awake ()
    {
        previousMousePosition = Input.mousePosition;       
    }

    private void Update ()
    {
        if (previousMousePosition != Input.mousePosition) mouseHasMoved = true;
        else mouseHasMoved = false;

        CheckInput ();

        previousMousePosition = Input.mousePosition;
    }

    private void CheckInput ()
    {
        if (BuildMode.Instance.isActive) { Unhighlight (); return; }

        if (Input.GetMouseButtonDown ( 0 ) && !Input.GetKey ( KeyCode.LeftControl ))
        {
            if (EventSystem.current.IsPointerOverGameObject ()) return;
            FindInspectables ();
        }

        if (VisualiserController.Instance.IsOn) return;      

        if (!Input.GetKey ( KeyCode.LeftControl ) && mouseHasMoved)
        {
            if (EventSystem.current.IsPointerOverGameObject ()) { Unhighlight (); return; }
            HighlightInspectables ();
        }
        else
        {
            if (EventSystem.current.IsPointerOverGameObject ()) { Unhighlight (); return; }
        }
    }

    private void HighlightInspectables ()
    {
        Ray ray = Camera.main.ScreenPointToRay ( Input.mousePosition );
        RaycastHit hit;

        if (Physics.Raycast ( ray, out hit, 1000, layerMask ))
        {
            Inspectable inspectable = hit.collider.gameObject.GetComponentInParent<Inspectable> ();
            if (inspectable == null) { Unhighlight (); return; }

            PropVisualiser propVisualiser = hit.collider.gameObject.GetComponentInParent<PropVisualiser> ();
            if (propVisualiser == null) { Unhighlight (); return; }

            if (highlightedProp != propVisualiser) Unhighlight ();

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

        if (Physics.Raycast ( ray, out hit, 1000, layerMask ))
        {
            if (hit.collider.gameObject.GetComponentInParent<Inspectable> () != null)
            {
                if (!hit.collider.gameObject.GetComponentInParent<Inspectable> ().enabled) return;

                hit.collider.gameObject.GetComponentInParent<Inspectable> ().Inspect ();
            }
        }
    }
}
