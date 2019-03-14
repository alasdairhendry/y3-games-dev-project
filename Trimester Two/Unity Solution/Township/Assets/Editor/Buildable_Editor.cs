using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Buildable))]
public class Buildable_Editor : Editor {

    Buildable targ;

    private void OnEnable ()
    {
        targ = (Buildable)target;
    }

    public override void OnInspectorGUI ()
    {
        base.OnInspectorGUI ();

        List<BuildableStage> children = targ.GetComponentsInChildren<BuildableStage> ().ToList ();
        children = children.OrderBy ( x => x.Stage ).ToList ();

        GUILayout.BeginVertical ( "Box" );
        GUILayout.Label ( "Stages", EditorStyles.boldLabel );
        for (int i = 0; i < children.Count; i++)
        {
            GUILayout.BeginHorizontal ();
            GUILayout.Label ( children[i].name );
            GUILayout.FlexibleSpace ();
            children[i].Stage = EditorGUILayout.IntField ( children[i].Stage );
            GUILayout.EndHorizontal ();
        }
        GUILayout.EndVertical ();

       
    }

}
