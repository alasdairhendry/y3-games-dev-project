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

    [SerializeField] private float currentZoomDistance;

    [SerializeField] private Vector3 targetPosition;

    private void Start()
    {        
        targetPosition = transform.position;            
    }

    private void Update () {
        Pan();
        Zoom();
        NavMeshControl();
	}

    private void NavMeshControl()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit;
        //    if(Physics.Raycast(ray, out hit))
        //    {
        //        target.SetDestination(hit.point);
        //        seeker.transform.position = hit.point;
        //    }
        //}
    }

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * panDamp);
    }

    private void Pan()
    {
        if (Input.GetMouseButton(2))
        {
            targetPosition += Vector3.right * Time.deltaTime * panSpeed * (Input.mousePosition.x - Screen.width * 0.5f) / (Screen.width * 0.5f) * currentZoomDistance;
            targetPosition += Vector3.forward * Time.deltaTime * panSpeed * (Input.mousePosition.y - Screen.height * 0.5f) / (Screen.height * 0.5f) * currentZoomDistance;   
        }       
    }

    private void Zoom()
    {
        Vector3 targetZoomPosition = targetPosition;

        targetZoomPosition += transform.forward * Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime;        

        RaycastHit hit;
        if(Physics.Raycast(targetZoomPosition, transform.forward, out hit))
        {
            currentZoomDistance = Vector3.Distance(targetZoomPosition, hit.point);

            if (currentZoomDistance <= zoomNearClamp)
                targetZoomPosition = hit.point + (-transform.forward * zoomNearClamp);

            if (currentZoomDistance >= zoomFarClamp)
                targetZoomPosition = hit.point + (-transform.forward * zoomFarClamp);
        }

        targetPosition = targetZoomPosition;
    }
}
