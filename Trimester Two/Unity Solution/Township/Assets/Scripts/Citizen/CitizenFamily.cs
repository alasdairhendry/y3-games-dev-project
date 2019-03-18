using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenFamily : MonoBehaviour {

    private CitizenBase cBase;

    public enum Gender { Male, Female }
    public Gender gender { get; protected set; }  
    
    public int SkinColour { get; protected set; }

	public CitizenFamilyMember Father { get; protected set; }
	public CitizenFamilyMember Mother { get; protected set; }
	public List<CitizenFamilyMember> Children { get; protected set; }

    public CitizenFamilyMember Partner { get; protected set; }
    public bool HasPartner { get; protected set; }

    public CitizenFamilyMember thisMember { get; protected set; }

    public int familyID { get; protected set; }
    public string familyName { get; protected set; }

    public bool isPregnant { get; protected set; }

    private PersistentData.CitizenData loadedData;
    
    public void SetFromNew (string firstName, string familyName, int familyID, Gender gender)
    {
        cBase = GetComponent<CitizenBase> ();
        this.gender = gender;
        this.familyName = familyName;
        this.familyID = familyID;
        this.Children = new List<CitizenFamilyMember> ();

        thisMember = new CitizenFamilyMember () { firstName = firstName, citizenBase = cBase };
        this.gameObject.name = "Citizen-" + firstName + " " + familyName;

        Father = new CitizenFamilyMember () { firstName = "Unknown", citizenBase = null, skinColour = Random.Range(0, 3) };
        Mother = new CitizenFamilyMember () { firstName = "Unknown", citizenBase = null, skinColour = Random.Range ( 0, 3 ) };

        cBase.CitizenAge.SetInitialAge ( Random.Range ( 18, 30 ) );

        SetSkinColour ();

        ProfessionController.Instance.SetProfession ( cBase.CitizenJob, ProfessionType.Worker );

        SetSelectable ();
    }
        
    public void SetFromParents (string firstName, Gender gender, CitizenFamilyMember father, CitizenFamilyMember mother)
    {
        cBase = GetComponent<CitizenBase> ();
        this.gender = gender;
        this.Children = new List<CitizenFamilyMember> ();

        thisMember = new CitizenFamilyMember () { firstName = firstName, citizenBase = cBase };

        Father = father;
        Mother = mother;        

        this.familyName = Father.citizenBase.CitizenFamily.familyName;
        this.gameObject.name = "Citizen-" + firstName + " " + familyName;

        cBase.CitizenAge.SetInitialAge ( 0 );

        SetSkinColour ();

        ProfessionController.Instance.SetProfession ( cBase.CitizenJob, ProfessionType.None );

        if (Mother.citizenBase != null) { Mother.citizenBase.CitizenFamily.AddChild ( thisMember ); this.familyID = mother.citizenBase.CitizenFamily.familyID; }
        if (Father.citizenBase != null) { Father.citizenBase.CitizenFamily.AddChild ( thisMember ); this.familyID = father.citizenBase.CitizenFamily.familyID; }

        SetSelectable ();
    }

    public void SetFromLoaded(PersistentData.CitizenData data)
    {
        loadedData = data;
        cBase = GetComponent<CitizenBase> ();
        this.Children = new List<CitizenFamilyMember> ();

        this.Father = new CitizenFamilyMember ();
        this.Mother = new CitizenFamilyMember ();
        this.Partner = new CitizenFamilyMember ();

        this.gender = (Gender)data.Gender;
        this.familyName = data.FamilyName;
        this.familyID = data.FamilyID;

        this.gameObject.name = "Citizen-" + data.FirstName + " " + familyName;

        thisMember = new CitizenFamilyMember () { firstName = data.FirstName, citizenBase = cBase };
        SetSkinColour ( data.SkinColour );

        this.cBase.CitizenAge.SetInitialAge ( data.Age, data.Birthday );
        ProfessionController.Instance.SetProfession ( this.cBase.CitizenJob, (ProfessionType)data.Profession );

        SetSelectable ();
    }

    public void SetFamilyFromLoaded ()
    {
        if (loadedData == null) { Debug.LogError ( "No loaded data found." ); return; }

        if(loadedData.FatherID == -1)
        {
            Father = new CitizenFamilyMember () { firstName = "Unknown", citizenBase = null, skinColour = Random.Range ( 0, 3 ) };
        }
        else
        {
            CitizenBase father = EntityManager.Instance.GetCitizenBaseByID ( loadedData.FatherID );
            if (father == null) { Debug.LogError ( "Father was null at ID: " + loadedData.FatherID ); }
            else
            {
                Father = new CitizenFamilyMember ();
                Father.citizenBase = father;
                Father.firstName = father.CitizenFamily.thisMember.firstName;
                Father.skinColour = father.CitizenFamily.thisMember.skinColour;
            }
        }

        if (loadedData.MotherID == -1)
        {
            Mother = new CitizenFamilyMember () { firstName = "Unknown", citizenBase = null, skinColour = Random.Range ( 0, 3 ) };
        }
        else
        {
            CitizenBase mother = EntityManager.Instance.GetCitizenBaseByID ( loadedData.MotherID );
            if (mother == null) { Debug.LogError ( "Mother was null at ID: " + loadedData.MotherID ); }
            else
            {
                Mother = new CitizenFamilyMember ();
                Mother.citizenBase = mother;
                Mother.firstName = mother.CitizenFamily.thisMember.firstName;
                Mother.skinColour = mother.CitizenFamily.thisMember.skinColour;
            }
        }

        if (loadedData.PartnerID == -1)
        {
            Partner = null;
        }
        else
        {
            CitizenBase partner = EntityManager.Instance.GetCitizenBaseByID ( loadedData.PartnerID );
            if (partner == null) { Debug.LogError ( "Partner was null at ID: " + loadedData.PartnerID ); }
            else
            {
                Partner = new CitizenFamilyMember ();
                Partner.citizenBase = partner;
                Partner.firstName = partner.CitizenFamily.thisMember.firstName;
                Partner.skinColour = partner.CitizenFamily.thisMember.skinColour;
            }
        }

        if(loadedData.ChildrenIDs.Count<= 0)
        {
            Children = new List<CitizenFamilyMember> ();
        }
        else
        {
            Children = new List<CitizenFamilyMember> ();

            for (int i = 0; i < loadedData.ChildrenIDs.Count; i++)
            {
                CitizenBase childBase = EntityManager.Instance.GetCitizenBaseByID ( loadedData.ChildrenIDs[i] );
                if (childBase == null) { Debug.LogError ( "Child was null at ID: " + loadedData.ChildrenIDs[i] ); }
                else
                {
                    CitizenFamilyMember child = new CitizenFamilyMember ();
                    child.citizenBase = childBase;
                    child.firstName = childBase.CitizenFamily.thisMember.firstName;
                    child.skinColour = childBase.CitizenFamily.thisMember.skinColour;

                    AddChild ( child );
                }
            }
        }

    }

    public void AddChild(CitizenFamilyMember child)
    {
        Children.Add ( child );
    }

    public void SetPartner(CitizenFamilyMember partner)
    {
        if(partner.citizenBase == null) { Debug.LogError ( "NO" ); return; }
        if(partner.citizenBase.CitizenFamily == null) { Debug.LogError ( "NO2" ); return; }
        if (partner.citizenBase.CitizenFamily.gender == this.gender) return;
        if (Children.Count > 0) return;

        Partner = partner;

        if (gender == Gender.Female)
        {
            familyID = Partner.citizenBase.CitizenFamily.familyID;
            familyName = Partner.citizenBase.CitizenFamily.familyName;
        }

        HasPartner = true;
    }

    public void GetPregnant ()
    {
        if(this.gender == Gender.Female && HasPartner)
        {
            isPregnant = true;
            DEBUG_CHARACTER_SPAWNER.Instance.CreateBabyCitizen ( Partner, thisMember );
            isPregnant = false;
        }
    }

    private void SetSelectable ()
    {
        GetComponent<Inspectable> ().SetAdditiveAction ( () =>
        {
            HUD_EntityInspection_Citizen_Panel panel = FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ();

            panel.AddTextData ( (pair) =>
            {
                if (thisMember == null) return "None";
                string s = "";
                if (!string.IsNullOrEmpty ( thisMember.firstName )) s += thisMember.firstName;
                if (!string.IsNullOrEmpty ( familyName )) s += " " + familyName;
                if (string.IsNullOrEmpty ( s )) s = "None";
                return s;
            }, "Name", "Overview" );

            if (cBase.CitizenJob.profession == ProfessionType.None || cBase.CitizenJob.profession == ProfessionType.Student)
            {
                panel.AddTextData ( (pair) =>
                  {
                      return cBase.CitizenJob.profession.ToString ();
                  }, "Profession", "Overview" );
            }
            else
            {
                panel.AddDropdownData ( (index, options) =>
                {
                    ProfessionController.Instance.SetProfession ( cBase.CitizenJob, options[index].text );
                }, (pair) =>
                {
                    return cBase.CitizenJob.profession.ToString ();
                }, "Profession", "Overview",
                ProfessionType.Worker.ToString (),
                ProfessionType.Lumberjack.ToString (),
                ProfessionType.Quarryman.ToString (),
                ProfessionType.Stonemason.ToString (),
                ProfessionType.Fisherman.ToString () );
            }

            panel.AddTextData ( (pair) =>
            {
                return gender.ToString ();
            }, "Gender", "Overview" );

            panel.AddTextData ( (pair) =>
            {
                return cBase.CitizenAge.Age.ToString ();
            }, "Age", "Overview" );

            //panel.AddTextData ( () =>
            //{
            //    return familyID.ToString ( "00" );
            //}, "Family ID", "Family" );

            panel.AddButtonTextData ( () =>
            {
                if (Father == null) return;
                if (string.IsNullOrEmpty ( Father.firstName )) return;

                else if (Father.citizenBase == null) return;
                else
                {
                    FindObjectOfType<CameraMovement> ().LockTo ( Father.citizenBase.transform );
                    Father.citizenBase.GetComponentInChildren<Inspectable> ().Inspect ();
                }
            }, (pair) =>
            {
                if (Father == null) return "None";
                if (string.IsNullOrEmpty ( Father.firstName )) return "None";

                if (Father.citizenBase == null && Father.firstName == "Unknown") return Father.firstName;
                else if (Father.citizenBase == null) return Father.firstName + " (Deceased)";
                else return Father.firstName;

            }, "Father", "Family" );

            panel.AddButtonTextData ( () =>
             {
                 if (Mother == null) return;
                 if (string.IsNullOrEmpty ( Mother.firstName )) return;

                 else if (Mother.citizenBase == null) return;
                 else
                 {
                     FindObjectOfType<CameraMovement> ().LockTo ( Mother.citizenBase.transform );
                     Mother.citizenBase.GetComponentInChildren<Inspectable> ().Inspect ();
                 }
             }, (pair) =>
             {
                 if (Mother == null) return "None";
                 if (string.IsNullOrEmpty ( Mother.firstName )) return "None";

                 if (Mother.citizenBase == null && Mother.firstName == "Unknown") return Mother.firstName;
                 else if (Mother.citizenBase == null) return Mother.firstName + " (Deceased)";
                 else return Mother.firstName;

             }, "Mother", "Family" );

            for (int i = 0; i < Children.Count; i++)
            {
                int x = i;
                panel.AddButtonTextData ( () =>
                {
                    if (x < 0 || x >= Children.Count) return;
                    if (Children[x] == null) return;
                    if (string.IsNullOrEmpty ( Children[x].firstName )) return;

                    else if (Children[x].citizenBase == null) return;
                    else
                    {
                        FindObjectOfType<CameraMovement> ().LockTo ( Children[x].citizenBase.transform );
                        Children[x].citizenBase.GetComponentInChildren<Inspectable> ().Inspect ();
                    }
                }, (pair) =>
                {
                    if (x < 0 || x >= Children.Count) return "None";
                    if (Children[x] == null) return "None";
                    if (string.IsNullOrEmpty ( Children[x].firstName )) return "None";

                    if (Children[x].citizenBase == null && Children[x].firstName == "Unknown") return Children[x].firstName;
                    else if (Children[x].citizenBase == null) return Children[x].firstName + " (Deceased)";
                    else return Children[x].firstName;

                }, "Child", "Family" );
            }

            panel.AddButtonTextData ( () =>
            {
                if (Partner == null) return;
                if (string.IsNullOrEmpty ( Partner.firstName )) return;

                else if (Partner.citizenBase == null) return;
                else
                {
                    FindObjectOfType<CameraMovement> ().LockTo ( Partner.citizenBase.transform );
                    Partner.citizenBase.GetComponentInChildren<Inspectable> ().Inspect ();
                }
            }, (pair) =>
            {
                if (Partner == null) return "None";
                if (string.IsNullOrEmpty ( Partner.firstName )) return "None";
                if (Partner.citizenBase == null) return Partner.firstName + " (Deceased)";
                else return Partner.firstName;
            }, "Partner", "Family" );

            if (gender == Gender.Female)
            {
                panel.AddButtonData ( () =>
                  {
                      GetPregnant ();
                  }, "Get Pregnant", "Family" );
            }
        } );
    }

    private void SetSkinColour (int skinColour = -1)
    {
        if (skinColour == -1)
        {
            int x = Father.skinColour;
            int y = Mother.skinColour;

            if (x == 0 & y == 0)
                thisMember.skinColour = 0;
            else if ((x == 1 && y == 0) || (x == 0 && y == 1))
                thisMember.skinColour = 0;
            else if ((x == 2 && y == 0) || (x == 0 && y == 2))
                thisMember.skinColour = 1;
            else if ((x == 2 && y == 1) || (x == 1 && y == 2))
                thisMember.skinColour = 2;
        }
        else
        {
            thisMember.skinColour = skinColour;
        }

        cBase.CitizenGraphics.SetCitizenMaterialSpecific ( thisMember.skinColour );
    }

    public class CitizenFamilyMember
    {
        public string firstName;
        public int skinColour;
        public CitizenBase citizenBase;
    }
}
