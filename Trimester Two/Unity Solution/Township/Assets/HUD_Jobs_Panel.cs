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
        //List<Job> _completeJobs = new List<Job> ();

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
            Debug.Log ( "Active jobs different" );
            activeJobs = _activeJobs;
            RefreshActive ();
        }
        if (_openJobs != openJobs)
        {
            Debug.Log ( "Open jobs different" );
            openJobs = _openJobs;
            RefreshOpen ();
        }

        if (oldJobs != completeJobs)
        {
            Debug.Log ( "Complete jobs different" );
            completeJobs = oldJobs;
            RefreshComplete ();
        }
    }

    private void RefreshActive ()
    {
        for (int i = 0; i < activePanel.transform.childCount; i++)
        {
            Destroy ( activePanel.transform.GetChild ( i ).gameObject );
        }

        for (int i = 0; i < activeJobs.Count; i++)
        {
            GameObject go = Instantiate ( prefab );
            go.GetComponentsInChildren<Text> ()[0].text = activeJobs[i].Name;
            go.GetComponentsInChildren<Text> ()[1].text = activeJobs[i].JobEntity.gameObject.name;
            go.GetComponentsInChildren<Text> ()[2].text = activeJobs[i].cBase.CitizenFamily.thisMember.firstName;
            go.transform.SetParent ( activePanel.transform );
        }
    }

    private void RefreshOpen ()
    {
        for (int i = 0; i < openPanel.transform.childCount; i++)
        {
            Destroy ( openPanel.transform.GetChild ( i ).gameObject );
        }

        for (int i = 0; i < openJobs.Count; i++)
        {
            GameObject go = Instantiate ( prefab );
            go.GetComponentsInChildren<Text> ()[0].text = openJobs[i].Name;
            go.GetComponentsInChildren<Text> ()[1].text = openJobs[i].JobEntity.gameObject.name;

            if (!openJobs[i].IsCompletable)
                go.GetComponentsInChildren<Text> ()[2].text = "Not Completable";
            else if (!openJobs[i].Open)
                go.GetComponentsInChildren<Text> ()[2].text = "Not Open";
            else 
                go.GetComponentsInChildren<Text> ()[2].text = "No Citizen";

            go.transform.SetParent ( openPanel.transform );
        }
    }

    private void RefreshComplete ()
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
