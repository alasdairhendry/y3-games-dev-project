using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor ( typeof ( CitizenBase ) )]
public class CitizenBase_Editor : Editor
{
    CitizenBase t;
    float f = 0.0f;

    private void OnEnable ()
    {
        t = (CitizenBase)target;
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
    }
}
