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

    [SerializeField] private AnimationCurve pregnancyProbabilityCurve;
    public bool isPregnant { get; protected set; } = false;
    public int pregnancyDueDay { get; protected set; } = 0;
    public bool isWidowed { get; protected set; } = false;

    private int pregnancyLength = 7;
    private int pregnancyMinAge = 12;
    private int pregnancyMaxAge = 40;
    private float pregnancyProbabilityMultiplier = 0.05f;

    private PersistentData.CitizenData loadedData;
    
    public void SetFromNew (string firstName, string familyName, int familyID, Gender gender)
    {
        cBase = GetComponent<CitizenBase> ();
        this.gender = gender;
        this.familyName = familyName;
        this.familyID = familyID;
        this.Children = new List<CitizenFamilyMember> ();

        Father = new CitizenFamilyMember ( "Unknown", familyName, Random.Range ( 0, 3 ), null );
        Mother = new CitizenFamilyMember ( "Unknown", familyName, Random.Range ( 0, 3 ), null );

        thisMember = new CitizenFamilyMember ( firstName, familyName, SetSkinColour (), cBase );
        this.gameObject.name = "Citizen-" + firstName + " " + familyName;

        //cBase.CitizenAge.onAgeChanged += OnAgeChanged;
        cBase.CitizenAge.SetInitialAge ( Random.Range ( 18, 30 ) );
        PartnerController.Instance.SetEligible ( this.cBase );

        ProfessionController.Instance.SetProfession ( cBase.CitizenJob, ProfessionType.Worker );
        SetSelectable ();

        GetComponent<WorldEntity> ().Setup ( thisMember.fullName, WorldEntity.EntityType.Citizen );
    }
        
    public void SetFromParents (string firstName, Gender gender, CitizenFamilyMember father, CitizenFamilyMember mother)
    {
        cBase = GetComponent<CitizenBase> ();
        this.gender = gender;
        this.Children = new List<CitizenFamilyMember> ();

        Father = father;
        Mother = mother;

        if(Father == null)
        {
            Father = new CitizenFamilyMember ( "Unknown", this.familyName, Random.Range ( 0, 3 ), null );
        }

        if (Father.citizenBase != null)
            this.familyName = Father.citizenBase.CitizenFamily.familyName;
        else if (Mother.citizenBase != null)
            this.familyName = Mother.citizenBase.CitizenFamily.familyName;
        else { Debug.LogError ( "Unknown family name", this.gameObject ); this.familyName = "Unknown"; }

        this.gameObject.name = "Citizen-" + firstName + " " + familyName;

        thisMember = new CitizenFamilyMember ( firstName, this.familyName, SetSkinColour (), cBase );

        //cBase.CitizenAge.onAgeChanged += OnAgeChanged;
        cBase.CitizenAge.SetInitialAge ( 0 );

        ProfessionController.Instance.SetProfession ( cBase.CitizenJob, ProfessionType.None );

        if (Mother.citizenBase != null) { Mother.citizenBase.CitizenFamily.AddChild ( thisMember ); this.familyID = mother.citizenBase.CitizenFamily.familyID; }
        if (Father.citizenBase != null) { Father.citizenBase.CitizenFamily.AddChild ( thisMember ); this.familyID = father.citizenBase.CitizenFamily.familyID; }

        SetSelectable ();
        GetComponent<WorldEntity> ().Setup ( thisMember.fullName, WorldEntity.EntityType.Citizen );
    }

    public void SetFromLoaded(PersistentData.CitizenData data)
    {
        loadedData = data;
        cBase = GetComponent<CitizenBase> ();
        cBase.SetID ( data.ID );

        this.Children = new List<CitizenFamilyMember> ();

        this.Father = new CitizenFamilyMember ( "Unknown", this.familyName, Random.Range ( 0, 3 ), null );
        this.Mother = new CitizenFamilyMember ( "Unknown", this.familyName, Random.Range ( 0, 3 ), null );
        this.Partner = new CitizenFamilyMember ( "Unknown", this.familyName, Random.Range ( 0, 3 ), null );

        this.gender = (Gender)data.Gender;
        this.familyName = data.FamilyName;
        this.familyID = data.FamilyID;

        this.gameObject.name = "Citizen-" + data.FirstName + " " + familyName;

        thisMember = new CitizenFamilyMember ( data.FirstName, data.FamilyName, data.SkinColour, cBase );

        //cBase.CitizenAge.onAgeChanged += OnAgeChanged;
        this.cBase.CitizenAge.SetInitialAge ( data.Age, data.Birthday );

        ProfessionController.Instance.SetProfession ( this.cBase.CitizenJob, (ProfessionType)data.Profession );

        SetSelectable ();
        GetComponent<WorldEntity> ().Setup ( thisMember.fullName, WorldEntity.EntityType.Citizen );
    }

    public void SetFamilyFromLoaded ()
    {
        isWidowed = loadedData.isWidowed;

        if (loadedData == null) { Debug.LogError ( "No loaded data found." ); return; }

        if(loadedData.FatherID == -1)
        {
            Father = new CitizenFamilyMember ( "Unknown", this.familyName, Random.Range ( 0, 3 ), null );
        }
        else
        {
            CitizenBase father = EntityManager.Instance.GetCitizenBaseByID ( loadedData.FatherID );
            if (father == null) { Debug.LogError ( "Father was null at ID: " + loadedData.FatherID ); }
            else
            {
                Father = new CitizenFamilyMember ( father.CitizenFamily.thisMember.firstName, father.CitizenFamily.thisMember.familyName, father.CitizenFamily.thisMember.skinColour, father );
            }
        }

        if (loadedData.MotherID == -1)
        {
            Mother = new CitizenFamilyMember ( "Unknown", this.familyName, Random.Range ( 0, 3 ), null );
        }
        else
        {
            CitizenBase mother = EntityManager.Instance.GetCitizenBaseByID ( loadedData.MotherID );
            if (mother == null) { Debug.LogError ( "Mother was null at ID: " + loadedData.MotherID ); }
            else
            {
                Mother = new CitizenFamilyMember ( mother.CitizenFamily.thisMember.firstName, mother.CitizenFamily.thisMember.familyName, mother.CitizenFamily.thisMember.skinColour, mother );
            }
        }

        if (isWidowed)
        {
            SetPartner ( null );
        }
        else
        {
            CitizenBase partner = null;

            try
            {
                partner = EntityManager.Instance.GetCitizenBaseByID ( loadedData.PartnerID );
            }
            catch
            {
                Debug.Log ( "This is an error - PartnerID = " + loadedData.PartnerID, this.gameObject );
            }

            if (partner == null)
            {
                SetPartner ( null );
                if (cBase.CitizenAge.Age >= 10)
                    PartnerController.Instance.SetEligible ( this.cBase );
            }
            else
            {
                //Partner = new CitizenFamilyMember ( partner.CitizenFamily.thisMember.firstName, partner.CitizenFamily.familyName, partner.CitizenFamily.thisMember.skinColour, partner );
                SetPartner ( partner.CitizenFamily.thisMember );
            }
        }

        if (loadedData.PartnerID == -1)
        {
            Partner = null;
        }
        else
        {
            CitizenBase partner = null;
            try { partner = EntityManager.Instance.GetCitizenBaseByID ( loadedData.PartnerID ); } catch { Debug.Log ( "BOop" ); }
            if (partner == null)
            {
                SetPartner ( null );
                Debug.LogError ( "Partner was null at ID: " + loadedData.PartnerID );
            }
            else
            {
                //Partner = new CitizenFamilyMember ( partner.CitizenFamily.thisMember.firstName, partner.CitizenFamily.familyName, partner.CitizenFamily.thisMember.skinColour, partner );
                SetPartner ( partner.CitizenFamily.thisMember );
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
                    CitizenFamilyMember child = new CitizenFamilyMember ( childBase.CitizenFamily.thisMember.firstName, childBase.CitizenFamily.familyName, childBase.CitizenFamily.thisMember.skinColour, childBase );

                    AddChild ( child );
                }
            }
        }

        SetPregnanyFromLoaded ( loadedData );

    }

    public void SetPregnanyFromLoaded (PersistentData.CitizenData data)
    {
        if (data.isPregnant)
        {
            isPregnant = true;
            //pregnancyPartner =
            pregnancyDueDay = data.pregnancyDueDate;
            GameTime.onDayChanged += CheckGiveBirth;

            //if (data.pregnancyPartnerID == -1)
            //{
            //    pregnancyPartner = new CitizenFamilyMember ( "Unknown", this.familyName, Random.Range ( 0, 3 ), null );
            //}
        }
    }

    public void SetNewFamilyID(int id)
    {
        this.familyID = id;
    }

    public void AddChild(CitizenFamilyMember child)
    {
        Children.Add ( child );
    }

    public void SetPartner(CitizenFamilyMember partner)
    {
        if (partner == null)
        {
            HasPartner = false;
            //Partner = null;
            //PartnerController.Instance.SetEligible ( this.cBase );
            return;
        }

        if (partner.citizenBase == null) { Debug.LogError ( "NO" ); return; }
        if(partner.citizenBase.CitizenFamily == null) { Debug.LogError ( "NO2" ); return; }
        if (partner.citizenBase.CitizenFamily.gender == this.gender) return;
        if (Children.Count > 0) return;

        Partner = partner;
        Partner.citizenBase.OnCitizenDied += (cBase) => { GameTime.onDayChanged -= CheckPregnancy; SetPartner ( null ); isWidowed = true; };

        if (gender == Gender.Female)
        {
            GameTime.onDayChanged += CheckPregnancy;
            familyID = Partner.citizenBase.CitizenFamily.familyID;
            familyName = Partner.citizenBase.CitizenFamily.familyName;
            thisMember.UpdateName ( partner.citizenBase.CitizenFamily.familyName );
            GetComponent<WorldEntity> ().Setup ( thisMember.fullName, WorldEntity.EntityType.Citizen );
        }

        HasPartner = true;        
    }

    /// <summary>
    /// Called each day. Calculates whether a person can/should become pregnant
    /// </summary>
    private void CheckPregnancy (int p, int c)
    {
        if(this.gender == Gender.Male) { Debug.LogError ( "Males cannot get pregnant" ); return; }
        if (isPregnant) { return; }
        if (!HasPartner) { Debug.LogError ( "Citizen does not have partner" ); return; }
        if (Children.Count >= GameData.Instance.startingConditions.maximumChildren) { GameTime.onDayChanged -= CheckPregnancy; return; }

        if (cBase.CitizenAge.Age < pregnancyMinAge) { return; }
        if (cBase.CitizenAge.Age > pregnancyMaxAge) { GameTime.onDayChanged -= CheckPregnancy; return; }

        float probability = pregnancyProbabilityCurve.Evaluate ( Mathf.InverseLerp ( pregnancyMinAge, pregnancyMaxAge, cBase.CitizenAge.Age ) );
        float newProbability = pregnancyProbabilityMultiplier * probability;
        newProbability *= GameData.Instance.startingConditions.fertilityModifier;

        if(Random.value <= newProbability)
        {
            GetPregnant ();
        }
    }

    //private void OnGUI ()
    //{
    //    if (this.gender == Gender.Female)
    //        if (GUILayout.Button ( "Get Pregnant" )) GetPregnant ();
    //}

    public void GetPregnant ()
    {
        isPregnant = true;
        pregnancyDueDay = GameTime.currentDayOfTheGame + pregnancyLength;
        GameTime.onDayChanged += CheckGiveBirth;

        if (GamePreferences.Instance.preferences.showNotification_Pregnancy)
            HUD_Notification_Panel.Instance.AddNotification ( thisMember.fullName + " has became pregnant", null, cBase.Inspectable );
    }

    /// <summary>
    /// Called each day when pregnant. Calculates whether a citizen should give birth now or not
    /// </summary>
    private void CheckGiveBirth (int p, int c)
    {
        if(GameTime.currentDayOfTheGame >= pregnancyDueDay)
        {
            GiveBirth ();
            GameTime.onDayChanged -= CheckGiveBirth;
        }
    }

    private void GiveBirth ()
    {
        CitizenBase baby = CitizenController.Instance.CreateBabyCitizen ( Partner, thisMember ).GetComponent<CitizenBase> ();

        if (GamePreferences.Instance.preferences.showNotification_Birth)
        {
            if (baby.CitizenFamily.gender == Gender.Male)
                HUD_Notification_Panel.Instance.AddNotification ( thisMember.fullName + " has given birth to a son", null, baby.Inspectable );
            else
                HUD_Notification_Panel.Instance.AddNotification ( thisMember.fullName + " has given birth to a daughter", null, baby.Inspectable );
        }

        isPregnant = false;
    }

    private void SetSelectable ()
    {
        GetComponent<Inspectable> ().SetAdditiveAction ( () =>
        {
            HUD_EntityInspection_Citizen_Panel panel = FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ();



            panel.AddTextData ( (pair) =>
            {
                return gender.ToString ();
            }, "Gender", "Overview" );

            panel.AddTextData ( (pair) =>
            {
                return cBase.CitizenAge.Age.ToString ();
            }, "Age", "Overview" );

            panel.AddButtonTextData ( () =>
            {
                if (Father == null) return;
                if (string.IsNullOrEmpty ( Father.firstName )) return;

                else if (Father.citizenBase == null) return;
                else
                {
                    Father.citizenBase.Inspectable.InspectAndLockCamera ();
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
                     Mother.citizenBase.Inspectable.InspectAndLockCamera ();
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

            panel.AddButtonTextData ( () =>
            {
                if (Partner == null) return;
                if (string.IsNullOrEmpty ( Partner.firstName )) return;

                else if (Partner.citizenBase == null) return;
                else
                {
                    Partner.citizenBase.Inspectable.InspectAndLockCamera ();
                    Partner.citizenBase.GetComponentInChildren<Inspectable> ().Inspect ();
                }
            }, (pair) =>
            {
                if (isWidowed) return "(Widowed)";
                if (Partner == null) return "None";
                if (string.IsNullOrEmpty ( Partner.firstName )) return "None";
                if (Partner.citizenBase == null) return Partner.firstName + " (Deceased)";
                else return Partner.firstName;
            }, "Partner", "Family" );

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
                        Children[x].citizenBase.Inspectable.InspectAndLockCamera ();
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

            if (this.gender == Gender.Female)
            {
                panel.AddTextData ( (pair) =>
                {
                    if (isPregnant) return "Yes"; else return "No";
                }, "Pregnant", "Family" );
            }
        } );
    }

    private int SetSkinColour (int skinColour = -1)
    {
        if (skinColour == -1)
        {
            int x = Father.skinColour;
            int y = Mother.skinColour;

            if (x == 0 & y == 0)
                return 0;
            else if ((x == 1 && y == 0) || (x == 0 && y == 1))
                return 0;
            else if ((x == 2 && y == 0) || (x == 0 && y == 2))
                return 1;
            else if ((x == 2 && y == 1) || (x == 1 && y == 2))
                return 2;
            else return 0;
        }
        else
        {
            return skinColour;
        }

        //cBase.CitizenGraphics.SetCitizenMaterialSpecific ( thisMember.skinColour );
    }

    private void OnDestroy ()
    {
        GameTime.onDayChanged -= CheckPregnancy;
    }

    public class CitizenFamilyMember
    {
        public string firstName { get; protected set; }
        public string familyName { get; protected set; }
        public string fullName { get; protected set; }

        public int skinColour { get; protected set; }
        public CitizenBase citizenBase { get; protected set; }

        public CitizenFamilyMember (string firstName, string familyName, int skinColour, CitizenBase citizenBase)
        {
            this.firstName = firstName;
            this.familyName = familyName;
            this.fullName = firstName + " " + familyName;
            this.skinColour = skinColour;
            this.citizenBase = citizenBase;
            
            citizenBase?.CitizenGraphics.SetCitizenMaterialSpecific ( this.skinColour );
        }

        public void UpdateName (string familyName)
        {
            this.familyName = familyName;
            this.fullName = firstName + " " + familyName;
        }
    }
}
