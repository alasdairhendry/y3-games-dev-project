using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropVisualiser : MonoBehaviour {

    [SerializeField] private List<RendererDefault> renderers = new List<RendererDefault> ();
    private bool collectedRenderers = false;
    private Material currentMaterial;

    private void Start ()
    {
        VisualiserController.Instance.AddVisualiser ( this );
    }

    private void CollectRenderers ()
    {
        if (collectedRenderers) return;

        Renderer[] r = GetComponentsInChildren<Renderer> ( true );
        for (int i = 0; i < r.Length; i++)
        {
            if (r[i].GetComponent<TextMesh> ()) continue;

            RendererDefault rendererDefault = new RendererDefault
            {
                meshRenderer = r[i],
                materials = r[i].materials,
                gameObject = r[i].gameObject
            };
            renderers.Add ( rendererDefault );
        }

        collectedRenderers = true;
    }

    public void Visualise (Material material)
    {
        if (currentMaterial != null && currentMaterial == material) return;
        if (!collectedRenderers) CollectRenderers ();        

        currentMaterial = material;

        for (int i = 0; i < renderers.Count; i++)
        {
            Material[] newMaterials = new Material[renderers[i].materials.Length];

            for (int x = 0; x < newMaterials.Length; x++)
            {
                newMaterials[x] = material;
            }

            renderers[i].meshRenderer.materials = newMaterials;
        }
    }

    public void TurnOff ()
    {
        if (currentMaterial == null) return;
        if (!collectedRenderers) CollectRenderers ();

        currentMaterial = null;

        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].meshRenderer.materials = renderers[i].materials;
        }
    }

    private void OnDestroy ()
    {
        VisualiserController.Instance.RemoveVisualiser ( this );
    }

    [System.Serializable]
    private class RendererDefault
    {
        public GameObject gameObject;
        public Renderer meshRenderer;
        public Material[] materials;
    }
	
}
