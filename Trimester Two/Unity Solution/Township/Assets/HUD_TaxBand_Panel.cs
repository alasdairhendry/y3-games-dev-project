using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD_TaxBand_Panel : UIPanel {

    [Header ( "Tax Band" )]
    [SerializeField] private GameObject globalObject;
    [SerializeField] private GameObject childrenObject;
    [SerializeField] private GameObject studentsObject;
    [SerializeField] private GameObject workersObject;
    [SerializeField] private GameObject gatherersObject;
    [SerializeField] private GameObject producersObject;

    private System.Action UpdateLocalObjectsAction;
    private System.Action UpdateGlobalObjectAction;

    protected override void Start ()
    {
        base.Start ();

        CreateObjects ();
    }

    private void CreateObjects ()
    {
        SetupLocalTaxObject (  childrenObject, TaxController.Instance.GetTaxBand ( "Children" ) );
        SetupLocalTaxObject (  studentsObject, TaxController.Instance.GetTaxBand ( "Students" ) );
        SetupLocalTaxObject (  workersObject, TaxController.Instance.GetTaxBand ( "Workers" ) );
        SetupLocalTaxObject (  gatherersObject, TaxController.Instance.GetTaxBand ( "Gatherers" ) );
        SetupLocalTaxObject (  producersObject, TaxController.Instance.GetTaxBand ( "Producers" ) );

        SetupGlobalTaxObject ();

        UpdateLocalObjectsAction += () => { UpdateGlobalObject (); };
        UpdateAllLocalObjects ();
    }

    private void SetupLocalTaxObject( GameObject obj, TaxController.TaxBand taxBand)
    {
        Slider slider = obj.transform.Find ( "Slider" ).GetComponent<Slider> ();
        TextMeshProUGUI textPercentage = obj.transform.Find ( "Percentage" ).GetComponentInChildren<TextMeshProUGUI> ();
        TextMeshProUGUI textMoney = obj.transform.Find ( "Money" ).GetComponentInChildren<TextMeshProUGUI> ();

        slider.minValue = TaxController.Instance.MinLocalModifier;
        slider.maxValue = TaxController.Instance.MaxLocalModifier;

        slider.onValueChanged.AddListener ( (f) =>
        {
            TaxController.Instance.SetTaxBandModifier ( taxBand, f );
            UpdateLocalObject ( taxBand,  slider,  textMoney,  textPercentage );
        } );

        obj.transform.Find ( "Refresh_Button" ).GetComponent<Button> ().onClick.AddListener ( () =>
        {
            taxBand.ResetModifier ();
            UpdateLocalObject ( taxBand,  slider,  textMoney,  textPercentage );
        } );

        UpdateLocalObjectsAction += () => { UpdateLocalObject ( taxBand,  slider,  textMoney,  textPercentage ); };
        //UpdateLocalObject ( taxBand,  slider,  textMoney,  textPercentage );
    }

    private void UpdateLocalObject (TaxController.TaxBand taxBand,  Slider slider,  TextMeshProUGUI textMoney,  TextMeshProUGUI textPercentage)
    {
        textPercentage.text = (taxBand.Modifier * 100.0f).ToString ( "00" ) + "%";
        textMoney.text = TaxController.Instance.GetRequiredTaxWithGlobalModifier ( taxBand.professions[0] ).ToString ( "00.00" );
        slider.value = taxBand.Modifier;

        UpdateGlobalObject ();
    }

    private void SetupGlobalTaxObject ()
    {
        Slider slider = globalObject.transform.Find ( "Slider" ).GetComponent<Slider> ();
        TextMeshProUGUI textPercentage = globalObject.transform.Find ( "Percentage" ).GetComponentInChildren<TextMeshProUGUI> ();
        TextMeshProUGUI textMoney = globalObject.transform.Find ( "Money" ).GetComponentInChildren<TextMeshProUGUI> ();

        slider.minValue = TaxController.Instance.MinGlobalModifier;
        slider.maxValue = TaxController.Instance.MaxGlobalModifier;

        UpdateGlobalObjectAction = () => {
            slider.value = TaxController.Instance.GlobalTaxModifier;
            textPercentage.text = (TaxController.Instance.GlobalTaxModifier * 100.0f).ToString ( "00" ) + "%";
            textMoney.text = TaxController.Instance.GetIncomeDailyTax().ToString ( "00.00" );
        };

        slider.onValueChanged.AddListener ( (f) =>
        {
            TaxController.Instance.SetGlobalModifier ( f );
            UpdateGlobalObject ();
        } );

        globalObject.transform.Find ( "Refresh_Button" ).GetComponent<Button> ().onClick.AddListener ( () =>
        {
            TaxController.Instance.ResetGlobalModifier ();
            UpdateGlobalObject ();
        } );
        //UpdateGlobalObject ();
    }

    private void UpdateGlobalObject ()
    {
        UpdateGlobalObjectAction?.Invoke ();
    }

    private void UpdateAllLocalObjects ()
    {
        UpdateLocalObjectsAction?.Invoke ();
    }

    public override void Show ()
    {
        base.Show ();

        UpdateAllLocalObjects ();
        ProfessionController.Instance.onRefreshProfessions += UpdateAllLocalObjects;
        CitizenController.Instance.onCitizenBornOrDied += (cBase, b) => { UpdateAllLocalObjects (); };
    }

    public override void Hide ()
    {
        base.Hide ();
        ProfessionController.Instance.onRefreshProfessions -= UpdateAllLocalObjects;
        CitizenController.Instance.onCitizenBornOrDied -= (cBase, b) => { UpdateAllLocalObjects (); Debug.Log ( "Woop" ); };
    }

}
