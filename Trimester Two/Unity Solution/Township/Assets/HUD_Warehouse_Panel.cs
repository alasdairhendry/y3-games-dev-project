using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Warehouse_Panel : UIPanel {

    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform content;

    private Dictionary<int, Text> entries = new Dictionary<int, Text> ();

    protected override void Start ()
    {
        base.Start ();

        List<Resource> r = ResourceManager.Instance.GetResourceList ();

        for (int i = 0; i < r.Count; i++)
        {
            GameObject go = Instantiate ( prefab );
            go.transform.SetParent ( content );

            go.GetComponentsInChildren<Image> ()[2].sprite = r[i].image;
            go.GetComponentsInChildren<Text> ()[0].text = r[i].name;
            go.GetComponentsInChildren<Text> ()[1].text = WarehouseController.Instance.Inventory.GetAvailableQuantity ( i ).ToString ( "00" );
            entries.Add ( i, go.GetComponentsInChildren<Text> ()[1] );
        }

        WarehouseController.Instance.Inventory.RegisterOnResourceAdded ( OnResourceAdded );
        WarehouseController.Instance.Inventory.RegisterOnResourceRemoved( OnResourceRemoved);
    }

    private void OnResourceAdded (int id, float q)
    {
        entries[id].text = WarehouseController.Instance.Inventory.GetAvailableQuantity ( id ).ToString ("00");
    }

    private void OnResourceRemoved (int id, float q)
    {
        entries[id].text = WarehouseController.Instance.Inventory.GetAvailableQuantity ( id ).ToString ( "00" );
    }

    public override void Show ()
    {
        base.Show ();
    }

    public override void Hide ()
    {
        base.Hide ();
    }
}
