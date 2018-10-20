using UnityEditor;
using UnityEngine;

/// <summary>
/// Call Draw<ScriptableObjectType> from OnInspectorGUI in an editor script
/// 
/// OnPropertyChange - An optional action to take when the target property is changed.
/// obj - Pass in the serializedObject from the editor script
/// propertyName - Pass in the name of the property that holds your Scriptable Object
/// targetParameter - Pass in a reference to the target parameter
/// foldOut - Create a boolean in the editor script and pass it in here
/// </summary>

public class QuickSwitch : Editor {

    public static void Draw<T>(System.Action OnPropertyChange, SerializedObject obj, string propertyName, ref T targetParameter, ref bool foldOut) where T : ScriptableObject
    {
        SerializedProperty p = obj.FindProperty(propertyName);
        T[] types = GetAssets<T>();

        EditorGUILayout.BeginVertical("Box");
        
        if (Check(() => { EditorGUILayout.PropertyField(p, new GUIContent(ObjectNames.NicifyVariableName(p.name))); }))
        {
            if (OnPropertyChange != null)
                OnPropertyChange();
        }        

        EditorGUI.indentLevel++;
        foldOut = EditorGUILayout.Foldout(foldOut, new GUIContent("Quick Switch"));
        if (foldOut)
        {                        
            for (int i = 0; i < types.Length; i++)
            {                
                EditorGUILayout.BeginHorizontal();
                if (types[i].name == targetParameter.name) GUILayout.Label(">");
                if (GUILayout.Button(types[i].name))
                {
                    targetParameter = types[i];

                    if (OnPropertyChange != null)
                        OnPropertyChange();
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Ping")) EditorGUIUtility.PingObject(targetParameter);       
                if (GUILayout.Button("View")) Selection.activeObject = targetParameter;
                
                EditorGUILayout.EndHorizontal();
            }            
        }
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
        
        obj.ApplyModifiedProperties();
    }

    private static T[] GetAssets<T>() where T : ScriptableObject
    {
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
        T[] t = new T[guids.Length];

        for (int i = 0; i < t.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            t[i] = AssetDatabase.LoadAssetAtPath<T>(path);
        }

        return t;
    }

    private static bool Check(System.Action field)
    {
        EditorGUI.BeginChangeCheck();
        field();
        if (EditorGUI.EndChangeCheck())
        {
            return true;
        }

        return false;
    }
}
