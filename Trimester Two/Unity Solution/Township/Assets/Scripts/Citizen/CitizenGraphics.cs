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
    [SerializeField] private GameObject rodGraphics;
    [SerializeField] private GameObject pickAxeGraphics;

    [SerializeField] private GameObject axeGraphics;
    [SerializeField] private Transform[] axePlaceholders;

    [SerializeField] private List<CitizenMesh> citizenMeshes = new List<CitizenMesh> ();
    [SerializeField] private List<Material> citizenMaterials = new List<Material> ();

    private Light lanternLight;
    private Animator animator;

    private float lanternWeightCounter = 0.0f;
    private bool isUsingLantern = false;
    private bool allowLantern = true;

    private void Awake ()
    {
        cBase = GetComponent<CitizenBase> ();
        cBase.Inventory.RegisterOnInventoryChanged ( OnInventoryChanged );

        cBase.CitizenJob.onProfessionChanged += OnProfessionChanged;

        animator = GetComponent<Animator> ();
        lanternLight = lanternGraphics.GetComponentInChildren<Light> ();
        SetUsingLantern ( SunController.Instance.time );
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

            GetComponent<IconDisplayer> ().RemoveIconByType ( IconDisplayer.IconType.Inventory );
            //cBase.CitizenAnimation.SetAnimationState = CitizenAnimation.AnimationState.Walking;
            //Debug.Log ( "booo" );
        }
        else
        {
            if (cBase.CitizenJob.GetCurrentJob.IsNull ())
            {
                //Debug.Log ( "2booo" );
                //cBase.CitizenAnimation.SetAnimationState = CitizenAnimation.AnimationState.Walking;
                GetComponent<IconDisplayer> ().AddIconGeneric ( IconDisplayer.IconType.Inventory );         

                if (satchelGraphics != null)
                    satchelGraphics.SetActive ( true );
            }
        }
    }

    public void SetUsingAxe (bool state, CitizenAnimation.AxeUseAnimation animState)
    {
        if (state)
        {
            axeGraphics.transform.SetParent ( axePlaceholders[1] );
            axeGraphics.transform.localEulerAngles = Vector3.zero;
            axeGraphics.transform.localPosition = Vector3.zero;

            switch (animState)
            {
                case CitizenAnimation.AxeUseAnimation.Chopping:
                    cBase.CitizenAnimation.SetState_Chopping ( state );
                    break;
                case CitizenAnimation.AxeUseAnimation.Splitting:
                    cBase.CitizenAnimation.SetState_Splitting ( state );
                    break;
            }
        }
        else
        {
            axeGraphics.transform.SetParent ( axePlaceholders[0] );
            axeGraphics.transform.localEulerAngles = Vector3.zero;
            axeGraphics.transform.localPosition = Vector3.zero;

            switch (animState)
            {
                case CitizenAnimation.AxeUseAnimation.Chopping:
                    cBase.CitizenAnimation.SetState_Chopping ( state );
                    break;
                case CitizenAnimation.AxeUseAnimation.Splitting:
                    cBase.CitizenAnimation.SetState_Splitting ( state );
                    break;
            }
        }
    }

    public void SetUsingPickaxe (bool state)
    {
        DisableGraphics ( logsGraphics, marketCartGraphics, rocksGraphics, rodGraphics, crateGraphics );

        if (pickAxeGraphics != null)
            pickAxeGraphics.SetActive ( state );

        cBase.CitizenAnimation.SetState_Pickaxing ( state );
    }

    public void SetUsingCart(bool state)
    {
        DisableGraphics ( crateGraphics, logsGraphics, rocksGraphics, rodGraphics, pickAxeGraphics );

        if (marketCartGraphics != null)
            marketCartGraphics.SetActive ( state );

        cBase.CitizenAnimation.SetAnimationState = state == true ? CitizenAnimation.AnimationState.Cart : CitizenAnimation.AnimationState.Idle;
    }

    public void SetUsingCrate(bool state)
    {
        DisableGraphics ( logsGraphics, marketCartGraphics, rocksGraphics, rodGraphics, pickAxeGraphics );

        if (crateGraphics != null)
            crateGraphics.SetActive ( state );

        cBase.CitizenAnimation.SetAnimationState = state == true ? CitizenAnimation.AnimationState.Carrying : CitizenAnimation.AnimationState.Idle;
    }

    public void SetUsingLogs (bool state)
    {
        DisableGraphics ( crateGraphics, marketCartGraphics, rocksGraphics, rodGraphics, pickAxeGraphics );

        if (logsGraphics != null)
            logsGraphics.SetActive ( state );

        cBase.CitizenAnimation.SetAnimationState = state == true ? CitizenAnimation.AnimationState.Carrying : CitizenAnimation.AnimationState.Idle;
    }

    public void SetUsingRocks (bool state)
    {
        DisableGraphics ( crateGraphics, marketCartGraphics, logsGraphics, rodGraphics, pickAxeGraphics );

        if (rocksGraphics != null)
            rocksGraphics.SetActive ( state );

        cBase.CitizenAnimation.SetAnimationState = state == true ? CitizenAnimation.AnimationState.Carrying : CitizenAnimation.AnimationState.Idle;
    }

    public void SetUsingRod (bool state)
    {
        DisableGraphics ( crateGraphics, marketCartGraphics, logsGraphics, rocksGraphics, pickAxeGraphics );

        if (rodGraphics != null)
            rodGraphics.SetActive ( state );

        cBase.CitizenAnimation.SetAnimationState = state == true ? CitizenAnimation.AnimationState.Fishing : CitizenAnimation.AnimationState.Idle;

        allowLantern = !state;
        SetUsingLantern ( SunController.Instance.time );
    }

    public void SetIsSquashing (bool state)
    {
        DisableGraphics ( crateGraphics, marketCartGraphics, logsGraphics, rocksGraphics, pickAxeGraphics, rodGraphics );
        cBase.CitizenAnimation.SetAnimationState = state == true ? CitizenAnimation.AnimationState.Squashing : CitizenAnimation.AnimationState.Idle;
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
        if (!allowLantern)
        {
            isUsingLantern = false;
            lanternGraphics.SetActive ( false );
            return;
        }

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

    private void OnProfessionChanged(ProfessionType type)
    {
        if (this == null) return;

        for (int i = 0; i < citizenMeshes.Count; i++)
        {
            if(citizenMeshes[i].types.Contains(type))
            {
                GetComponentInChildren<SkinnedMeshRenderer> ().sharedMesh = citizenMeshes[i].male;
            }
        }
    }

    public void UpdateCitizenMesh ()
    {
        for (int i = 0; i < citizenMeshes.Count; i++)
        {
            if (citizenMeshes[i].types.Contains(cBase.CitizenJob.profession))
            {
                GetComponentInChildren<SkinnedMeshRenderer> ().sharedMesh = citizenMeshes[i].male;
            }
        }
    }

    public void SetCitizenMaterialSpecific (int index)
    {
        GetComponentInChildren<SkinnedMeshRenderer> ().material = citizenMaterials[index];
    }

    private void OnDestroy ()
    {
        SunController.Instance.onSwitch -= SetUsingLantern;
    }

    [System.Serializable]
    private class CitizenMesh
    {
        public List<ProfessionType> types;
        public Mesh male;        
    }
}
