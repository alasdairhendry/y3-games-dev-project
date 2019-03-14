using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DEBUG_CHARACTER_SPAWNER : MonoBehaviour
{
    public static DEBUG_CHARACTER_SPAWNER Instance;

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

    private int familyIDCounter = 0;
    private int citizenIDCounter = 0;

    private List<CitizenBase> loadedCitizens = new List<CitizenBase> ();

    //public void SpawnCharacter ()
    //{
    //    GameObject go = Instantiate ( prefab, spawnPosition, Quaternion.identity );
    //    go.GetComponent<NavMeshAgent> ().avoidancePriority = spawnCounter;

    //    spawnCounter++;

    //    if (spawnCounter > 100) spawnCounter = 1;
    //}

    public void LOAD_Citizen (PersistentData.CitizenData citizen)
    {
        GameObject go = Instantiate ( prefab, citizen.Position.ToVector3 () + new Vector3 ( 0.0f, 150.0f, 0.0f ), Quaternion.identity );
        loadedCitizens.Add ( go.GetComponent<CitizenBase> () );
        go.GetComponent<CitizenBase> ().SetID ( citizenIDCounter );
        go.GetComponent<CitizenFamily> ().SetFromLoaded ( citizen );
        go.GetComponent<CitizenMovement> ().AvoidancePriority = citizenIDCounter % 100;
        citizenIDCounter++;

        if (citizen.FamilyID > familyIDCounter) familyIDCounter = citizen.FamilyID;
    }

    public void LOAD_CitizenFamilies ()
    {
        for (int i = 0; i < loadedCitizens.Count; i++)
        {
            loadedCitizens[i].CitizenFamily.SetFamilyFromLoaded ();
        }
    }

    public void CreateSingleCitizen ()
    {
        GameObject male = CreateCitizen ( 0 );
    }

    public void CreateInitialFamily ()
    {
        GameObject male = CreateCitizen ( 0 );
        GameObject female = CreateCitizen ( 1 );

        male.GetComponent<CitizenFamily> ().SetPartner ( female.GetComponent<CitizenFamily> ().thisMember );
        female.GetComponent<CitizenFamily> ().SetPartner ( male.GetComponent<CitizenFamily> ().thisMember );
    }

    public GameObject CreateCitizen (int gender)
    {
        GameObject go = Instantiate ( prefab, spawnPosition + (Random.insideUnitSphere * 5), Quaternion.identity );
        go.GetComponent<CitizenBase> ().SetID ( citizenIDCounter );

        go.GetComponent<CitizenFamily> ().SetFromNew ( GetGenderedName ( (CitizenFamily.Gender)gender ), GetFamilyName (), familyIDCounter, (CitizenFamily.Gender)gender );

        go.GetComponent<CitizenMovement> ().AvoidancePriority = citizenIDCounter % 100;
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
        citizenIDCounter++;

        return go;
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
