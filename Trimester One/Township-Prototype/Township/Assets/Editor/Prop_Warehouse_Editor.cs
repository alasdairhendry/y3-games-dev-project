using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Prop_Warehouse))]
public class Prop_Warehouse_Editor : Editor {

    Prop_Warehouse t;
    float f = 0.0f;

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

        if (t.inventory == null) return;

        List<int> keys = new List<int> ( t.inventory.inventoryOverall.Keys );

        foreach (int k in keys)
        {
            EditorGUILayout.BeginHorizontal ();

            Resource r = ResourceManager.Instance.GetResourceByID ( k );
            GUILayout.Label ( r.name + ": " + t.inventory.inventoryOverall[k] );

            if (GUILayout.Button ( "Add " + r.name ))
            {
                t.inventory.AddItemQuantity ( k, f );
            }
            else if (GUILayout.Button ( "Remove " + r.name ))
            {
                t.inventory.RemoveItemQuantity ( k, f );
            }
            EditorGUILayout.EndHorizontal ();
        }                
    }
}
