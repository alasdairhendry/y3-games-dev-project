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
            Birthday = GameTime.currentDayOfTheYear;
        else Birthday = birthday;

        if (Age < 3) cBase.CitizenAnimation.SetWalkClip ( false );
        SetSize ();
        //GameTime.onDayChanged += OnDayChanged;
        GameTime.onMonthChanged += OnMonthChanged;
    }

    public void OnMonthChanged(int previous, int current)
    {
        return;

        Age++;
        OnAgeChanged ( Age );
    }

    bool done = false;
    private void Update ()
    {
        if (done) return;
        if (Input.GetKeyDown ( KeyCode.O ))
        {
            OnAgeChanged ( 10 );
            done = true;
        }
    }

    //public void OnDayChanged(int previous, int current)
    //{
    //    if(current == Birthday)
    //    {
    //        Age++;
    //        OnAgeChanged (Age);
    //    }
    //}

    private void OnAgeChanged (int newAge)
    {
        if(newAge == 3)
        {
            /// If unlocked education, begin education.
            cBase.CitizenAnimation.SetWalkClip ( true );
        }
        else if(newAge == 6)
        {
            // Begin work as a child
            // TODO: Notify player of a new worker
            ProfessionController.Instance.SetProfession ( cBase.CitizenJob, ProfessionType.Worker );
        }
        else if(newAge == 10)
        {
            Debug.Log ( cBase.CitizenFamily.thisMember.fullName + " is has turned 10" );
            PartnerController.Instance.SetEligible ( this.cBase );
        }

        SetSize ();
    }

    private void SetSize ()
    {
        float scaleFactor = Mathf.Lerp ( 0.5f, 1.0f, (float)Age / 6.0f );
        transform.GetChild ( 0 ).localScale = Vector3.one * scaleFactor;
    }

    private void OnDestroy ()
    {
        //GameTime.onDayChanged -= OnDayChanged;
        GameTime.onMonthChanged -= OnMonthChanged;
    }
}
