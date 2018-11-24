using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEBUG_TriggerGatherJob : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0) && !Input.GetKey ( KeyCode.LeftControl ))
        {            
            Ray ray = Camera.main.ScreenPointToRay ( Input.mousePosition );
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit ))
            {         
                if (hit.collider.gameObject.GetComponentInParent<RawMaterial> () != null)
                {             
                    hit.collider.gameObject.GetComponentInParent<RawMaterial> ().CreateRemovalJob ();
                }
            }
        }
	}
}
