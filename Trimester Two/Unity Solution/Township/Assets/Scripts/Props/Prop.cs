﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Prop : MonoBehaviour {

    public ResourceInventory inventory;
    public WorldEntity worldEntity { get; protected set; }

    public PropData data { get; protected set; }
    public Buildable buildable { get; protected set; }
    private bool isBlueprint = true;
    public bool IsBluePrint { get { return isBlueprint; } }

    [SerializeField] protected Vector3 citizenInteractionPoint;
    public Vector3 CitizenInteractionPointGlobal { get { return transform.Find ( "citizenInteractionPlaceholder" ).position; } }

    private bool collectedNavMeshProbes = false;
    private List<Transform> navMeshGroundProbes = new List<Transform> ();
    private List<Transform> navMeshWaterProbes = new List<Transform> ();

    public System.Action<Prop> onDestroy;

    public Inspectable Inspectable { get; protected set; }

    protected virtual void Awake ()
    {
        isBlueprint = true;
        SetActiveNavMeshObstacles ( false );
    }

    protected virtual void Start () { }

    protected virtual void Update () { }

    // Called by BuildMode when the prop is placed on the terrain
    public void Place (PropData data)
    {
        this.data = data;
        isBlueprint = false;
        buildable = GetComponent<Buildable> ().OnPropPlaced ();        
        SetActiveNavMeshObstacles ( true );

        CreateInteractionObject ();

        worldEntity = GetComponent<WorldEntity> ().Setup ( data.name, WorldEntity.EntityType.Building );
        Inspectable = GetComponent<Inspectable> ();
        Inspectable.enabled = true;

        SnowController.Instance.SetObjectMaterial ( GetComponentsInChildren<MeshRenderer> ( true ), false );
        EntityManager.Instance.OnEntityCreated ( this.gameObject, this.GetType () );

        SetInventory ();
        OnPlaced ();
    }

    public void LOAD_Place(PersistentData.PropData data, PropData propData)
    {
        this.data = propData;
        isBlueprint = false;
        buildable = GetComponent<Buildable> ();
        SetActiveNavMeshObstacles ( true );

        CreateInteractionObject ();

        worldEntity = GetComponent<WorldEntity> ().Setup ( propData.name, WorldEntity.EntityType.Building );
        Inspectable = GetComponent<Inspectable> ();
        Inspectable.enabled = true;

        SnowController.Instance.SetObjectMaterial ( GetComponentsInChildren<MeshRenderer> ( true ), false );
        EntityManager.Instance.OnEntityCreated ( this.gameObject, this.GetType () );

        inventory = new ResourceInventory ( data.PropInventoryCapacity );
        for (int i = 0; i < data.PropResourceIDs.Count; i++)
        {
            inventory.AddItemQuantity ( data.PropResourceIDs[i], data.PropResourceQuantities[i] );
        }

        OnPlaced ();
        buildable.LOAD_OnPropPlaced ( data );
    }

    // Called by Place() when the prop has been placed on the terrain
    protected virtual void OnPlaced ()
    {

    }

    // Called by the buildable when this prop is finished building
    public virtual void OnBuilt ()
    {

    }

    protected virtual void SetActiveNavMeshObstacles(bool state)
    {
        NavMeshObstacle[] obstacles = GetComponentsInChildren<NavMeshObstacle> ( true );
        NavMeshObstacle obstacle = GetComponent<NavMeshObstacle> ();

        if (obstacle != null)
            obstacle.enabled = state;

        for (int i = 0; i < obstacles.Length; i++)
        {
            obstacles[i].enabled = state;
        }
    }

    protected virtual void SetInventory ()
    {
        inventory = new ResourceInventory ( 10 );
    }

    protected virtual void SetInspectable ()
    {
        Inspectable.SetDestroyAction ( () => { DestroyProp (); }, false, "Bulldoze" );
        Inspectable.SetFocusAction ( () => { Inspectable.InspectAndLockCamera (); }, false );

        Inspectable.SetAdditiveAction ( () =>
        {
            HUD_EntityInspection_Citizen_Panel panel = FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ();

            panel.AddButtonData ( () =>
            {
                if (this == null) return;
                if (this.buildable == null) return;
                if (this.gameObject == null) return;
                this.buildable.CompleteInspectorDEBUG ();

            }, "Complete", "Any" );        
        } );
    }

    private void CreateInteractionObject ()
    {
        GameObject citizenInteractionPlaceholder = new GameObject ( "citizenInteractionPlaceholder" );
        citizenInteractionPlaceholder.transform.SetParent ( this.transform );
        citizenInteractionPlaceholder.transform.localRotation = Quaternion.identity;
        citizenInteractionPlaceholder.transform.localPosition = citizenInteractionPoint;

        if (citizenInteractionPoint == null || citizenInteractionPoint == Vector3.zero)
        {
            Debug.LogError ( "You may have forgot to assign the interaction point in the inspector!", this );
            citizenInteractionPoint = Vector3.zero;
        }
    }

    public virtual void DestroyProp ()
    {
        if (onDestroy != null) onDestroy ( this );

        if (this == null) return;
        if (this.gameObject == null) return;

        EntityManager.Instance.OnEntityDestroyed ( this.gameObject, this.GetType() );
        GetComponent<JobEntity> ().DestroyJobs ();
        SnowController.Instance.SetObjectMaterial ( GetComponentsInChildren<MeshRenderer> ( true ), true );
        Destroy ( this.gameObject );
    }

    protected virtual void CollectNavMeshProbes ()
    {
        Transform navMesh = transform.Find ( "NavMesh" );
        if(navMesh == null) { Debug.LogError ( "NavMesh probes don't exist" ); return; }

        foreach (Transform child in transform.Find("NavMesh"))
        {
            if (child.name.Contains ( "Ground" )) navMeshGroundProbes.Add ( child );
            if (child.name.Contains ( "Water" )) navMeshWaterProbes.Add ( child );
        }

        collectedNavMeshProbes = true;
    }

    public virtual bool SampleSurface (PropData _data)
    {
        if (!collectedNavMeshProbes) CollectNavMeshProbes ();

        bool onGround = true;
        bool onWater = true;

        for (int i = 0; i < navMeshGroundProbes.Count; i++)
        {
            if (!BuildMode.Instance.SampleNavMesh ( navMeshGroundProbes[i].position, 1 ))
            {
                onGround = false;
            }
            else
            {
                FindObjectOfType<HUD_Tooltip_Panel> ().RemoveTooltip ( "Terrain not suitable." );
            }
        }

        for (int i = 0; i < navMeshWaterProbes.Count; i++)
        {
            if (!BuildMode.Instance.SampleNavMesh ( navMeshWaterProbes[i].position, 8 ))
            {
                onWater = false;
            }
        }

        switch (_data.placementArea)
        {
            case PlacementArea.Ground:

                if (onGround)
                {
                    RemoveTooltip ();
                    return true;
                }
                else
                {
                    SetTooltip ( "Terrain location not suitable", HUD_Tooltip_Panel.Tooltip.Preset.Error );
                    return false;
                }

            case PlacementArea.Waterside:

                if (onGround && onWater)
                {
                    RemoveTooltip ();
                    return true;
                }
                else
                {
                    SetTooltip ( "Must be placed on shoreline", HUD_Tooltip_Panel.Tooltip.Preset.Error );
                    return false;
                }

            case PlacementArea.Water:

                if (onWater)
                {
                    RemoveTooltip ();
                    return true;
                }
                else
                {
                    SetTooltip ( "Must be placed on water", HUD_Tooltip_Panel.Tooltip.Preset.Error );
                    return false;
                }

            default:
                Debug.LogError ( "Placement type " + _data.placementArea.ToString () + " not found" );
                RemoveTooltip ();
                return false;
        }
    }

    GameObject tooltipObject;
    private string currentTooltipMessage;

    private void SetTooltip(string message, HUD_Tooltip_Panel.Tooltip.Preset preset)
    {
        if (currentTooltipMessage == message) return;
        if (tooltipObject != null) RemoveTooltip ();
        tooltipObject = HUD_Tooltip_Panel.Instance.AddTooltip ( message, preset );
        currentTooltipMessage = message;
    }

    private void RemoveTooltip ()
    {
        currentTooltipMessage = "";
        if (tooltipObject != null)
            HUD_Tooltip_Panel.Instance.RemoveTooltip ( tooltipObject );
    }

    private void OnDestroy ()
    {
        RemoveTooltip ();
    }

    private void OnDrawGizmosSelected ()
    {
        if (citizenInteractionPoint == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere ( transform.position + citizenInteractionPoint, 0.5f );
    }
}
