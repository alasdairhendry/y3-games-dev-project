using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainBehaviourGroup))]
public class TerrainBehaviour_Editor : Editor {

    TerrainBehaviourGroup targ;

    SerializedProperty list;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        targ = (TerrainBehaviourGroup)target;

        base.DrawDefaultInspector();

        list = serializedObject.FindProperty("terrainBehaviours");

        EditorGUILayout.PropertyField(list, true);

        for (int i = 0; i < targ.TerrainBehaviours.Count; i++)
        {
            //SerializedProperty listBehaviour = list.FindPropertyRelative("terrainBehaviour");
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label(new GUIContent(i.ToString()));

            SerializedProperty e = list.GetArrayElementAtIndex(i).FindPropertyRelative("enabled");
            SerializedProperty b = list.GetArrayElementAtIndex(i).FindPropertyRelative("terrainBehaviour");            

            EditorGUILayout.PropertyField(e, new GUIContent(""));
            EditorGUILayout.PropertyField(b, new GUIContent(""));

            if (GUILayout.Button("^"))
            {
                if(i != 0)
                {
                    TerrainBehaviourLayer tbl = targ.terrainBehaviours[i];

                    targ.terrainBehaviours.RemoveAt(i);
                    targ.terrainBehaviours.Insert(i - 1, tbl);
                }
            }

            if (GUILayout.Button("X"))
            {
                targ.terrainBehaviours.RemoveAt(i);
            }


            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("+"))
        {
            targ.terrainBehaviours.Add(new TerrainBehaviourLayer(null, true));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
