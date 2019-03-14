using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProfessionController : MonoBehaviour {

    public static ProfessionController Instance;
    public List<Profession> professions { get; protected set; }
    public Dictionary<ProfessionType, int> professionCount { get; protected set; }
    private List<CitizenJob> citizenJobs = new List<CitizenJob> ();

    private void Awake ()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy ( this.gameObject );

        CreateProfessions ();
    }
    
    private void CreateProfessions ()
    {
        professions = new List<Profession> ();
        professionCount = new Dictionary<ProfessionType, int> ();

        professions.Add ( new Profession ( ProfessionType.None, 0, 6 ) );
        professions.Add ( new Profession ( ProfessionType.Student, 3, 6 ) );
        professions.Add ( new Profession ( ProfessionType.Worker ) );
        professions.Add ( new Profession ( ProfessionType.Lumberjack ) );
        professions.Add ( new Profession ( ProfessionType.Quarryman ) );
        professions.Add ( new Profession ( ProfessionType.Stonemason ) );
        
    }

    public void SetProfession (CitizenJob citizen, ProfessionType type)
    {
        Profession profession = ReturnProfessionByType ( type );
        if (profession == null) return;

        int citizenAge = citizen.cBase.CitizenAge.Age;
        if (citizenAge < profession.minimumAge || citizenAge > profession.maximumAge) return;

        citizen.UpdateProfession ( type );
        RefreshAllProfessions ();
    }

    public void SetProfession (CitizenJob citizen, string stringType)
    {
        ProfessionType type = ProfessionType.None;
        bool found = false;

        for (int i = 0; i < professions.Count; i++)
        {
            if (professions[i].type.ToString () == stringType)
            {
                found = true;
                type = professions[i].type;
                break;
            }
        }

        if (!found) { Debug.LogError ( "Profession String " + stringType + " not found" ); return; }

        Profession profession = ReturnProfessionByType ( type );
        if (profession == null) return;

        int citizenAge = citizen.cBase.CitizenAge.Age;
        if (citizenAge < profession.minimumAge || citizenAge > profession.maximumAge) return;

        citizen.UpdateProfession ( type );
        RefreshAllProfessions ();
    }

    public void IncreaseProfession (ProfessionType type)
    {
        if (type == ProfessionType.None || type == ProfessionType.Student || type == ProfessionType.Worker) return;

        RefreshCitizens ();

        for (int i = 0; i < citizenJobs.Count; i++)
        {
            bool found = false;            
            if (citizenJobs[i].profession == ProfessionType.Worker) found = true;

            if (citizenJobs[i].profession == ProfessionType.None || citizenJobs[i].profession == ProfessionType.Student)
            {
                if(CitizenAgeIsCompatible(citizenJobs[i], ProfessionType.None ))
                {
                    found = true;
                }
            }

            if (found)
            {
                SetProfession ( citizenJobs[i], type );
                break;
            }            
        }
    }

    public void DecreaseProfession (ProfessionType type)
    {
        if (type == ProfessionType.None || type == ProfessionType.Student || type == ProfessionType.Worker) return;

        RefreshCitizens ();

        for (int i = 0; i < citizenJobs.Count; i++)
        {
            if (citizenJobs[i].profession == type)
            {
                SetProfession ( citizenJobs[i], ProfessionType.Worker );
                break;
            }
        }
    }

    public Profession ReturnProfessionByType(ProfessionType type)
    {
        for (int i = 0; i < professions.Count; i++)
        {
            if(professions[i].type == type)
            {
                return professions[i];
            }
        }

        Debug.LogError ( "Please set up a profession for type " + type );
        return null;
    }

    private bool CitizenAgeIsCompatible (CitizenJob citizen, ProfessionType profession)
    {
        Profession prof = ReturnProfessionByType ( profession );
        if (prof == null) return false;

        if (citizen.cBase.CitizenAge.Age < prof.minimumAge || citizen.cBase.CitizenAge.Age > prof.maximumAge) return false;

        return true;
    }

    private void RefreshCitizens ()
    {
        citizenJobs = FindObjectsOfType<CitizenJob> ().ToList ();
    }

    private void RefreshAllProfessions ()
    {
        professionCount = new Dictionary<ProfessionType, int> ();

        for (int i = 0; i < professions.Count; i++)
        {
            professionCount.Add ( professions[i].type, 0 );
        }

        RefreshCitizens ();

        for (int i = 0; i < citizenJobs.Count; i++)
        {
            if(!professionCount.ContainsKey( citizenJobs[i].profession)) { Debug.LogError ( "Profession does not exist in dictionary" ); continue; }

            professionCount[citizenJobs[i].profession] += 1;
        }
    }
}
