using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public static UIController Instance;
    [SerializeField] private List<UIEnumValue> elements = new List<UIEnumValue> ();
    private GameObject previousGameObject;

    private void Awake ()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy ( this.gameObject );
    }

    private void Update ()
    {
        if (previousGameObject != null)
        {
            if (previousGameObject == EventSystem.current.currentSelectedGameObject) return;
        }
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            previousGameObject = EventSystem.current.currentSelectedGameObject;

            if (EventSystem.current.currentSelectedGameObject.GetComponent<Button> () != null)
                EventSystem.current.SetSelectedGameObject ( null );
        }
    }

    public GameObject SpawnUI(UIEnumValue.Type type, Transform parent)
    {
        for (int i = 0; i < elements.Count; i++)
        {
            if(elements[i].type == type)
            {
                GameObject go = Instantiate ( elements[i].prefab );
                go.transform.SetParent ( parent );
                return go;
            }
        }

        Debug.LogError ( "Nothing Created In Inspector" );
        return null;
    }

    [System.Serializable]
    public class UIEnumValue
    {
        public enum Type { KeyValueText, KeyValueInput, KeyValueColour, LongButton, Tab, Dropdown };
        public Type type;
        public GameObject prefab;
    }

}
