using System.Collections.Generic;
using UnityEngine;

public class Buildable : MonoBehaviour {

    public ResourceInventory inventory;

    private Prop prop;
    public Prop Prop { get { return prop; } }

    private float constructionPercent = 0.0f;
    public float ConstructionPercent { get { return constructionPercent; } }

    public bool IsComplete { get; protected set; }

    public int currentStage { get; protected set; }
    private int stageCount = 0;

    private Dictionary<int, List<GameObject>> stages = new Dictionary<int, List<GameObject>> ();

    public System.Action onComplete;

    [SerializeField] private bool stackedStages = true;
    [SerializeField] private MaterialPoint[] materialPoints;
    [SerializeField] private GameObject materialGraphic;

    private Dictionary<int, IconDisplayer.TextIcon> materialIcons = new Dictionary<int, IconDisplayer.TextIcon> ();

    private void Awake ()
    {
        BuildableStage[] _stages = GetComponentsInChildren<BuildableStage> ();

        for (int i = 0; i < _stages.Length; i++)
        {
            if (stageCount < _stages[i].Stage) stageCount = _stages[i].Stage;

            if (stages.ContainsKey(_stages[i].Stage))
            {
                stages[_stages[i].Stage].Add ( _stages[i].gameObject );
            }
            else
            {
                stages.Add ( _stages[i].Stage, new List<GameObject> () { _stages[i].gameObject } );
            }
        }
    }
   
    public Buildable OnPropPlaced ()
    {        
        prop = GetComponent<Prop> ();
        inventory = new ResourceInventory ();
        inventory.RegisterOnResourceAdded ( OnMaterialAdded );

        if (stageCount <= 0) { Complete (); return this; }

        for (int i = 1; i < stageCount + 1; i++)
        {
            SetStageActive ( i, false );
        }

        SetStage ();
        CreateJobs ();
        return this;
    }

    public Buildable LOAD_OnPropPlaced(PersistentData.PropData data)
    {
        prop = GetComponent<Prop> ();
        inventory = new ResourceInventory ();
        inventory.RegisterOnResourceAdded ( OnMaterialAdded );

        for (int i = 0; i < prop.data.requiredMaterials.Count; i++)
        {
            if (data.BuildableResourceIDs.Contains ( prop.data.requiredMaterials[i].id ))
            {
                int index = data.BuildableResourceIDs.IndexOf ( prop.data.requiredMaterials[i].id );

                if (data.BuildableResourceQuantities[index] > 0)
                {
                    inventory.AddItemQuantity ( data.BuildableResourceIDs[index], data.BuildableResourceQuantities[index], transform, prop.data.UIOffsetY );
                }
            }
        }

        if (stageCount <= 0) { Complete (); return this; }

        for (int i = 1; i < stageCount + 1; i++)
        {
            SetStageActive ( i, false );
        }

        currentStage = data.BuildableStage;

        if(data.ConstructionPercent < 100.0f)
        {
            for (int i = 0; i < currentStage + 1; i++)
            {
                SetStageActive ( i, true );
            }
        }

        AddConstructionPercentage ( data.ConstructionPercent );
        if (IsComplete) return this;

        SetStage ();
        CreateJobs ();
        return this;
    }

    private void CreateJobs ()
    {
        IconDisplayer id = GetComponent<IconDisplayer> ();

        for (int i = 0; i < prop.data.requiredMaterials.Count; i++)
        {
            if (!inventory.CheckHasQuantityAvailable ( prop.data.requiredMaterials[i].id, prop.data.requiredMaterials[i].amount ))
                GetComponent<JobEntity> ().CreateJob_Haul ( "Haul Item ID " + prop.data.requiredMaterials[i].id, true, 5.0f, null, prop.data.requiredMaterials[i].id, prop.data.requiredMaterials[i].amount, prop, inventory );

            if (!materialIcons.ContainsKey ( prop.data.requiredMaterials[i].id ))
            {
                IconDisplayer.TextIcon icon = id.AddTextIcon ( ResourceManager.Instance.GetResourceByID ( prop.data.requiredMaterials[i].id ).image, 16 );
                icon.text.text = "0 / " + prop.data.requiredMaterials[i].amount;
                materialIcons.Add ( prop.data.requiredMaterials[i].id, icon );
            }
        }
    }

    public void AddConstructionPercentage (float amount)
    {
        if (IsComplete) { Debug.Log ( "complete - returning" ); return; }

        constructionPercent += amount;
        CheckStages ();
    }

