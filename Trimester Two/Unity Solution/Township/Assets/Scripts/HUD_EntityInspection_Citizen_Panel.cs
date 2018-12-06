using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_EntityInspection_Citizen_Panel : UIPanel {

    //private Character citizen;
    private GameObject target;

    [SerializeField] private Transform leftHorizontalView;
    [SerializeField] private Transform rightHorizontalView;

    [SerializeField] private GameObject keyValueText_Prefab;    

    public void ShowPanel (GameObject target)
    {
        this.target = target;
        FindObjectOfType<HUD_EntityInspection_Panel> ().ShowPanel ( HUD_EntityInspection_Panel.InspectionType.Citizen );
    }   

    public override void Show ()
    {
        base.Show ();
        ClearPanel ();
        GameTime.RegisterGameTick ( Tick_ValidityCheck );
    }

    protected override void SetAnchoredPosition ()
    {        
        if (target == null) return;

        Vector3 citizenScreenPosition = Camera.main.WorldToScreenPoint ( target.transform.position );
        
        base.targetAnchorOffset = new Vector3 ( 32.0f, -32.0f, 0.0f );
        base.targetAnchoredPosition = citizenScreenPosition;
    }

    public void AddTextData(System.Func<string> action, string key)
    {
        UIController.Instance.SpawnUI(UIController.UIEnumValue.Type.KeyValueText, leftHorizontalView).GetComponent<KeyValueUIPair> ().SetData ( action, key );        
    }

    public void AddButtonData(System.Action action, string key)
    {
        GameObject go = UIController.Instance.SpawnUI ( UIController.UIEnumValue.Type.LongButton, rightHorizontalView );
        go.GetComponent<Button> ().onClick.AddListener ( () => { action (); } );
        go.GetComponentInChildren<Text> ().text = key;
    }

    private void ClearPanel ()
    {        
        for (int i = 0; i < leftHorizontalView.childCount; i++)
        {
            Destroy ( leftHorizontalView.GetChild ( i ).gameObject );
        }

        for (int i = 0; i < rightHorizontalView.childCount; i++)
        {
            Destroy ( rightHorizontalView.GetChild ( i ).gameObject );
        }
    }

    private void Tick_ValidityCheck (int relativeTick)
    {
        if (target == null) { ClearPanel (); Hide (); }
    }

    public override void Hide ()
    {
        base.Hide ();
        FindObjectOfType<HUD_EntityInspection_Panel> ().Hide ();
        GameTime.UnRegisterGameTick ( Tick_ValidityCheck );
    }
}
