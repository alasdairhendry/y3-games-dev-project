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
    private bool isLocked = false;
    private Transform lockTarget;

    private void Start()
    {        
        targetPosition = transform.position;            
    }

    private void Update () {
        Pan();
        Zoom();

        if (isLocked && lockTarget != null)
        {
            FollowTarget ();
        }
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
            isLocked = false;
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

        targetPosition = targetZoomPosition;
    }

    public void PanTo(Vector3 position)
    {
        targetPosition = position;
        Zoom ();
    }

    private void FollowTarget ()
    {
        Vector3 targetZoomPosition = lockTarget.position;
        targetZoomPosition += transform.forward * Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * currentZoomDistance * Time.deltaTime;
        targetPosition = targetZoomPosition;
        Zoom ();
    }

    public void LockTo(Transform target)
    {
        lockTarget = target;
        isLocked = true;
    }
}
