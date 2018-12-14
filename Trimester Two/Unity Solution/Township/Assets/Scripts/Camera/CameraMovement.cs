using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class CameraMovement : MonoBehaviour {

    [SerializeField] private float panSpeed;
    [SerializeField] private float panDamp;

    [SerializeField] private float rotateSpeed;
    [SerializeField] private float rotateDamp;
    private float currentRotation = 0;

    [SerializeField] private float zoomSpeed;
    [SerializeField] private float zoomNearClamp;
    [SerializeField] private float zoomFarClamp;

    [SerializeField] private Vector2 xClamp;
    [SerializeField] private Vector2 zClamp;

    [SerializeField] private float currentZoomDistance;

    [SerializeField] private Vector3 targetPosition;
    private bool isLocked = false;
    private Transform lockTarget;

    [SerializeField] private LayerMask layerMask;

    private void Start()
    {        
        targetPosition = transform.position;            
    }

    private void Update () {
        if (Input.GetKeyDown ( KeyCode.F ))
        {
            PanTo ( new Vector3 ( 0.0f, 16.0f, -24.5f ) );
            //PanTo ( new Vector3 ( 0.0f, 110.84f, -110.41f ) );
        }

        Pan();
        Zoom();
        Rotate ();

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
        //if (Input.GetMouseButton(2) || Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftControl))
        //{
        //    isLocked = false;
        //    targetPosition += transform.right * Time.deltaTime * panSpeed * (Input.mousePosition.x - Screen.width * 0.5f) / (Screen.width * 0.5f) * currentZoomDistance;
        //    targetPosition += (transform.forward - new Vector3(0.0f, transform.forward.y, 0.0f)) * Time.deltaTime * panSpeed * (Input.mousePosition.y - Screen.height * 0.5f) / (Screen.height * 0.5f) * currentZoomDistance;   
        //}

        if (Hotkey.GetKey(Hotkey.Function.CameraPan) ||( Input.GetKey(KeyCode.Mouse0) && Input.GetKey( KeyCode.LeftControl)))
        {
            isLocked = false;
            targetPosition += transform.right * Time.deltaTime * panSpeed * (Input.mousePosition.x - Screen.width * 0.5f) / (Screen.width * 0.5f) * currentZoomDistance;
            targetPosition += (transform.forward - new Vector3(0.0f, transform.forward.y, 0.0f)) * Time.deltaTime * panSpeed * (Input.mousePosition.y - Screen.height * 0.5f) / (Screen.height * 0.5f) * currentZoomDistance;   
        }
    }

    private void Zoom()
    {
        Vector3 targetZoomPosition = targetPosition;

        RaycastHit hit;
        if (Physics.Raycast ( targetZoomPosition, transform.forward, out hit, 10000, layerMask ))
        {
            currentZoomDistance = Vector3.Distance ( targetZoomPosition, hit.point );

            if (currentZoomDistance <= zoomNearClamp)
                targetZoomPosition = hit.point + (-transform.forward * zoomNearClamp);

            if (currentZoomDistance >= zoomFarClamp)
                targetZoomPosition = hit.point + (-transform.forward * zoomFarClamp);
        }

        if (!EventSystem.current.IsPointerOverGameObject ())
            targetZoomPosition += transform.forward * Input.GetAxis ( "Mouse ScrollWheel" ) * zoomSpeed * currentZoomDistance * Time.deltaTime;

        targetPosition = targetZoomPosition;
    }

    private void Rotate ()
    {
        if (Hotkey.GetKey ( Hotkey.Function.CameraRotateRight )) currentRotation -= rotateSpeed * Time.deltaTime;
        else if (Hotkey.GetKey(Hotkey.Function.CameraRotateLeft)) currentRotation += rotateSpeed * Time.deltaTime;

        transform.localRotation = Quaternion.Slerp ( transform.localRotation, Quaternion.Euler ( 50.0f, currentRotation, 0.0f ), Time.deltaTime * rotateDamp );
    }

    private Vector3 RotateAroundPoint(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 direction = point - pivot;
        direction = Quaternion.Euler ( angles ) * direction;
        point = direction + pivot;
        return point;
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
