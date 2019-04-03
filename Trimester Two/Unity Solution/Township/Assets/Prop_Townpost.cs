using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop_Townpost : Prop {

    protected override void Start ()
    {
        TextMesh townName = transform.Find ( "Graphics" ).Find ( "Stage_Complete" ).Find ( "TownName_Text" ).GetComponent<TextMesh> ();
        TextMesh settled = transform.Find ( "Graphics" ).Find ( "Stage_Complete" ).Find ( "Settled_Text" ).GetComponent<TextMesh> ();
        TextMesh pop = transform.Find ( "Graphics" ).Find ( "Stage_Complete" ).Find ( "Population_Text" ).GetComponent<TextMesh> ();
    }

    protected override void OnPlaced ()
    {
        base.OnPlaced ();

        TextMesh townName = transform.Find ( "Graphics" ).Find ( "Stage_Complete" ).Find ( "TownName_Text" ).GetComponent<TextMesh> ();
        townName.text = GameData.Instance.CurrentSaveFileName.ToUpper ();

        TextMesh settled = transform.Find ( "Graphics" ).Find ( "Stage_Complete" ).Find ( "Settled_Text" ).GetComponent<TextMesh> ();
        settled.text = "SETTLED " + GameData.Instance.GetSaveData.created.Year.ToString ( "0000" );

        TextMesh pop = transform.Find ( "Graphics" ).Find ( "Stage_Complete" ).Find ( "Population_Text" ).GetComponent<TextMesh> ();
        pop.text = "POP. " + CitizenController.Instance.activeCitizens.Count.ToString ( "00" );

        TextSize ts = new TextSize ( townName );
        ts.SetMaxWidth ( 2.9f );

        float settledPosX = (ts.width / 2.0f) / 1.25f;
        settledPosX = Mathf.Clamp ( settledPosX, 0.616f, 1.16f );
        settled.transform.localPosition = new Vector3 ( settledPosX, settled.transform.localPosition.y, settled.transform.localPosition.z );

        float popPosX = (-ts.width / 2.0f) / 1.25f;
        popPosX = Mathf.Clamp ( popPosX, -1.16f, -0.616f );
        pop.transform.localPosition = new Vector3 ( popPosX, pop.transform.localPosition.y, pop.transform.localPosition.z );

        CitizenController.Instance.onCitizenBornOrDied += UpdatePopulation;
        PropManager.Instance.GetPropDataByName ( "Warehouse" ).Unlock ();

        SetInspectable ();
    }

    private void UpdatePopulation (CitizenBase cBase, bool born)
    {
        transform.Find ( "Graphics" ).Find ( "Stage_Complete" ).Find ( "Population_Text" ).GetComponent<TextMesh> ().text = "POP. " + CitizenController.Instance.activeCitizens.Count.ToString ( "00" );
    }

    public override void DestroyProp ()
    {
        CitizenController.Instance.onCitizenBornOrDied -= UpdatePopulation;
        base.DestroyProp ();
    }
}
