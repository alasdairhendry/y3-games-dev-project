using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProfessionController : MonoBehaviour {

    public static ProfessionController Instance;
    public List<Profession> professions { get; protected set; }

    public string[] GetProfessionsAreStringList (int start, int end = 0)
    {
        List<string> strings = new List<string> ();

        if (start >= professions.Count) start--;
        if (end <= 0) end = professions.Count;
        end = Mathf.Clamp ( end, start, professions.Count );

        for (int i = start; i < end; i++)
        {
            strings.Add (professions[i].type.ToString ());
        }

        return strings.ToArray ();
    }

    public Dictionary<ProfessionType, List<CitizenBase>> professionsByCitizen { get; protected set; }

    private List<CitizenJob> citizenJobs = new List<CitizenJob> ();
    public System.Action onRefreshProfessions;

    private void Awake ()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy ( this.gameObject );

        CreateProfessions ();
    }
    
    private void CreateProfessions ()
    {
        professions = new List<Profession> ();
        professionsByCitizen = new Dictionary<ProfessionType, List<CitizenBase>> ();

        professions.Add ( new Profession ( ProfessionType.None, 0, 6 ) );
        professions.Add ( new Profession ( ProfessionType.Student, 3, 6 ) );
        professions.Add ( new Profession ( ProfessionType.Worker ) );
        professions.Add ( new Profession ( ProfessionType.Lumberjack ) );
        professions.Add ( new Profession ( ProfessionType.Charcoal_Burner ) );

        professions.Add ( new Profession ( ProfessionType.Stonemason ) );
        professions.Add ( new Profession ( ProfessionType.Fisherman ) );

        professions.Add ( new Profession ( ProfessionType.Quarryman ) );
        professions.Add ( new Profession ( ProfessionType.Miner ) );
        professions.Add ( new Profession ( ProfessionType.Blacksmith ) );

        professions.Add ( new Profession ( ProfessionType.Winemaker ) );
        professions.Add ( new Profession ( ProfessionType.Vintner ) );

        RefreshAllProfessions ();
        
    }

    public void SetProfession (CitizenJob citizen, ProfessionType type)
    {
        Profession profession = ReturnProfessionByType ( type );
        if (profession == null) { Debug.Log ( "No profession" ); return; }

        int citizenAge = citizen.cBase.CitizenAge.Age;
        if (citizenAge < profession.minimumAge || citizenAge > profession.maximumAge) { Debug.Log ( "Age out of bounds" ); return; }

        citizen.UpdateProfession ( type );
        RefreshAllProfessions ();
    }

    public void SetProfession (CitizenJob citizen, string stringType)
    {
        ProfessionType type = ProfessionType.None;
        bool found = false;

        stringType = stringType.Replace ( " ", "_" );

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

    public void SetProfessionExplicit(CitizenJob citizen)
    {
        citizen.UpdateProfession ( ProfessionType.None );
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
        professionsByCitizen = new Dictionary<ProfessionType, List<CitizenBase>> ();

        for (int i = 0; i < professions.Count; i++)
        {
            professionsByCitizen.Add ( professions[i].type, new List<CitizenBase> () );
        }

        RefreshCitizens ();

        for (int i = 0; i < citizenJobs.Count; i++)
        {
            if(!professionsByCitizen.ContainsKey( citizenJobs[i].profession)) { Debug.LogError ( "Profession does not exist in dictionary" ); continue; }

            professionsByCitizen[citizenJobs[i].profession].Add ( citizenJobs[i].cBase );
        }

        onRefreshProfessions?.Invoke ();
    }
}
