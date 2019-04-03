using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenAge : MonoBehaviour {

    public CitizenBase cBase { get; protected set; }
    public int Age { get; protected set; }
    public int Birthday { get; protected set; }

    public System.Action<int> onAgeChanged;

    private void Awake ()
    {
        cBase = GetComponent<CitizenBase> ();
    }

    public void SetInitialAge (int age, int birthday = -1)
    {
        Age = age;

        if (birthday == -1)
            Birthday = GameTime.currentDayOfTheMonth;
        else Birthday = birthday;

        if (Age < 3) cBase.CitizenAnimation.SetWalkClip ( false );
        SetSize ();
        GameTime.onDayChanged += OnDayChanged;
    }   

    public void OnDayChanged (int previous, int current)
    {
        if (CheckDeath ()) return;

        if (current == Birthday)
        {
            Age++;
            OnAgeChanged ( Age );
        }
    }

    private void OnAgeChanged (int newAge)
    {
        if(newAge == 3)
        {
            /// If unlocked education, begin education.
            cBase.CitizenAnimation.SetWalkClip ( true );

            ProfessionController.Instance.SetProfession ( cBase.CitizenJob, ProfessionType.Student );

            if (GamePreferences.Instance.preferences.showNotification_SpecialBirthday)
                HUD_Notification_Panel.Instance.AddNotification ( cBase.CitizenFamily.thisMember.fullName + " has grown up and is now a student", null, cBase.Inspectable );

        }
        else if(newAge == 6)
        {
            // Begin work as a child
            // TODO: Notify player of a new worker
            ProfessionController.Instance.SetProfession ( cBase.CitizenJob, ProfessionType.Worker );

            if (GamePreferences.Instance.preferences.showNotification_SpecialBirthday)
                HUD_Notification_Panel.Instance.AddNotification ( cBase.CitizenFamily.thisMember.fullName + " has grown up and can now work", null, cBase.Inspectable );
        }
        else if(newAge == 10)
        {
            if (!cBase.CitizenFamily.HasPartner)
                PartnerController.Instance.SetEligible ( this.cBase );

            if (GamePreferences.Instance.preferences.showNotification_SpecialBirthday)
                HUD_Notification_Panel.Instance.AddNotification ( cBase.CitizenFamily.thisMember.fullName + " has grown up and can now marry", null, cBase.Inspectable );
        }
        else if(newAge == 12)
        {
            if (GamePreferences.Instance.preferences.showNotification_SpecialBirthday)
                HUD_Notification_Panel.Instance.AddNotification ( cBase.CitizenFamily.thisMember.fullName + " has grown up and can now have children", null, cBase.Inspectable );
        }
        else
        {
            if (GamePreferences.Instance.preferences.showNotification_Birthday)
                HUD_Notification_Panel.Instance.AddNotification ( cBase.CitizenFamily.thisMember.fullName + " has turned " + newAge.ToString (), null, cBase.Inspectable );
        }

        SetSize ();
    }

    private bool CheckDeath ()
    {
        float v = Random.Range ( 0.0f, 100.0f );
        float prob = (GameData.Instance.MortalityCurve.Evaluate ( Age ) * 0.05f) * GameData.Instance.startingConditions.mortalityRate;

        if (v < prob)
        {
            string reason = "";

            if (Age <= 6) reason = "Young Age";
            else if (Age <= 45) reason = "Unexpected Causes";
            else reason = "Old Age";

            cBase.KillCitizen ( reason );
            return true;
        }
        else
        {
            //Debug.LogError ( "Citizen survived day " + day + " at age " + age );
            return false;
        }
    }

    private void SetSize ()
    {
        float scaleFactor = Mathf.Lerp ( 0.5f, 1.0f, (float)Age / 6.0f );
        transform.GetChild ( 0 ).localScale = Vector3.one * scaleFactor;
    }

    private void OnDestroy ()
    {
        GameTime.onDayChanged -= OnDayChanged;
    }
}
