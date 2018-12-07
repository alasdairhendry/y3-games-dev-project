using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEBUG_DrawSnowDepressionsWithMouse : MonoBehaviour {

    public Shader shader;
    private RenderTexture _splatMap;

    public GameObject terrainObject;
    private Material terrainSnowMaterial;
    private Material drawMaterial;

    private Camera _camera;

    private RaycastHit hit;

	void Start () {
        drawMaterial = new Material ( shader );
        drawMaterial.SetVector ( "_Color", Color.red );

        terrainSnowMaterial = GetComponent<MeshRenderer> ().material;
        _splatMap = new RenderTexture ( 2048, 2048, 0, RenderTextureFormat.ARGBFloat );
        terrainSnowMaterial.SetTexture ( "_Splat", _splatMap );
        _camera = Camera.main;
	}

    [SerializeField] private Vector2 offset;

    public void DrawDepression(float brushSize, float strength, Vector3 position)
    {
        
        LayerMask layer = LayerMask.NameToLayer ( "Terrain" );
        if (Physics.Raycast ( position - new Vector3 ( 0.0f, 0.0f, 0.0f ), Vector3.down, out hit ))
        {            
            drawMaterial.SetFloat ( "_BrushSize", brushSize );
            drawMaterial.SetFloat ( "_Strength", strength );


            Vector2 hitPoint = new Vector2 ();
            hitPoint.x = Mathf.InverseLerp ( -480.0f, 480.0f, hit.point.x );
            hitPoint.y = Mathf.InverseLerp ( -480.0f, 480.0f, hit.point.z );

            drawMaterial.SetVector ( "_Coord", new Vector4 ( hitPoint.x - offset.x, hitPoint.y - offset.y, 0, 0 ) );

            RenderTexture temp = RenderTexture.GetTemporary ( _splatMap.width, _splatMap.height, 0, RenderTextureFormat.ARGBFloat );
            Graphics.Blit ( _splatMap, temp );
            Graphics.Blit ( temp, _splatMap, drawMaterial );
            RenderTexture.ReleaseTemporary ( temp );
        }
    }

    //// Update is called once per frame
    //void Update ()
    //{
    //    if (Input.GetMouseButton ( 0 ) && !Input.GetKey ( KeyCode.LeftControl ))
    //    {
    //        Debug.Log ( "0" );
    //        if (Physics.Raycast ( _camera.ScreenPointToRay ( Input.mousePosition ), out hit ))
    //        {
    //            Debug.Log ( "hit" );
    //            drawMaterial.SetVector ( "_Coord", new Vector4 ( hit.textureCoord.x, hit.textureCoord.y, 0, 0 ) );

    //            RenderTexture temp = RenderTexture.GetTemporary ( _splatMap.width, _splatMap.height, 0, RenderTextureFormat.ARGBFloat );
    //            Graphics.Blit ( _splatMap, temp );
    //            Graphics.Blit ( temp, _splatMap, drawMaterial );
    //            RenderTexture.ReleaseTemporary ( temp );
    //        }
    //    }
    //}

    private void OnGUI ()
    {
        //GUI.DrawTexture ( new Rect ( 0, 0, 256, 256 ), _splatMap, ScaleMode.ScaleToFit, false, 1 );
    }
}
