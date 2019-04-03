using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Inspectable))]
public class Inspectable_Editor : Editor {

    Inspectable type;
    private bool destroy = false;
    private GameObject cameraObject;
    private Camera camera;

    //private bool displayCamera = false;
    //private RenderTexture renderTexture;        

    private void OnEnable ()
    {
        return;
        type = (Inspectable)target;
        destroy = false;

        GameObject[] r = GameObject.FindObjectsOfType<GameObject> ().Where ( obj => obj.name == "_InspectableCameraView" ).ToArray();

        for (int i = 0; i < r.Length; i++)
        {
            PrefabType p = PrefabUtility.GetPrefabType ( r[i] );
            if (p == PrefabType.Prefab || p == PrefabType.ModelPrefab) { Debug.LogError ( "Found asset " + r[i].name + " - returning", r[i] ); continue; }

            //Debug.Log ( r[i].name, r[i] );
            DestroyImmediate ( r[i] );
        }

        if (cameraObject)
        {
            DestroyImmediate ( cameraObject );
        }

        if (Application.isPlaying) return;

        PrefabType pType = PrefabUtility.GetPrefabType ( type );
        if (pType == PrefabType.ModelPrefab || pType == PrefabType.Prefab) return;

        cameraObject = new GameObject ( "_InspectableCameraView" );
        cameraObject.transform.rotation = Camera.main.transform.rotation;
        camera = cameraObject.AddComponent<Camera> ();
        camera.enabled = false;
        camera.targetTexture = new RenderTexture ( 1920, 1080, 24, RenderTextureFormat.ARGB32 );
    }

    public override void OnInspectorGUI ()
    {
        base.OnInspectorGUI ();

        if (cameraObject)
        {
            if (camera)
            {
                Vector3 position = type.transform.TransformPoint ( type.LockOffset );
                position -= Camera.main.transform.forward * type.LockDistance;

                cameraObject.transform.position = position;
                camera.Render ();
                GUILayout.Label ( camera.targetTexture, GUILayout.MaxWidth ( 256 ), GUILayout.MaxHeight ( 256 ) );

                Repaint ();
            }
        }
    }

    private void OnDisable ()
    {
        if (destroy) return;
        destroy = true;

        if (cameraObject)
            DestroyImmediate ( cameraObject );
    }

    private void OnDestroy ()
    {
        if (destroy) return;
        destroy = true;

        if (cameraObject)
            DestroyImmediate ( cameraObject );
    }
}
