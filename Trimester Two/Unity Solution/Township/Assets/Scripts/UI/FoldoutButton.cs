using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(EventTrigger))]
public class FoldoutButton : MonoBehaviour {

    [Header("Data")]
    [SerializeField] private string text;
    [SerializeField] private bool generateTopSeperator;
    [SerializeField] private bool animatingChildren;

    [Header("Hotkey")]
    [SerializeField] public Hotkey.Function hotkey;
    [SerializeField] public bool hasHotkey;

    [Header("Children")]
    [SerializeField] private List<Child> children = new List<Child> ();
    [SerializeField] private GameObject childPrefab;
    [SerializeField] private GameObject seperatorPrefab;

    private Button button;
    private GameObject childrenRoot;

    private void Start ()
    {
        if (!string.IsNullOrEmpty ( text ))
            GetComponentInChildren<Text> ().text = text;

        if (hasHotkey)
        {
            transform.Find("HotkeyText").GetComponent<Text>().text = Hotkey.GetData ( hotkey ).GetCommandString ();
        }

        childrenRoot = transform.Find ( "Children" ).gameObject;
        HideChildren ();

        button = GetComponent<Button> ();
        EventTrigger trigger = GetComponent<EventTrigger> ();

        EventTrigger.Entry onEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter }; ;
        onEnter.callback.AddListener ( (eventData) => { ShowChildren (); } );
        trigger.triggers.Add ( onEnter );

        EventTrigger.Entry onExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit }; ;
        onExit.callback.AddListener ( (eventData) => { HideChildren (); } );
        trigger.triggers.Add ( onExit );

        GenerateChildren ();
    }

    private void Update ()
    {
        if (hasHotkey)
        {
            if (Hotkey.GetKeyDown ( hotkey ))
            {
                if (button.onClick != null)
                {
                    button.onClick.Invoke ();
                }
            }
        }

        for (int i = 0; i < children.Count; i++)
        {
            if (!children[i].hasHotkey) continue;
            if (Hotkey.GetKeyDown ( children[i].hotkey ))
            {
                if (children[i].action != null)
                    children[i].action.Invoke ();
            }
        }
    }

    private void GenerateChildren ()
    {
        for (int i = 0; i < children.Count; i++)
        {
            Child _c = children[i];
            if (!string.IsNullOrEmpty(_c.seperatorName))
            {
                GameObject seperator = Instantiate ( seperatorPrefab );
                seperator.transform.SetParent ( transform.Find ( "Children" ) );
                seperator.GetComponent<RectTransform> ().localScale = Vector3.one;
                seperator.GetComponentInChildren<Text> ().text = _c.seperatorName;
            }
            else
            {
                if (i == 0 && generateTopSeperator)
                {
                    GameObject seperator = Instantiate ( seperatorPrefab );
                    seperator.transform.SetParent ( transform.Find ( "Children" ) );
                    seperator.GetComponent<RectTransform> ().localScale = Vector3.one;
                    seperator.GetComponentInChildren<Text> ().text = text;
                }
            }

            GameObject child = Instantiate ( childPrefab );

            child.transform.SetParent ( transform.Find ( "Children" ) );
            child.GetComponent<RectTransform> ().localScale = Vector3.one;
            child.GetComponent<Button> ().onClick.AddListener ( () => { if (_c.action != null) _c.action.Invoke (); } );
            child.GetComponentInChildren<Text> ().text = _c.name;

            if (_c.hasHotkey)
                child.transform.Find ( "HotkeyText" ).GetComponent<Text> ().text = Hotkey.GetData ( _c.hotkey ).GetCommandString ();
        }
    }

    private void ShowChildren ()
    {
        if (animatingChildren) transform.Find ( "Children" ).GetComponent<Animator> ().SetBool ( "isOpen", true );
        else childrenRoot.SetActive ( true );
    }

    private void HideChildren ()
    {
        if (animatingChildren) transform.Find ( "Children" ).GetComponent<Animator> ().SetBool ( "isOpen", false );
        else childrenRoot.SetActive ( false );
    }

    [System.Serializable]
    public class Child
    {
        public string name;
        public string seperatorName;
        public UnityEvent action = new UnityEvent();
        public Hotkey.Function hotkey;
        public bool hasHotkey;
    }
}
