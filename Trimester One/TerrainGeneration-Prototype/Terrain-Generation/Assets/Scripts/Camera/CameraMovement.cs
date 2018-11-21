using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CameraMovement : MonoBehaviour {

    [SerializeField] private float panSpeed;
    [SerializeField] private float panDamp;

    [SerializeField] private float zoomSpeed;
    [SerializeField] private float zoomNearClamp;
    [SerializeField] private float zoomFarClamp;

    [SerializeField] private Vector2 xClamp;
    [SerializeField] private Vector2 zClamp;

    [SerializeField] private float currentZoomDistance;

    [SerializeField] private Vector3 targetPosition;

    private void Start()
    {        
        targetPosition = transform.position;            
    }

    private void Update () {
        Pan();
        Zoom();
	}

    private void LateUpdate()
    {
        targetPosition.x = Mathf.Clamp ( targetPosition.x, xClamp.x, xClamp.y );
        targetPosition.z = Mathf.Clamp ( targetPosition.z, zClamp.x, zClamp.y );
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * panDamp);
    }

    private void Pan()
    {
        if (Input.GetMouseButton(2) || Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftControl))
        {
            targetPosition += Vector3.right * Time.deltaTime * panSpeed * (Input.mousePosition.x - Screen.width * 0.5f) / (Screen.width * 0.5f) * currentZoomDistance;
            targetPosition += Vector3.forward * Time.deltaTime * panSpeed * (Input.mousePosition.y - Screen.height * 0.5f) / (Screen.height * 0.5f) * currentZoomDistance;   
        }
    }

    private void Zoom()
    {
        Vector3 targetZoomPosition = targetPosition;

        RaycastHit hit;
        if(Physics.Raycast(targetZoomPosition, transform.forward, out hit))
        {
            currentZoomDistance = Vector3.Distance(targetZoomPosition, hit.point);            

            if (currentZoomDistance <= zoomNearClamp)
                targetZoomPosition = hit.point + (-transform.forward * zoomNearClamp);

            if (currentZoomDistance >= zoomFarClamp)
                targetZoomPosition = hit.point + (-transform.forward * zoomFarClamp);
        }

        targetZoomPosition += transform.forward * Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * currentZoomDistance * Time.deltaTime;

        //RaycastHit hit2;
        //if(Physics.Raycast(targetZoomPosition, -transform.forward, out hit2))
        //{
        //    currentZoomDistance = -Vector3.Distance(targetZoomPosition, hit2.point);

        //    Debug.Log("Camera is past terrain: Distance - (" + currentZoomDistance + ")");

        //    if (currentZoomDistance <= zoomNearClamp)
        //        targetZoomPosition = hit2.point + (-transform.forward * zoomNearClamp);

        //    if (currentZoomDistance >= zoomFarClamp)
        //        targetZoomPosition = hit2.point + (-transform.forward * zoomFarClamp);
        //}

        //Debug.DrawRay(targetZoomPosition, transform.forward * 250.0f, Color.red, 0.25f);
        //Debug.DrawRay(targetZoomPosition, -transform.forward * 250.0f, Color.blue, 0.25f);

        targetPosition = targetZoomPosition;
    }

    public void PanTo(Vector3 position)
    {
        targetPosition = position;
        Zoom ();
    }
}
