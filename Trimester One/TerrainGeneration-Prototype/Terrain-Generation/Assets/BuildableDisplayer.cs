using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildableDisplayer : MonoBehaviour {

    private TextMesh textMesh;    
    private Buildable buildable;

	// Use this for initialization
	void Start () {
        textMesh = GetComponent<TextMesh> ();
        buildable = GetComponentInParent<Buildable> ();	
	}
	
	// Update is called once per frame
	void Update () {
        if (buildable == null) return;
        if (buildable.GetInventory == null) return;
        if (buildable.GetPropData == null) return;

        string text = "";
        text += "Build Percent: " + buildable.ConstructionPercent.ToString ( "00.00" );
        text += "\n\n";

        for (int i = 0; i < buildable.GetPropData.data.requiredMaterials.Count; i++)
        {
            float amount = buildable.GetInventory.inventoryOverall[buildable.GetPropData.data.requiredMaterials[i].id];

            text += ResourceManager.Instance.GetResourceByID ( buildable.GetPropData.data.requiredMaterials[i].id ).name + ": ";
            text += amount + " / " + buildable.GetPropData.data.requiredMaterials[i].amount;
            text += "\n";
        }

        textMesh.text = text;
    }
}
