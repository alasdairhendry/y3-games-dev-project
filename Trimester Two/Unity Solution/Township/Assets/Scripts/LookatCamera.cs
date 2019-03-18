using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookatCamera : MonoBehaviour {

    Camera camera;

    Vector3 direction = new Vector3 ();
    Quaternion rotation = new Quaternion ();

    // Use this for initialization
    void Start () {
        camera = Camera.main;
        rotation = transform.rotation;

        direction = transform.position - camera.transform.position;
        rotation = Quaternion.LookRotation ( direction );       
    }

    // Update is called once per frame
    void Update ()
    {
        if (CameraMovement.CameraMoved)
        {
            direction = transform.position - camera.transform.position;
            rotation = Quaternion.LookRotation ( direction );
        }

        if (transform.rotation != rotation)
            transform.rotation = Quaternion.Slerp ( transform.rotation, rotation, Time.deltaTime * 5.0f );
    }
}
