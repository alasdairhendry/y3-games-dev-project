using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenGraphics : MonoBehaviour {

    private CitizenBase cBase;
    //private CitizenMovement characterMovement;
    [SerializeField] private GameObject marketCartGraphics;
    [SerializeField] private GameObject satchelGraphics;
    
    [SerializeField] private GameObject lanternGraphics;
    [SerializeField] private GameObject crateGraphics;
    [SerializeField] private GameObject logsGraphics;
    [SerializeField] private GameObject rocksGraphics;

    [SerializeField] private GameObject axeGraphics;
    [SerializeField] private Transform[] axePlaceholders;

    private Light lanternLight;
    private Animator animator;

    private float lanternWeightCounter = 0.0f;
    private bool isUsingLantern = false;

    private void Start ()
    {
        cBase = GetComponent<CitizenBase> ();
        cBase.Inventory.RegisterOnInventoryChanged ( OnInventoryChanged );
        
        animator = GetComponent<Animator> ();
        lanternLight = lanternGraphics.GetComponentInChildren<Light> ();
        SunController.Instance.onSwitch += SetUsingLantern;
    }

    private void Update ()
    {
        UpdateLanternWeight ();
    }

    public void OnInventoryChanged(ResourceInventory inventory)
    {
        if (inventory.IsEmpty ())
        {
            if (satchelGraphics != null)
                satchelGraphics.SetActive ( false );

            GetComponent<IconDisplayer> ().RemoveIcon ( IconDisplayer.IconType.Inventory );
            cBase.CitizenMovement.SetAnimationState = CitizenMovement.AnimationState.Walking;
        }
        else
        {
            if (cBase.CitizenJob.GetCurrentJob.IsNull ())
            {
                cBase.CitizenMovement.SetAnimationState = CitizenMovement.AnimationState.Walking;
                GetComponent<IconDisplayer> ().AddIcon ( IconDisplayer.IconType.Inventory );         

                if (satchelGraphics != null)
                    satchelGraphics.SetActive ( true );
            }
        }
    }

    public void OnUseAxeAction(bool useAxe)
    {
        if (useAxe)
        {
            axeGraphics.transform.SetParent ( axePlaceholders[1] );
            axeGraphics.transform.localEulerAngles = Vector3.zero;
            axeGraphics.transform.localPosition = Vector3.zero;
            cBase.CitizenMovement.SetUsingTool ( useAxe );
        }
        else
        {
            axeGraphics.transform.SetParent ( axePlaceholders[0] );
            axeGraphics.transform.localEulerAngles = Vector3.zero;
            axeGraphics.transform.localPosition = Vector3.zero;
            cBase.CitizenMovement.SetUsingTool ( useAxe );
        }
    }

    public void SetUsingCart(bool state)
    {
        DisableGraphics ( crateGraphics, logsGraphics, rocksGraphics );

        if (marketCartGraphics != null)
            marketCartGraphics.SetActive ( state );

        cBase.CitizenMovement.SetAnimationState = state == true ? CitizenMovement.AnimationState.Cart : CitizenMovement.AnimationState.Idle;
    }

    public void SetUsingCrate(bool state)
    {
        DisableGraphics ( logsGraphics, marketCartGraphics, rocksGraphics );

        if (crateGraphics != null)
            crateGraphics.SetActive ( state );

        cBase.CitizenMovement.SetAnimationState = state == true ? CitizenMovement.AnimationState.Carrying : CitizenMovement.AnimationState.Idle;
    }

    public void SetUsingLogs (bool state)
    {
        DisableGraphics ( crateGraphics, marketCartGraphics, rocksGraphics );

        if (logsGraphics != null)
            logsGraphics.SetActive ( state );

        cBase.CitizenMovement.SetAnimationState = state == true ? CitizenMovement.AnimationState.Carrying : CitizenMovement.AnimationState.Idle;
    }

    public void SetUsingRocks (bool state)
    {
        DisableGraphics ( crateGraphics, marketCartGraphics, logsGraphics );

        if (rocksGraphics != null)
            rocksGraphics.SetActive ( state );

        cBase.CitizenMovement.SetAnimationState = state == true ? CitizenMovement.AnimationState.Carrying : CitizenMovement.AnimationState.Idle;
    }

    private void DisableGraphics(params GameObject[] objects)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].SetActive ( false );
        }
    }

    private void UpdateLanternWeight ()
    {
        if (isUsingLantern)
        {
            if (lanternWeightCounter == 1.0f) return;

            lanternWeightCounter += GameTime.DeltaGameTime;

            if (lanternWeightCounter >= 1.0f) lanternWeightCounter = 1.0f;

            animator.SetLayerWeight ( 1, lanternWeightCounter );
            lanternLight.intensity = lanternWeightCounter;
        }
        else
        {
            if (lanternWeightCounter == 0.0f) return;

            lanternWeightCounter -= GameTime.DeltaGameTime;

            if (lanternWeightCounter <= 0.0f) lanternWeightCounter = 0.0f;

            animator.SetLayerWeight ( 1, lanternWeightCounter );
            lanternLight.intensity = lanternWeightCounter;
        }
    }

    private void SetUsingLantern(SunController.Time time)
    {
        if(time == SunController.Time.Day)
        {
            isUsingLantern = false;
            lanternGraphics.SetActive ( false );          
        }
        else
        {
            isUsingLantern = true;
            lanternGraphics.SetActive ( true );
        }
    }

    private void OnDestroy ()
    {
        SunController.Instance.onSwitch -= SetUsingLantern;
    }
}
