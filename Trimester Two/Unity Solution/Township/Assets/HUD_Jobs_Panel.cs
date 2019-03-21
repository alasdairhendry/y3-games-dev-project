using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Jobs_Panel : UIPanel {

    private List<Job> activeJobs = new List<Job> ();
    private List<Job> openJobs = new List<Job> ();
    private List<Job> completeJobs = new List<Job> ();

    [SerializeField] private GameObject activePanel;
    [SerializeField] private GameObject openPanel;
    [SerializeField] private GameObject completePanel;

    [SerializeField] private GameObject prefab;

    private void Awake ()
    {
        GetComponentInChildren<TabGroup> ().onTabChanged += OnTabChange;
        JobController.onJobsChanged += OnJobsChanged;
    }

    protected override void Start ()
    {
        base.Start ();
    }

    private void OnJobsChanged (List<Job> jobs, List<Job> oldJobs)
    {
        List<Job> _activeJobs = new List<Job> ();
        List<Job> _openJobs = new List<Job> ();

        for (int i = 0; i < jobs.Count; i++)
        {
            if (jobs[i].cBase != null)
            {
                _activeJobs.Add ( jobs[i] );
            }
            else
            {
                _openJobs.Add ( jobs[i] );
            }
        }

        if (_activeJobs != activeJobs)
        {
            activeJobs = _activeJobs;
            RefreshActive ( activePanel, activeJobs );
        }
        if (_openJobs != openJobs)
        {
            openJobs = _openJobs;
            RefreshOpen ( openPanel, openJobs );
        }

        if (oldJobs != completeJobs)
        {
            completeJobs = oldJobs;
            RefreshComplete ( completePanel, completeJobs );
        }
    }

    private void RefreshActive (GameObject panel, List<Job> jobs)
    {
        for (int i = 0; i < panel.transform.childCount; i++)
        {
            Destroy ( panel.transform.GetChild ( i ).gameObject );
        }

        for (int i = 0; i < jobs.Count; i++)
        {
            int actionIndexer = i;
            GameObject go = Instantiate ( prefab );
            //Debug.Log ( "Adding job " + i + " " + jobs[i].Name );

            Text[] texts = go.GetComponentsInChildren<Text> ();
            Button[] buttons = go.transform.Find("Buttons").GetComponentsInChildren<Button> ();

            texts[0].text = i.ToString("00") + " - " + jobs[i].Name;

            texts[1].text = jobs[i].JobEntity.worldEntity.EntityName;
            texts[1].GetComponent<Button> ()?.onClick.AddListener ( () => { jobs[actionIndexer].JobEntity?.GetComponent<Inspectable> ()?.InspectAndLockCamera (); } );

            texts[2].text = jobs[i].cBase.CitizenFamily.thisMember.fullName;
            texts[2].GetComponent<Button> ()?.onClick.AddListener ( () => { jobs[actionIndexer].cBase.Inspectable.InspectAndLockCamera (); } );

            buttons[0].onClick.AddListener ( () => { JobController.DecreasePriority ( jobs[actionIndexer] ); } );
            buttons[1].onClick.AddListener ( () => { JobController.IncreasePriority ( jobs[actionIndexer] ); } );
            buttons[2].gameObject.SetActive ( false );
            buttons[3].gameObject.SetActive ( false );
            buttons[4].onClick.AddListener ( () => { jobs[actionIndexer].OnCharacterLeave ( "Player Command", true, Job.GetCompletableParams ( Job.CompleteIdentifier.None ) ); } );

            go.transform.SetParent ( panel.transform );
        }
    }

    private void RefreshOpen (GameObject panel, List<Job> jobs)
    {
        for (int i = 0; i < panel.transform.childCount; i++)
        {
            Destroy ( panel.transform.GetChild ( i ).gameObject );
        }

        for (int i = 0; i < jobs.Count; i++)
        {
            int actionIndexer = i;
            GameObject go = Instantiate ( prefab );

            Text[] texts = go.GetComponentsInChildren<Text> ();
            Button[] buttons = go.transform.Find ( "Buttons" ).GetComponentsInChildren<Button> ();

            texts[0].text = i.ToString ( "00" ) + " - " + jobs[i].Name;

            texts[1].text = jobs[i].JobEntity.worldEntity.EntityName;
            texts[1].GetComponent<Button> ()?.onClick.AddListener ( () => { jobs[actionIndexer].JobEntity?.GetComponent<Inspectable> ()?.InspectAndLockCamera (); } );

            if (!openJobs[i].IsCompletable)
            {
                texts[2].text = "Not Completable";
                texts[2].gameObject.AddComponent<Tooltip> ().SetTooltip ( jobs[actionIndexer].IsCompletableReason, HUD_Tooltip_Panel.Tooltip.Preset.Warning );
            }
            else if (!openJobs[i].Open)
            {
                texts[2].text = "Not Open";
                texts[2].gameObject.AddComponent<Tooltip> ().SetTooltip ( "This job is not open. Production may be halted.", HUD_Tooltip_Panel.Tooltip.Preset.Warning );
            }
            else
            {
                texts[2].text = "No Citizen";
                texts[2].gameObject.AddComponent<Tooltip> ().SetTooltip ( "There are no citizens available to do this job. Assign citizens to the appropriate profession.", HUD_Tooltip_Panel.Tooltip.Preset.Warning );
            }

            DestroyImmediate ( texts[2].GetComponent<Button> () );

            buttons[0].onClick.AddListener ( () => { JobController.DecreasePriority ( jobs[actionIndexer] ); } );
            buttons[1].onClick.AddListener ( () => { JobController.IncreasePriority ( jobs[actionIndexer] ); } );
            buttons[3].onClick.AddListener ( () => { JobController.LowestPriority ( jobs[actionIndexer] ); } );
            buttons[2].onClick.AddListener ( () => { JobController.TopPriority ( jobs[actionIndexer] ); } );
            buttons[4].gameObject.SetActive ( false );

            go.transform.SetParent ( panel.transform );
        }        
    }

    private void RefreshComplete (GameObject panel, List<Job> jobs)
    {
        for (int i = 0; i < panel.transform.childCount; i++)
        {
            Destroy ( panel.transform.GetChild ( i ).gameObject );
        }

        for (int i = 0; i < jobs.Count; i++)
        {
            int actionIndexer = i;
            GameObject go = Instantiate ( prefab );

            Text[] texts = go.GetComponentsInChildren<Text> ();
            go.transform.Find ( "Buttons" ).gameObject.SetActive ( false );

            texts[0].text = i.ToString ( "00" ) + " - " + jobs[i].Name;

            texts[1].text = jobs[i].JobEntity.worldEntity.EntityName;
            texts[1].GetComponent<Button> ()?.onClick.AddListener ( () => { jobs[actionIndexer].JobEntity?.GetComponent<Inspectable> ()?.InspectAndLockCamera (); } );

            texts[2].text = "Complete";
            texts[2].GetComponent<Button> ()?.onClick.AddListener ( () => { jobs[actionIndexer].cBase.Inspectable.InspectAndLockCamera (); } );

            go.transform.SetParent ( panel.transform );
        }
    }

    private void RefreshGeneric (GameObject activePanel, List<Job> activeJobs, Text[] texts, Button[] buttons)
    {
        
    }

    private void OnTabChange(GameObject tabObject, string tabName)
    {        
        if (tabName == "Active")
        {

        }
        else if (tabName == "Open")
        {

        }
        else if (tabName == "Complete")
        {

        }
    }
	
}
