using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainController))]
[CanEditMultipleObjects]
public class TerrainController_Editor : Editor {

    TerrainController controller;
    TerrainGenerator generator;

    private SerializedProperty autoGenerateMesh;
    private SerializedProperty levelOfDetail;      

    private void OnEnable()
    {
        autoGenerateMesh = serializedObject.FindProperty("autoGenerateMesh");
        levelOfDetail = serializedObject.FindProperty("levelOfDetail");
        generator = controller.GetComponent<TerrainGenerator>();
        Debug.Log(this.name + " enabled");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        controller = (TerrainController)target;

        if (!generator)
        {
            generator = controller.GetComponent<TerrainGenerator>();
        }

        DrawDefaultInspector();

        EditorGUILayout.BeginVertical("Box");

        EditorGUI.indentLevel++;


        for (int i = 0; i < generator.BehaviourGroup.TerrainBehaviours.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label(generator.BehaviourGroup.TerrainBehaviours[i].name);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Activate"))
            {
                generator.BehaviourGroup.TerrainBehaviours[i].Activate(generator.BehaviourGroup, controller.GetComponent<Terrain>(), controller.GetComponent<Terrain>().terrainData);
            }

            EditorGUILayout.EndHorizontal();
        }


        EditorGUI.indentLevel--;

        EditorGUILayout.EndVertical();

        if (GUILayout.Button("Generate Terrain"))
        {
            controller.GenerateTerrain();
        }
        if (GUILayout.Button("Generate Mesh"))
        {
            controller.GenerateMesh();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
