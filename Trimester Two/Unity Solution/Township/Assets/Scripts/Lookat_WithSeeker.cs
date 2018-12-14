using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lookat_WithSeeker : MonoBehaviour {

    [SerializeField] private Transform seeker;
    [SerializeField] private Transform target;

    [SerializeField] private bool constrainX;
    [SerializeField] private bool constrainY;
    [SerializeField] private bool constrainZ;

    private Vector3 direction = new Vector3 ();

    private void LateUpdate ()
    {
        direction = target.transform.position - seeker.transform.position;

        if (constrainX) direction.x = 0;
        if (constrainY) direction.y = 0;
        if (constrainZ) direction.z = 0;

        direction.Normalize ();

        transform.rotation = Quaternion.LookRotation ( direction );
	}
}
