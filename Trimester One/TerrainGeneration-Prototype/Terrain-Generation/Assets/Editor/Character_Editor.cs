using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor ( typeof ( Character ) )]
public class Character_Editor : Editor
{
    Character t;
    float f = 0.0f;

    private void OnEnable ()
    {
        t = (Character)target;
    }

    public override void OnInspectorGUI ()
    {
        base.OnInspectorGUI ();
        EditorGUILayout.Space ();

        if (t == null) Debug.LogError ( "oops" );

        f = EditorGUILayout.FloatField ( "Amount to add/remove", f );

        if (t.inventory == null) return;

        List<int> keys = new List<int> ( t.inventory.inventory.Keys );

        foreach (int k in keys)
        {
            EditorGUILayout.BeginHorizontal ();

            Resource r = ResourceManager.Instance.GetResourceByID ( k );
            GUILayout.Label ( r.name + ": " + t.inventory.inventory[k] );

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
