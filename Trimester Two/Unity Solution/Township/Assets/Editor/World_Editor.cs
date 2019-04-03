using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(World))]
public class World_Editor : Editor {

    private bool worldDataFoldout = false;
    private bool noiseDataFoldout = false;
    private bool textureDataFoldout = false;
    private bool biomeFoldout = false;
    private bool envFoldout = false;

    public override void OnInspectorGUI()
    {
        World world = (World)target;

        base.OnInspectorGUI();

        QuickSwitch.Draw<WorldData>(null, serializedObject, "worldData", ref world.worldData, ref worldDataFoldout);
        QuickSwitch.Draw<NoiseData>(null, serializedObject, "noiseData", ref world.noiseData, ref noiseDataFoldout);
        QuickSwitch.Draw<TextureData>(null, serializedObject, "textureData", ref world.textureData, ref textureDataFoldout);
        QuickSwitch.Draw<BiomeData>(null, serializedObject, "biomeData", ref world.biomeData, ref biomeFoldout);
        QuickSwitch.Draw<EnvironmentData>(null, serializedObject, "environmentData", ref world.environmentData, ref envFoldout);

        //if(Input.GetKey(KeyCode.LeftShift) && Input.GetKey ( KeyCode.A )) { world.DEBUG_UpdateShaderParams (); }

        if (GUILayout.Button ( "Update NavMesh" )) { world.DEBUG_UpdateNavMesh (); }
        if (GUILayout.Button ( "Update Shader Params" )) { world.DEBUG_UpdateShaderParams (); }

        if (GUILayout.Button ( "Generate" )) { world.CreateFrom_New (); }        
    }

    private Type[] GetAttributes<T>() where T : MonoBehaviour
    {
        Type type = typeof(T);
        FieldInfo[] objectFields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
        List<Type> types = new List<Type>();
        for (int i = 0; i < objectFields.Length; i++)
        {
            Switchable attr = Attribute.GetCustomAttribute(objectFields[i], typeof(Switchable)) as Switchable;

            if (attr != null)
            {
                types.Add(objectFields[i].FieldType);
                Debug.Log(objectFields[i].FieldType);
            }
        }

        return types.ToArray();
    }
}
