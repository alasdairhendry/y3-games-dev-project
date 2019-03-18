using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour {

    [SerializeField] private List<Group> tabs = new List<Group> ();
    [SerializeField] [Header ( "ActiveColours" )] private ColorBlock activeColours;
    [SerializeField] [Header ( "InactiveColours" )] private ColorBlock inactiveColours;

    public System.Action<GameObject, string> onTabChanged;
    private Group activeTab;

    private void Start ()
    {
        if (tabs.Count == 0) { Debug.LogError ( "NO TABS SETUP IN INSPECTOR", this.gameObject ); return; }

        ConfigureTabs ();
        ChangeTab ( tabs[0] );
    }

    private void ConfigureTabs ()
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            Group t = tabs[i];
            tabs[i].tabButton.onClick.AddListener ( () => { ChangeTab ( t ); } );
            SetInactive ( tabs[i] );
        }
    }

    private void ChangeTab (Group newTab)
    {
        if(activeTab != null)
        {
            SetInactive ( activeTab );
        }

        activeTab = newTab;
        activeTab.tabButton.colors = activeColours;
        activeTab.tabButton.interactable = false;

        activeTab.target.SetActive ( true );

        if (onTabChanged != null) onTabChanged ( activeTab.tabButton.gameObject, activeTab.tabButton.GetComponentInChildren<Text> ().text );
    }

    private void SetInactive(Group activeTab)
    {
        activeTab.tabButton.colors = inactiveColours;
        activeTab.tabButton.interactable = true;

        activeTab.target.SetActive ( false );
    }

    [System.Serializable]
    public class Group
    {
        public Button tabButton;
        public GameObject target;
    }
}
