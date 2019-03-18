using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ColourGroupController))]
public class ColourGroupController_Editor : Editor {

    ColourGroupController t;

    private void OnEnable ()
    {
        t = (ColourGroupController)target;
    }

    public override void OnInspectorGUI ()
    {
        base.OnInspectorGUI ();

        EditorGUILayout.Space ();

        EditorGUILayout.BeginVertical ( "Box" );

        for (int i = 0; i < t.Group.group.Count; i++)
        {
            EditorGUILayout.BeginHorizontal ();

            t.Group.group[i].name = EditorGUILayout.TextField ( t.Group.group[i].name );


            t.Group.group[i].colour = EditorGUILayout.ColorField ( t.Group.group[i].colour );


            if (GUILayout.Button ( "x" ))
            {
                t.Group.group.RemoveAt ( i );

            }
            EditorGUILayout.EndHorizontal ();
        }

        if (GUILayout.Button ( "Add" ))
        {
            t.Group.group.Add ( new ScriptableColourGroup.ColourGroup ( "New", Color.white ) );

        }

        EditorGUILayout.EndVertical ();
        EditorUtility.SetDirty ( t );
    }
}