    public void OnMaterialAdded (int resourceID, float quantity)
    {
        SpawnMaterialGraphic ();
        CheckMaterials ();

        if (materialIcons.ContainsKey ( resourceID ))
        {
            GetComponent<IconDisplayer> ().RemoveIconByObject ( materialIcons[resourceID].go );
        }
    }

    private void SpawnMaterialGraphic ()
    {
        for (int i = 0; i < materialPoints.Length; i++)
        {
            if(materialPoints[i].taken == false)
            {
                GameObject go = Instantiate ( materialGraphic, this.transform );
                go.transform.localPosition = materialPoints[i].localPosition;
                materialPoints[i].taken = true;
                materialPoints[i].gameObject = go;
                return;
            }
        }
    }

    private void DestroyMaterialGraphics ()
    {
        for (int i = 0; i < materialPoints.Length; i++)
        {
            if (materialPoints[i].taken)
            {
                if (materialPoints[i].gameObject != null)
                    Destroy ( materialPoints[i].gameObject );

                materialPoints[i].taken = false;
                materialPoints[i].gameObject = null;
            }
        }
    }

    private void CheckMaterials ()
    {
        for (int i = 0; i < prop.data.requiredMaterials.Count; i++)
        {
            if(!inventory.CheckHasQuantityAvailable(prop.data.requiredMaterials[i].id, prop.data.requiredMaterials[i].amount ))
            {
                return;
            }
        }

        OnMaterialsMet ();
    }

    private void OnMaterialsMet ()
    {
        GetComponent<JobEntity> ().CreateJob_Build ( "Build Object " + prop.data.name, true, 5.0f, null, 5.0f, this );
    }

    float targetPercent = 0;
    private void CheckStages ()
    {
        targetPercent = ((float)(currentStage + 1) / (float)stageCount) * 100.0f;

        if(constructionPercent >= 100.0f)
        {
            Complete ();
        }
        else if(constructionPercent >= targetPercent)
        {
            currentStage++;
            SetStage ();
        }
    }

    private void SetStage ()
    {
        if (currentStage >= 1)
        {
            if (!stackedStages)
            {
                SetStageActive ( currentStage - 1, false );
            }
        }

        SetStageActive ( currentStage, true );
    }

    private void SetStageActive (int stage, bool state)
    {
        if (!stages.ContainsKey ( stage )) { Debug.LogError ( "Why does " + Prop.data.name + " have invalid stuffs " + stage ); return; }        

        for (int x = 0; x < stages[stage].Count; x++)
        {
            stages[stage][x].SetActive ( state );
        }
    }
    
    private void Complete ()
    {
        if (IsComplete) return;
        IsComplete = true;

        constructionPercent = 100.0f;

        DestroyMaterialGraphics ();
        for (int i = 0; i < stageCount + 1; i++)
        {
            SetStageActive ( i, true );
        }
        inventory.UnregisterOnResourceAdded ( OnMaterialAdded );
        prop.OnBuilt ();
        if (onComplete != null) onComplete ();
    }
    
    public void CompleteInspectorDEBUG ()
    {
        if (IsComplete) return;
        IsComplete = true;

        constructionPercent = 100.0f;

        DestroyMaterialGraphics ();
        for (int i = 0; i < stageCount + 1; i++)
        {
            SetStageActive ( i, true );
        }
        inventory.UnregisterOnResourceAdded ( OnMaterialAdded );
        GetComponent<JobEntity> ().DestroyJobs ();
        prop.OnBuilt ();
        if (onComplete != null) onComplete ();
    }

    private void OnDrawGizmosSelected ()
    {
        for (int i = 0; i < materialPoints.Length; i++)
        {
            Gizmos.color = new Color ( 1.0f, 0.5f, 0.0f );
            Gizmos.DrawWireCube ( transform.localPosition + materialPoints[i].localPosition, Vector3.one );
        }
    }

    private void OnDestroy ()
    {
        if (inventory != null)
            inventory.UnregisterOnResourceAdded ( OnMaterialAdded );

        if (inventory != null)
        {
            int materialPointID = 0;

            if (!IsComplete)
            {
                List<Resource> r = ResourceManager.Instance.GetResourceList ();
                for (int i = 0; i < r.Count; i++)
                {
                    if (!inventory.CheckIsEmpty ( r[i].id ))
                    {
                        Resource.DropResource ( r[i].id, inventory.GetAvailableQuantity ( r[i].id ), transform.TransformPoint ( materialPoints[materialPointID].localPosition ), transform.TransformDirection ( Vector3.zero ) );
                        materialPointID++;
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class MaterialPoint
    {
        public bool taken;
        public Vector3 localPosition;
        public GameObject gameObject;
    }
}
