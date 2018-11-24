using UnityEngine;

public class Buildable : MonoBehaviour {

    private Prop prop;
    public Prop GetPropData { get { return prop; } }

    private float constructionPercent = 0.0f;
    public float ConstructionPercent { get { return constructionPercent; } }

    public bool IsComplete { get { return constructionPercent >= 100.0f ? true : false; } }

    private int currentStage = 0;
    private int stageCount = 0;

    private bool hasBegun = false;

    [SerializeField] private MaterialPoint[] materialPoints;
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

        transform.Find ( "Graphics" ).GetChild ( 0 ).gameObject.SetActive ( false );        
        SetStage ();
        CreateJobs ();
        return this;
    }

    private void CreateJobs ()
    {
        for (int i = 0; i < prop.data.requiredMaterials.Count; i++)
        {
            Job_Haul job = new Job_Haul ( "Haul Item ID " + prop.data.requiredMaterials[i].id, true, prop.data.requiredMaterials[i].id, prop.data.requiredMaterials[i].amount, this);
            JobController.QueueJob ( job );
        }
    }

    public void AddConstructionPercentage (float amount)
    {
        if (IsComplete) { Debug.Log ( "complete - returning" ); return; }

        Debug.Log ( "AddConstructionPercentage - " + amount);

        constructionPercent += amount;
        CheckStages ();
    }

    public void AddMaterial (int resourceID, float quantity)
    {
        inventory.AddItemQuantity ( resourceID, quantity );
        CheckMaterials ();
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
        Debug.Log ( "Creating new Job Item (Build Job) for item " + this.gameObject.name );
        Job_Build job = new Job_Build ( "Build object " + prop.data.name, true, 5.0f, this );
        JobController.QueueJob ( job );
    }

    private void CheckStages ()
    {
        float targetPercent = ((float)currentStage + 1 / (float)stageCount) * 100.0f;

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
            transform.Find ( "Graphics" ).Find ( "Stages" ).GetChild ( currentStage - 1 ).gameObject.SetActive ( false );            
        }

        Transform t = transform.Find ( "Graphics" ).Find ( "Stages" ).GetChild ( currentStage );
        Debug.Log ( t.name );
        t.gameObject.SetActive ( true );
    }

    private void Complete ()
    {        
        transform.Find ( "Graphics" ).Find ( "Stages" ).gameObject.SetActive ( false );
        transform.Find ( "Graphics" ).GetChild ( 0 ).gameObject.SetActive ( true );
        PropManager.Instance.OnPropBuilt ( this.gameObject );
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
    }
}
