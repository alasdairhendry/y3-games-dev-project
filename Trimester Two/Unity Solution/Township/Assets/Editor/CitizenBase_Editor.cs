using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor ( typeof ( CitizenBase ) )]
public class CitizenBase_Editor : Editor
{
    CitizenBase cBase;
    float f = 1.0f;

    private void OnEnable ()
    {
        cBase = (CitizenBase)target;
    }

    public override void OnInspectorGUI ()
    {
        base.OnInspectorGUI ();
        EditorGUILayout.Space ();

        if (cBase == null) return;

        Details ();
        Inventory ();
    }

    private void Details ()
    {
        if (cBase.CitizenJob == null) return;
        if (cBase.CitizenJob.GetCurrentJob == null) { GUILayout.Label ( "No Job" ); }
        else { GUILayout.Label ( "Job: " + cBase.CitizenJob.GetCurrentJob.Name ); }
    }

    private void Inventory ()
    {
        if (cBase.Inventory == null) return;
        f = EditorGUILayout.FloatField ( "Amount to add/remove", f );

        List<int> keys = new List<int> ( cBase.Inventory.inventoryOverall.Keys );

        foreach (int k in keys)
        {
            EditorGUILayout.BeginHorizontal ();

            Resource r = ResourceManager.Instance.GetResourceByID ( k );
            GUILayout.Label ( r.name + ": " + cBase.Inventory.inventoryOverall[k] );

            if (GUILayout.Button ( "Add " + r.name ))
            {
                cBase.Inventory.AddItemQuantity ( k, f );
            }
            else if (GUILayout.Button ( "Remove " + r.name ))
            {
                cBase.Inventory.RemoveItemQuantity ( k, f );
            }
            EditorGUILayout.EndHorizontal ();
        }
    }
}
