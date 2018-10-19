using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapDisplay))]
public class MapDisplay_Editor : Editor {
    public override void OnInspectorGUI()
    {        
        MapDisplay md = (MapDisplay)target;

        if (DrawDefaultInspector())
        {
            if (md.terrainMesh.targetObject != null)
            {
                if (md.terrainMesh.meshCollider == null)
                {
                    if (md.terrainMesh.targetObject.GetComponent<MeshCollider>() == null)
                    {
                        md.terrainMesh.meshCollider = md.terrainMesh.targetObject.AddComponent<MeshCollider>();
                    }
                    else
                    {
                        md.terrainMesh.meshCollider = md.terrainMesh.targetObject.GetComponent<MeshCollider>();
                    }
                }

                if (md.terrainMesh.meshFilter == null)
                {
                    if (md.terrainMesh.targetObject.GetComponent<MeshFilter>() == null)
                    {
                        md.terrainMesh.meshFilter = md.terrainMesh.targetObject.AddComponent<MeshFilter>();
                    }
                    else
                    {
                        md.terrainMesh.meshFilter = md.terrainMesh.targetObject.GetComponent<MeshFilter>();
                    }
                }

                if (md.terrainMesh.meshRenderer == null)
                {
                    if (md.terrainMesh.targetObject.GetComponent<MeshRenderer>() == null)
                    {
                        md.terrainMesh.meshRenderer = md.terrainMesh.targetObject.AddComponent<MeshRenderer>();
                    }
                    else
                    {
                        md.terrainMesh.meshRenderer = md.terrainMesh.targetObject.GetComponent<MeshRenderer>();
                    }
                }
            }

            if (md.waterMesh.targetObject != null)
            {
                if (md.waterMesh.meshCollider == null)
                {
                    if (md.waterMesh.targetObject.GetComponent<MeshCollider>() == null)
                    {
                        md.waterMesh.meshCollider = md.waterMesh.targetObject.AddComponent<MeshCollider>();
                    }
                    else
                    {
                        md.waterMesh.meshCollider = md.waterMesh.targetObject.GetComponent<MeshCollider>();
                    }
                }

                if (md.waterMesh.meshFilter == null)
                {
                    if (md.waterMesh.targetObject.GetComponent<MeshFilter>() == null)
                    {
                        md.waterMesh.meshFilter = md.waterMesh.targetObject.AddComponent<MeshFilter>();
                    }
                    else
                    {
                        md.waterMesh.meshFilter = md.waterMesh.targetObject.GetComponent<MeshFilter>();
                    }
                }

                if (md.waterMesh.meshRenderer == null)
                {
                    if (md.waterMesh.targetObject.GetComponent<MeshRenderer>() == null)
                    {
                        md.waterMesh.meshRenderer = md.waterMesh.targetObject.AddComponent<MeshRenderer>();
                    }
                    else
                    {
                        md.waterMesh.meshRenderer = md.waterMesh.targetObject.GetComponent<MeshRenderer>();
                    }
                }
            }
        }
    }
}
