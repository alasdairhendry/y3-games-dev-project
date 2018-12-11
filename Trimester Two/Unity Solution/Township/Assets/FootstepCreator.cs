using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepCreator : MonoBehaviour {

    [SerializeField] private Material material;
    //[SerializeField] private Vector3[] corners;
    [SerializeField] private float width = 1;
    [SerializeField] private float minHeight = 1;
    [SerializeField] private List<GameObject> quads = new List<GameObject> ();

    [ContextMenu("Create")]
    public void Create (Vector3[] corners)
    {
        Clear ();

        for (int i = 0; i < corners.Length; i++)
        {
            if (i == corners.Length - 1) continue;

            GameObject quad = GameObject.CreatePrimitive ( PrimitiveType.Quad );

            float height = Vector3.Distance ( corners[i], corners[i + 1] );
            quad.transform.localScale = new Vector3 ( width, Mathf.Max ( minHeight, height ), 0.0f );

            Vector3 direction = corners[i + 1] - corners[i];

            float angle = Mathf.Atan2 ( corners[i + 1].x - corners[i].z, corners[i + 1].z - corners[i].x ) * Mathf.Rad2Deg;            
            quad.transform.eulerAngles = new Vector3 ( 90.0f, angle, 0.0f );

            quad.transform.position = corners[i] + (quad.transform.up * Vector3.Distance ( corners[i + 1], corners[i] ) / 2.0f);
            quad.GetComponent<MeshRenderer> ().material = material;

            quad.GetComponent<MeshRenderer> ().material.mainTextureScale = new Vector2 ( 1.0f, Mathf.Max ( minHeight, Mathf.FloorToInt ( height ) ) );

            quad.transform.SetParent ( transform );
            quads.Add ( quad );
        }
    }

    [ContextMenu ( "Clear" )]
    public void Clear ()
    {
        for (int i = 0; i < quads.Count; i++)
        {
            DestroyImmediate ( quads[i] );
        }

        quads.Clear ();        
    }
}
