using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour {

    public Renderer textureRenderer;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public MeshCollider meshCollider;
    public Material[] materials;

    public void DrawTexture(Texture2D texture)
    {        
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1.0f, texture.height);
        meshRenderer.enabled = false;
        textureRenderer.enabled = true;
    }

    public void DrawMesh(MeshData meshData, Texture2D texture, bool useFlatShading)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();

        if (useFlatShading)
            meshRenderer.sharedMaterial = materials[1];
        else
            meshRenderer.sharedMaterial = materials[0];

        meshRenderer.sharedMaterial.mainTexture = texture;
        meshCollider.sharedMesh = meshFilter.sharedMesh;
        meshRenderer.enabled = true;
        textureRenderer.enabled = false;
        Debug.Log("Mesh Generated");
    }
}
