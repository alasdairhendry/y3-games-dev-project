using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Canvas_HideAll))]
public class Canvas_HideAll_Editor : Editor {

    Canvas_HideAll t;

    public override void OnInspectorGUI ()
    {
        base.OnInspectorGUI ();
        t = (Canvas_HideAll)target;

        if (GUILayout.Button ( "Show/Hide" ))
        {
            t.ShowHide ();
        }

    }
}
