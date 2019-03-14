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
    public float currentRotation { get; protected set; }

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

    public static bool CameraPanned { get; protected set; }
    //public static bool CameraZoomed { get; protected set; }
    public static bool CameraRotated { get; protected set; }
    public static bool CameraMoved { get; protected set; }

    private Vector3 previousPosition = new Vector3 ();
    private Vector3 previousEuler = new Vector3 ();

    private void Start()
    {        
        targetPosition = transform.position;            
    }

    public void LOAD_Position (PersistentData.SaveData data)
    {
        targetPosition = data.CameraPosition.ToVector3 ();
        currentRotation = data.CameraRotation;
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
        CheckMovement ();

        if (isLocked && lockTarget != null)
        {
            FollowTarget ();
        }
	}

    private void CheckMovement ()
    {
        int moveFlag = 0;
        if(previousPosition != transform.position)
        {
            CameraPanned = true;
            moveFlag++;
        }
        else
        {
            CameraPanned = false;
        }

        if(previousEuler != transform.eulerAngles)
        {
            CameraRotated = true;
            moveFlag++;
        }
        else
        {
            CameraRotated = false;
        }

        if (moveFlag > 0)
        {
            CameraMoved = true;
        }
        else
        {
            CameraMoved = false;
        }

        previousPosition = transform.position;
        previousEuler = transform.eulerAngles;
    }

    private void LateUpdate()
    {
        targetPosition.x = Mathf.Clamp ( targetPosition.x, xClamp.x, xClamp.y );
        targetPosition.z = Mathf.Clamp ( targetPosition.z, zClamp.x, zClamp.y );
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * panDamp);
    }

    private void Pan()
    {
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

        if (isLocked)
        {
            if (Input.GetAxis ( "Mouse ScrollWheel" ) != 0.0f) isLocked = false;
        }

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
        Vector3 targetFollowPosition = lockTarget.position;
        targetPosition = targetFollowPosition;
        Zoom ();
    }

    public void LockTo(Transform target)
    {
        lockTarget = target;
        isLocked = true;
    }
}
