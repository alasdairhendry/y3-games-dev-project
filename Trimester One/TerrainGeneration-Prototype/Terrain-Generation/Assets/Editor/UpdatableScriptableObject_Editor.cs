using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UpdatableScriptableObject), true)]
public class UpdatableScriptableObject_Editor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        UpdatableScriptableObject data = (UpdatableScriptableObject)target;

        //if (!data.autoUpdate)
        //{
            if (GUILayout.Button("Manual Update"))
            {
                data.NotifyOfUpdatedValues();                
            }
        //}
    }
}
