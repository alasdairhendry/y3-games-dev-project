using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class BuildMode : ModeBase {

    public static BuildMode Instance;

    private PropData currentPropData;
    private GameObject currentPropOutline;

    private float outlineYRotation = 180.0f;
    private float outlineYIncrementRotation = 180.0f;

    [SerializeField] private Material[] outlineMaterials;

    [SerializeField] private bool axisSnap = false;

    protected override void Awake ()
    {
        base.Awake ();

        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy ( this.gameObject );

        isActive = false;
    }

    public override void StartMode ()
    {
        base.StartMode ();

        if (currentPropOutline == null) currentPropOutline = new GameObject ( "propOutline" );
        isActive = true;
        TriggerCallback ();
    }

    public override void StopMode ()
    {
        base.StopMode ();

        isActive = false;
        TriggerCallback ();
    }

    public override void Toggle ()
    {
        base.Toggle ();      
    }

    public void SetPropData (PropData data)
    {
        if (!isActive) return;
        DestroyPropOutline ();

        currentPropData = data;
        GameObject g = Instantiate ( data.Prefab );
        g.transform.SetParent ( currentPropOutline.transform );
        g.transform.localPosition = Vector3.zero;
        g.transform.localEulerAngles = Vector3.zero;
        g.transform.localScale = Vector3.one;

        currentPropOutline.transform.rotation = Quaternion.identity;
        outlineYRotation = 180.0f;
        outlineYIncrementRotation = 180.0f;
        FindObjectOfType<HUD_Tooltip_Panel> ().AddTooltip ( "£300.00", HUD_Tooltip_Panel.Tooltip.Preset.Warning );
        CheckMoving ();
    }

    protected override void Update ()
    {
        base.Update ();

        if (!isActive) return;
        if (currentPropOutline == null) return;

        RotationHandle ();
        CheckClicking ();
        if (!Hotkey.MouseMoved && !CameraMovement.CameraMoved) return;
        CheckMoving ();
    }

    private void CheckMoving ()
    {
        Ray ray = Camera.main.ScreenPointToRay ( Input.mousePosition );
        RaycastHit hit;
        int mask = 1 << 9;

        if (Physics.Raycast ( ray, out hit, 10000, mask ))
        {
            MovementHandle ( hit );
            GraphicsHandle ( hit.point );
        }
    }

    private void CheckClicking ()
    {
        if (Input.GetMouseButtonDown ( 0 ) && !Input.GetKey ( KeyCode.LeftControl ))
        {
            Ray ray = Camera.main.ScreenPointToRay ( Input.mousePosition );
            RaycastHit hit;
            int mask = 1 << 9;

            if (Physics.Raycast ( ray, out hit, 10000, mask ))
            {
                PlaceHandle ( hit.point );
            }
        }
    }

    private void MovementHandle (RaycastHit hit)
    {        
        currentPropOutline.transform.position = hit.point;    
    }

    bool hasRotatedIncrementally = false;

    private void RotationHandle ()
    {
        Vector3 r = currentPropOutline.transform.localEulerAngles;

        if (Hotkey.GetKeyDown ( Hotkey.Function.PropRotateIncrementClockwise ))
        {
            if (outlineYRotation % 45.0f == 0)
            {
                outlineYRotation += 45.0f;
            }
            else
            {
                outlineYRotation = Mathf.CeilToInt ( outlineYRotation / 45.0f );
                outlineYRotation = outlineYRotation * 45.0f;
            }
            hasRotatedIncrementally = true;
        }
        else if (Hotkey.GetKeyDown ( Hotkey.Function.PropRotateIncrementAntiClockwise ))
        {
            if (outlineYRotation % 45.0f == 0)
            {
                outlineYRotation -= 45.0f;
            }
            else
            {
                outlineYRotation = Mathf.FloorToInt ( outlineYRotation / 45.0f );
                outlineYRotation = outlineYRotation * 45.0f;
            }
            hasRotatedIncrementally = true;
        }

        if (!hasRotatedIncrementally)
        {
            if (Hotkey.GetKey ( Hotkey.Function.PropRotateClockwise ))
                outlineYRotation += Time.deltaTime * 75.0f;
            else if (Hotkey.GetKey ( Hotkey.Function.PropRotateAntiClockwise ))
                outlineYRotation -= Time.deltaTime * 75.0f;
        }

        if (Hotkey.GetKeyUp ( Hotkey.Function.PropRotateIncrementAntiClockwise ) || Hotkey.GetKeyUp ( Hotkey.Function.PropRotateIncrementClockwise )) hasRotatedIncrementally = false;

        r.y = outlineYRotation;

        currentPropOutline.transform.rotation = Quaternion.Slerp ( currentPropOutline.transform.rotation, Quaternion.Euler ( r ), Time.deltaTime * 20 );
    }

    private void GraphicsHandle (Vector3 point)
    {
        if (SampleEligibility ( point, 1 ))
        {
            if (currentPropOutline.GetComponentInChildren<PropVisualiser> () == null) return;
            currentPropOutline.GetComponentInChildren<PropVisualiser> ().Visualise ( outlineMaterials[0] );
        }
        else
        {
            if (currentPropOutline.GetComponentInChildren<PropVisualiser> () == null) return;
            currentPropOutline.GetComponentInChildren<PropVisualiser> ().Visualise ( outlineMaterials[1] );
        }
    }

    private void PlaceHandle (Vector3 point)
    {
        if (!EventSystem.current.IsPointerOverGameObject ())
        {
            if (SampleEligibility ( point, 1 ))
            {
                RemoveRawMaterials ();

                GameObject go = Instantiate ( currentPropData.Prefab );
                go.transform.position = point;
                go.transform.rotation = currentPropOutline.transform.rotation;
                go.transform.name = "PlacedObject: " + currentPropData.name;
                Prop prop = go.GetComponent<Prop> ();
                prop.Place ( currentPropData );
            }
        }
    }

    private void RemoveRawMaterials ()
    {
        List<RawMaterialCollider> colliders = currentPropOutline.GetComponentInChildren<SpatialCollider> ().EnvironmentColliders;

        for (int i = 0; i < colliders.Count; i++)
        {
            if (colliders[i] == null) continue;
            RawMaterial rm = colliders[i].GetComponentInParent<RawMaterial> ();

            //if (rm.RemovableByBuilding)
                rm.RemoveOnBuildingPlaced ();
        }
    }    

    private bool SampleCollider ()
    {
        if (currentPropOutline.GetComponentInChildren<SpatialCollider> () == null) return false;
        return !currentPropOutline.GetComponentInChildren<SpatialCollider> ().IsColliding;
    }

    private bool SampleEligibility (Vector3 point, int mask)
    {
        if (SamplePropOnNavMesh ( point ) && SampleCollider ())
        {
            return true;
        }
        else return false;
    }

    private bool SamplePropOnNavMesh (Vector3 point)
    {
        return currentPropOutline.GetComponentInChildren<Prop> ().SampleSurface ();
        //if (currentPropData.placementArea == PlacementArea.Waterside)
        //{
        //    bool valid = false;
        //    Transform navMesh = currentPropOutline.transform.GetChild(0).Find ( "NavMesh" );
        //    if (navMesh == null) { Debug.LogError ( "NavMesh Child not found" );  return false; }

        //    valid = SampleNavMesh ( navMesh.Find ( "Ground" ).position, 1 );
        //    if (!valid) return valid;

        //    valid = SampleNavMesh ( navMesh.Find ( "Water" ).position, 8, true );

        //    return valid;
        //}
        //else
        //{
        //    return SampleNavMesh ( point, 1 );
        //}
    }

    public bool SampleNavMesh (Vector3 point, int mask)
    {
        NavMeshHit hit;

        if (NavMesh.SamplePosition ( point, out hit, 1.0f, NavMesh.AllAreas ))
        {
            if (hit.mask == mask)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private void TriggerCallback ()
    {
        //if (isActive) if (OnActivate != null) OnActivate ();
        //if (!isActive) if (OnDeactivate != null) OnDeactivate ();        
        FindObjectOfType<HUD_Tooltip_Panel> ().RemoveTooltip ( "£300.00" );
        DestroyPropOutline ();
    }   
    
    private void DestroyPropOutline ()
    {
        if (currentPropOutline.transform.childCount > 0)
            Destroy ( currentPropOutline.transform.GetChild ( 0 ).gameObject );
    }
}
