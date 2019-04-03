using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CitizenController : MonoBehaviour
{
    public static CitizenController Instance;

    private void Awake ()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy ( this.gameObject );
    }

    [SerializeField] Vector3 spawnPosition;
    [SerializeField] private GameObject prefab;

    [SerializeField] private List<string> maleNames = new List<string> ();
    [SerializeField] private List<string> femaleNames = new List<string> ();
    [SerializeField] private List<string> familyNames = new List<string> ();

    public int familyIDCounter { get; protected set; } = 0;
    private int citizenIDCounter = 0;

    public List<CitizenBase> activeCitizens { get; protected set; } = new List<CitizenBase> ();
    public System.Action<CitizenBase, bool> onCitizenBornOrDied;

    public int currentCitizensCount = 0;

    private void Start ()
    {

    }

    public void Start_NewGame ()
    {
        if(GameData.Instance.gameDataType == GameData.GameDataType.New)
        {
            for (int i = 0; i < GameData.Instance.startingConditions.startingFamilies; i++)
            {
                CreateInitialFamily ();
            }
        }
    }

    public void LoadFamilyIDCounter(int count)
    {
        familyIDCounter = count;
    }

    public void LOAD_Citizen (PersistentData.CitizenData citizen)
    {
        GameObject go = Instantiate ( prefab, citizen.Position.ToVector3 () + new Vector3 ( 0.0f, 150.0f, 0.0f ), Quaternion.identity );
        go.GetComponent<CitizenBase> ().SetID ( citizen.ID );
        go.GetComponent<CitizenBase> ().SetInventory ( citizen.ResourceIDs, citizen.ResourceQuantities );
        go.GetComponent<CitizenFamily> ().SetFromLoaded ( citizen );
        go.GetComponent<CitizenMovement> ().AvoidancePriority = citizenIDCounter % 100;
        OnCitizenCreated ( go.GetComponent<CitizenBase> () );
        citizenIDCounter++;

        //if (citizen.FamilyID > familyIDCounter) familyIDCounter = citizen.FamilyID;
    }

    public void LOAD_CitizenFamilies ()
    {
        for (int i = 0; i < activeCitizens.Count; i++)
        {
            activeCitizens[i].CitizenFamily.SetFamilyFromLoaded ();
        }
    }

    public void CreateSingleCitizen (bool male)
    {
        if (male)
            CreateCitizen ( 0 );
        else CreateCitizen ( 1 );
    }

    public void CreateInitialFamily ()
    {
        GameObject male = CreateCitizen ( 0 );
        GameObject female = CreateCitizen ( 1 );

        //male.GetComponent<CitizenFamily> ().SetPartner ( female.GetComponent<CitizenFamily> ().thisMember );
        //female.GetComponent<CitizenFamily> ().SetPartner ( male.GetComponent<CitizenFamily> ().thisMember );
    }

    public GameObject CreateCitizen (int gender)
    {
        GameObject go = Instantiate ( prefab, spawnPosition + (Random.insideUnitSphere * 5), Quaternion.identity );
        go.GetComponent<CitizenBase> ().SetID ( citizenIDCounter );


        go.GetComponent<CitizenFamily> ().SetFromNew ( GetGenderedName ( (CitizenFamily.Gender)gender ), GetFamilyName (), familyIDCounter, (CitizenFamily.Gender)gender );

        go.GetComponent<CitizenMovement> ().AvoidancePriority = citizenIDCounter % 100;

        OnCitizenCreated ( go.GetComponent<CitizenBase> () );

        citizenIDCounter++;
        familyIDCounter++;

        return go;
    }

    public GameObject CreateBabyCitizen (CitizenFamily.CitizenFamilyMember Father, CitizenFamily.CitizenFamilyMember Mother)
    {
        GameObject go = Instantiate ( prefab, spawnPosition + (Random.insideUnitSphere * 5), Quaternion.identity );
        go.GetComponent<CitizenBase> ().SetID ( citizenIDCounter );

        CitizenFamily.Gender gender = (CitizenFamily.Gender)Random.Range ( 0, 2 );
        go.GetComponent<CitizenFamily> ().SetFromParents ( GetGenderedName ( gender ), gender, Father, Mother );

        go.GetComponent<CitizenMovement> ().AvoidancePriority = citizenIDCounter % 100;
        
        OnCitizenCreated ( go.GetComponent<CitizenBase> () );

        citizenIDCounter++;

        return go;
    }

    private void OnCitizenCreated(CitizenBase cBase)
    {
        cBase.OnCitizenDied += (_cBase) => { OnCitizenDied ( _cBase ); };
        activeCitizens.Add ( cBase );
        onCitizenBornOrDied?.Invoke ( cBase, true );
    }

    private void OnCitizenDied(CitizenBase cBase)
    {
        if (activeCitizens.Contains ( cBase ))
        {
            activeCitizens.Remove ( cBase );
        }
        else
        {
            Debug.LogError ( "Why is this happening" );
        }

        onCitizenBornOrDied?.Invoke ( cBase, false );
    }

    public void SetAsNewFamily(params CitizenBase[] family)
    {
        familyIDCounter++;
        for (int i = 0; i < family.Length; i++)
        {
            family[i].CitizenFamily.SetNewFamilyID ( familyIDCounter );
        }
    }

    private string GetGenderedName(CitizenFamily.Gender gender)
    {
        if (gender == CitizenFamily.Gender.Male)
            return maleNames[Random.Range ( 0, maleNames.Count )];
        else return femaleNames[Random.Range ( 0, femaleNames.Count )];
    }

    private string GetFamilyName ()
    {
        return familyNames[Random.Range ( 0, familyNames.Count )];
    }
}
