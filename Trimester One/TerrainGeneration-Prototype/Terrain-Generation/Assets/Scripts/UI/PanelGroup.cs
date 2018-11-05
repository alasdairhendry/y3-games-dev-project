using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelGroup : MonoBehaviour {

    Dictionary<int, List<UIPanel>> panelGroups = new Dictionary<int, List<UIPanel>> ();
    //private UIPanel defaultPanel;

    private void Awake ()
    {
        FindGroups ();
    }

    private void FindGroups ()
    {
        UIPanel[] panels = GetComponentsInChildren<UIPanel> ();
        
        for (int i = 0; i < panels.Length; i++)
        {
            KeyValuePair<int, bool> details = panels[i].GroupDetails;
            Debug.Log ( panels[i].gameObject.name + ": " +  details );

            if (panelGroups.ContainsKey ( details.Key ))
            {
                panelGroups[details.Key].Add ( panels[i] );
            }
            else
            {
                panelGroups.Add ( details.Key, new List<UIPanel> () { panels[i] } );
            }
        }
    }

    public void SetActivePanel(UIPanel panel)
    {
        List<UIPanel> panels = panelGroups[panel.GroupDetails.Key];

        for (int i = 0; i < panels.Count; i++)
        {
            if(panel != panels[i])
            {
                panels[i].Hide ();
            }
        }
    }

    public void RemoveActivePanel(UIPanel panel)
    {
        List<UIPanel> panels = panelGroups[panel.GroupDetails.Key];

        for (int i = 0; i < panels.Count; i++)
        {
            if (panels[i].GroupDetails.Value)
                panels[i].Show ();
            else
                panels[i].Hide ();
        }
    }
}
