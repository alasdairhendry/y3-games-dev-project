using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGenerator_Editor : Editor {

    private bool worldDataFoldout = false;
    private bool noiseDataFoldout = false;
    private bool textureDataFoldout = false;

    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target;

        if (DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
            {
                mapGen.GenerateMap();
                Debug.Log("Generate");
            }
        }

        QuickSwitch.Draw<WorldData>(mapGen.GenerateMap, serializedObject, "worldData", ref mapGen.worldData, ref worldDataFoldout);
        QuickSwitch.Draw<NoiseData>(mapGen.GenerateMap, serializedObject, "noiseData", ref mapGen.noiseData, ref noiseDataFoldout);
        QuickSwitch.Draw<TextureData>(mapGen.GenerateMap, serializedObject, "textureData", ref mapGen.textureData, ref textureDataFoldout);

        if (GUILayout.Button("Generate"))
        {
            mapGen.GenerateMap();
        }
    }

    //private bool Check(System.Action field)
    //{
    //    EditorGUI.BeginChangeCheck();
    //    field();
    //    if (EditorGUI.EndChangeCheck())
    //    {
    //        return true;
    //    }

    //    return false;
    //}

    //private void DrawQuickSwitches<T>(MapGenerator mapGen, T[] types, ref bool foldOut, string foldOutLabel, SerializedProperty property, ref T target) where T : ScriptableObject
    //{
    //    EditorGUILayout.BeginVertical("Box");
    //    if (Check(() => { EditorGUILayout.PropertyField(property, new GUIContent(foldOutLabel)); }))
    //    {
    //        mapGen.GenerateMap();
    //    }

    //    EditorGUI.indentLevel++;
    //    foldOut = EditorGUILayout.Foldout(foldOut, new GUIContent("Quick Switch"));
    //    if (foldOut)
    //    {
    //        EditorGUI.indentLevel++;
    //        for (int i = 0; i < types.Length; i++)
    //        {
    //            if (types[i].name == target.name) continue;

    //            if (GUILayout.Button(types[i].name))
    //            {
    //                target = types[i];
    //                mapGen.GenerateMap();
    //            }
    //        }
    //        EditorGUI.indentLevel--;
    //    }
    //    EditorGUI.indentLevel--;
    //    EditorGUILayout.EndVertical();
    //}

    //private T[] DisplaySOTypes<T>() where T : ScriptableObject
    //{
    //    string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
    //    T[] t = new T[guids.Length];

    //    for (int i = 0; i < t.Length; i++)
    //    {
    //        string path = AssetDatabase.GUIDToAssetPath(guids[i]);
    //        t[i] = AssetDatabase.LoadAssetAtPath<T>(path);
    //    }

    //    return t;
    //}
}
