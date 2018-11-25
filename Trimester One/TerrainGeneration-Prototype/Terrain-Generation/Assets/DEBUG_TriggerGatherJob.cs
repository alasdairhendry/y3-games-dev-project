using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DEBUG_TriggerGatherJob : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown ( 0 ) && !Input.GetKey ( KeyCode.LeftControl ))
        {
            if (EventSystem.current.IsPointerOverGameObject ()) return;
            Ray ray = Camera.main.ScreenPointToRay ( Input.mousePosition );
            RaycastHit hit;

            if (Physics.Raycast ( ray, out hit ))
            {
                if (hit.collider.gameObject.GetComponentInParent<RawMaterial> () != null)
                {
                    hit.collider.gameObject.GetComponentInParent<RawMaterial> ().CreateRemovalJob ();
                }
                else if (hit.collider.gameObject.GetComponentInParent<Buildable> () != null)
                {
                    hit.collider.gameObject.GetComponentInParent<Buildable> ().DestroyBuildable ();
                }
                else if (hit.collider.gameObject.GetComponentInParent<Character> () != null)
                {
                    FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ().ShowPanel ( hit.collider.gameObject.GetComponentInParent<Character> () );
                }
            }
        }
	}
}
