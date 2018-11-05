using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test_biomeping : MonoBehaviour {

    public Text text;
    public int terrainLayer = 9;
    public int t;
	void Update () {        
        RaycastHit hit;
        int terrainMask = 1 << terrainLayer;
        t = terrainMask;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 5000, terrainMask))
        {            
            FindHeightAtPoint(hit);
        }
	}

    private void FindHeightAtPoint(RaycastHit hit)
    {
        World world = GameObject.FindObjectOfType<World>();

        if (world.heightMap == null) return;
        Vector2 textureCoordinates = WorldToHeightMap(hit.point);
        Debug.Log(textureCoordinates);
        Debug.Log("Height: " + world.heightMap[(int)textureCoordinates.x, (int)textureCoordinates.y]);

        if (text == null) return;
        if (BiomeGenerator.biomeLayers.ContainsKey(textureCoordinates))
        {
            Debug.Log(BiomeGenerator.biomeLayers[textureCoordinates].biomeName);
            text.text = BiomeGenerator.biomeLayers[textureCoordinates].biomeName;
        }
        else text.text = "No Biome Found";
    }

    private Vector2 WorldToHeightMap(Vector3 worldPosition)
    {
        worldPosition += new Vector3(480.0f, 0.0f, 480.0f);
        worldPosition /= 10.0f;
        worldPosition.x = Mathf.Floor(worldPosition.x);
        worldPosition.z = Mathf.Floor(worldPosition.z);
        return new Vector2(worldPosition.x, worldPosition.z);
    }

    //private float Round20(float value, float i = 20)
    //{
    //    if (value > 0)
    //    {
    //        float v = Mathf.Floor((value + (i / 2.0f))) * i;
    //        return v;
    //    }
    //    else if (value < 0)
    //    {
    //        return 0;
    //    }
    //    else return 0;
    //}
}
