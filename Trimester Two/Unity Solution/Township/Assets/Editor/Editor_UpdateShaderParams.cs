using UnityEditor;
using UnityEngine;

public static class Editor_UpdateShaderParams
{
    [MenuItem("World/Update Shaders %w")]
    public static void UpdateShaderParams ()
    {
        GameObject.FindObjectOfType<World> ().DEBUG_UpdateShaderParams ();
    }
}
