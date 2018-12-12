using UnityEngine;

public class Buildable : MonoBehaviour {

    private Prop prop;
    public Prop GetPropData { get { return prop; } }

    private float constructionPercent = 0.0f;
    public float ConstructionPercent { get { return constructionPercent; } }

    public bool IsComplete { get { return constructionPercent >= 100.0f ? true : false; } }

    private int currentStage = 0;
    private int stageCount = 0;

    public System.Action onComplete;

    private bool hasBegun = false;

    [SerializeField] private bool stackedStages = true;
    [SerializeField] private MaterialPoint[] materialPoints;
    [SerializeField] private GameObject materialGraphic;
    [SerializeField] private ResourceInventory inventory;
    public ResourceInventory GetInventory { get { return inventory; } }

    private void Start ()
    {
        if (hasBegun) return;

        inventory = new ResourceInventory ();
        stageCount = transform.Find ( "Graphics" ).Find ( "Stages" ).childCount;
        for (int i = 0; i < stageCount; i++)
        {
            transform.Find ( "Graphics" ).Find ( "Stages" ).GetChild ( i ).gameObject.SetActive ( false );
        }
    }
   
    public Buildable Begin ()
    {        
        hasBegun = true;
        prop = GetComponent<Prop> ();
        inventory = new ResourceInventory ();

        stageCount = transform.Find ( "Graphics" ).Find ( "Stages" ).childCount;
        if (stageCount <= 0) { Complete (); return this; }

        for (int i = 0; i < stageCount; i++)
        {
            transform.Find ( "Graphics" ).Find ( "Stages" ).GetChild ( i ).gameObject.SetActive ( false );
        }

        transform.Find ( "Graphics" ).Find ( "Stage_Complete" ).gameObject.SetActive ( false );        
        SetStage ();
        CreateJobs ();
        return this;
    }

    private void CreateJobs ()
    {
        for (int i = 0; i < prop.data.requiredMaterials.Count; i++)
        {
            GetComponent<JobEntity> ().CreateJob_Haul ( "Haul Item ID " + prop.data.requiredMaterials[i].id, true, prop.data.requiredMaterials[i].id, prop.data.requiredMaterials[i].amount, this );
            //Job_Haul job = new Job_Haul ( "Haul Item ID " + prop.data.requiredMaterials[i].id, true, prop.data.requiredMaterials[i].id, prop.data.requiredMaterials[i].amount, this);
            //JobController.QueueJob ( job );
        }
    }

    public void AddConstructionPercentage (float amount)
    {
        if (IsComplete) { Debug.Log ( "complete - returning" ); return; }

        constructionPercent += amount;
        CheckStages ();
    }

    public void AddMaterial (int resourceID, float quantity)
    {
        inventory.AddItemQuantity ( resourceID, quantity );
        SpawnMaterialGraphic ();
        CheckMaterials ();
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
        GetComponent<JobEntity> ().CreateJob_Build ( "Build Object " + prop.data.name, true, 5.0f, this );
        //Job_Build job = new Job_Build ( "Build object " + prop.data.name, true, 5.0f, this );
        //JobController.QueueJob ( job );
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
                transform.Find ( "Graphics" ).Find ( "Stages" ).GetChild ( currentStage - 1 ).gameObject.SetActive ( false );          
        }

        Transform t = transform.Find ( "Graphics" ).Find ( "Stages" ).GetChild ( currentStage );        
        t.gameObject.SetActive ( true );

    }

    private void Complete ()
    {
        DestroyMaterialGraphics ();
        transform.Find ( "Graphics" ).Find ( "Stages" ).gameObject.SetActive ( false );
        transform.Find ( "Graphics" ).Find ( "Stage_Complete" ).gameObject.SetActive ( true );
        PropManager.Instance.OnPropBuilt ( this.gameObject );
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

    [System.Serializable]
    public class MaterialPoint
    {
        public bool taken;
        public Vector3 localPosition;
        public GameObject gameObject;
    }
}
