using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor ( typeof ( WarehouseController ) )]
public class WarehouseController_Editor : Editor
{

    WarehouseController t;
    float f = 1.0f;

    private void OnEnable ()
    {
        t = (WarehouseController)target;
    }

    public override void OnInspectorGUI ()
    {
        base.OnInspectorGUI ();
        EditorGUILayout.Space ();

        if (t == null) Debug.LogError ( "oops" );

        f = EditorGUILayout.FloatField ( "Amount to add/remove", f );

        if (t.Inventory == null) return;

        List<int> keys = new List<int> ( t.Inventory.inventoryOverall.Keys );

        foreach (int k in keys)
        {
            EditorGUILayout.BeginHorizontal ();

            Resource r = ResourceManager.Instance.GetResourceByID ( k );
            GUILayout.Label ( r.name + ": " + t.Inventory.inventoryOverall[k] );

            if (GUILayout.Button ( "Add " + r.name ))
            {
                t.Inventory.AddItemQuantity ( k, f );
            }
            else if (GUILayout.Button ( "Remove " + r.name ))
            {
                t.Inventory.RemoveItemQuantity ( k, f );
            }
            EditorGUILayout.EndHorizontal ();
        }

        EditorGUILayout.Space ();
        EditorGUILayout.Space ();

        EditorGUILayout.BeginHorizontal ();

        GUILayout.Label ( "All" );

        if (GUILayout.Button ( "Add All" ))
        {
            for (int i = 0; i < ResourceManager.Instance.GetResourceList ().Count; i++)
            {
                t.Inventory.AddItemQuantity ( ResourceManager.Instance.GetResourceList ()[i].id, f );
            }
        }
        else if (GUILayout.Button ( "Remove All" ))
        {
            for (int i = 0; i < ResourceManager.Instance.GetResourceList ().Count; i++)
            {
                t.Inventory.RemoveItemQuantity ( ResourceManager.Instance.GetResourceList ()[i].id, f );
            }
        }
        EditorGUILayout.EndHorizontal ();
    }
}
