using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenTaxing : MonoBehaviour {

    public CitizenBase cBase { get; protected set; }

    private void Awake ()
    {
        cBase = GetComponent<CitizenBase> ();
    }

    private void Start () {
        GameTime.onDayChanged += OnDayChanged;
	}
	
    private void OnDayChanged(int p, int c)
    {
        MoneyController.Instance.AddWithTransaction ( Mathf.RoundToInt ( TaxController.Instance.GetRequiredTax ( cBase.CitizenJob.profession ) ), "Citizen " + TaxController.Instance.GetTaxBand(cBase.CitizenJob.profession).BandName + " Tax", MoneyController.Transaction.Category.Tax );
    }

    private void OnDestroy ()
    {
        GameTime.onDayChanged -= OnDayChanged;
    }
}
