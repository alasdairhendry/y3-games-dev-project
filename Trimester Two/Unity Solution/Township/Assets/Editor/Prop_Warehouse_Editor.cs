using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Prop_Warehouse))]
public class Prop_Warehouse_Editor : Editor {

    Prop_Warehouse t;
    float f = 1.0f;

    private void OnEnable ()
    {
        t = (Prop_Warehouse)target;
    }

    public override void OnInspectorGUI ()
    {
        base.OnInspectorGUI ();
        EditorGUILayout.Space ();

        if (t == null) Debug.LogError ( "oops" );

        f = EditorGUILayout.FloatField ( "Amount to add/remove", f );

        if (WarehouseController.Instance.Inventory == null) return;

        List<int> keys = new List<int> ( WarehouseController.Instance.Inventory.inventoryOverall.Keys );

        foreach (int k in keys)
        {
            EditorGUILayout.BeginHorizontal ();

            Resource r = ResourceManager.Instance.GetResourceByID ( k );
            GUILayout.Label ( r.name + ": " + WarehouseController.Instance.Inventory.inventoryOverall[k] );

            if (GUILayout.Button ( "Add " + r.name ))
            {
                WarehouseController.Instance.Inventory.AddItemQuantity ( k, f );
            }
            else if (GUILayout.Button ( "Remove " + r.name ))
            {
                WarehouseController.Instance.Inventory.RemoveItemQuantity ( k, f );
            }
            EditorGUILayout.EndHorizontal ();
        }                
    }
}
