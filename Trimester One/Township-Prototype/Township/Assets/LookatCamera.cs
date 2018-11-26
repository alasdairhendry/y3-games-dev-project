using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookatCamera : MonoBehaviour {

    Camera camera;

	// Use this for initialization
	void Start () {
        camera = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 direction = transform.position - camera.transform.position;
        Quaternion rotation = Quaternion.LookRotation ( direction );
        transform.rotation = Quaternion.Slerp ( transform.rotation, rotation, Time.deltaTime * 5.0f);
	}
}
