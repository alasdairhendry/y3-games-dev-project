using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_EntityInspection_Citizen_Panel : UIPanel {

    //private Character citizen;

    [SerializeField] private Transform leftHorizontalView;
    [SerializeField] private Transform rightHorizontalView;

    [SerializeField] private GameObject keyValueText_Prefab;    

    public void ShowPanel (Character citizen)
    {
        if (base.active) return;
        this.citizen = citizen;
        FindObjectOfType<HUD_EntityInspection_Panel> ().ShowPanel ( HUD_EntityInspection_Panel.InspectionType.Citizen );
    }   

    public override void Show ()
    {
        base.Show ();
        AddTextData ();
        AddButtonData ();
        GameTime.RegisterGameTick ( Tick_ValidityCheck );
    }

    protected override void SetAnchoredPosition ()
    {
        Vector3 citizenScreenPosition = Camera.main.WorldToScreenPoint ( citizen.transform.position );
        
        base.targetAnchorOffset = new Vector3 ( 32.0f, -32.0f, 0.0f );
        base.targetAnchoredPosition = citizenScreenPosition;
    }

    private void AddTextData ()
    {
        AddTextData ( () => {

            if (citizen == null) return "None";

            if (citizen.GetCurrentJob == null) return "None";
            else return citizen.GetCurrentJob.Name;

        }, "Job", leftHorizontalView );

        AddTextData ( () => {

            if (citizen == null) return "Idling";

            if (citizen.GetCurrentJob == null) return "Idling";
            else return citizen.GetCurrentJob.AgentJobStatus;

        }, "Status", leftHorizontalView );

    }

    private void AddButtonData ()
    {
        AddButtonData ( () =>
        {
            Debug.Log ( "Woop" );
            if (citizen == null) return;

            if (citizen.GetCurrentJob == null) return;
            else citizen.GetCurrentJob.OnCharacterLeave ( "User Left" );
        }, "Cancel Job", rightHorizontalView );

        AddButtonData ( () =>
        {
            Debug.Log ( "Woop2" );
            if (citizen == null) return;

            if (citizen.GetCurrentJob != null) citizen.GetCurrentJob.OnCharacterLeave ( "Citizen Died" );

            Destroy ( citizen.gameObject );

        }, "Kill Citizen", rightHorizontalView );
    }

    private void AddTextData(System.Func<string> action, string key, Transform parent)
    {
        UIController.Instance.SpawnUI(UIController.UIEnumValue.Type.KeyValueText, parent).GetComponent<KeyValueUIPair> ().SetData ( action, key );        
    }

    private void AddButtonData(System.Action action, string key, Transform parent)
    {
        GameObject go = UIController.Instance.SpawnUI ( UIController.UIEnumValue.Type.LongButton, parent );
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

    private void Tick_ValidityCheck ()
    {
        if (citizen == null) { ClearPanel (); Hide (); }
    }

    public override void Hide ()
    {
        base.Hide ();
        GameTime.UnRegisterGameTick ( Tick_ValidityCheck );
    }
}
