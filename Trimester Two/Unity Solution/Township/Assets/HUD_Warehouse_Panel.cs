using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Warehouse_Panel : UIPanel {

    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform content;

    private Dictionary<int, TextMeshProUGUI> entries = new Dictionary<int, TextMeshProUGUI> ();

    protected override void Start ()
    {
        base.Start ();

        List<Resource> r = ResourceManager.Instance.GetResourceList ();

        for (int i = 0; i < r.Count; i++)
        {
            GameObject go = Instantiate ( prefab );
            go.transform.SetParent ( content );

            go.GetComponentsInChildren<Image> ()[2].sprite = r[i].image;
            go.GetComponentsInChildren<TextMeshProUGUI> ()[0].text = r[i].name;
            go.GetComponentsInChildren<TextMeshProUGUI> ()[1].text = Mathf.Floor ( WarehouseController.Instance.Inventory.GetAvailableQuantity ( i ) ).ToString ( "00" );
            entries.Add ( i, go.GetComponentsInChildren<TextMeshProUGUI> ()[1] );
        }

        WarehouseController.Instance.Inventory.RegisterOnResourceAdded ( OnResourceAdded );
        WarehouseController.Instance.Inventory.RegisterOnResourceRemoved( OnResourceRemoved);
    }

    private void OnResourceAdded (int id, float q)
    {
        entries[id].text = Mathf.Floor ( WarehouseController.Instance.Inventory.GetAvailableQuantity ( id ) ).ToString ( "00" );
    }

    private void OnResourceRemoved (int id, float q)
    {
        entries[id].text = Mathf.Floor ( WarehouseController.Instance.Inventory.GetAvailableQuantity ( id ) ).ToString ( "00" );
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
