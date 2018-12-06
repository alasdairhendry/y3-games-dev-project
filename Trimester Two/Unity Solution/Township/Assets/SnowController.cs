using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowController : MonoBehaviour
{
    [SerializeField] private GameObject terrainObject;
    private Material terrainMaterial;

    private List<MeshRenderer> objectMaterials = new List<MeshRenderer> ();

    [SerializeField] private bool shouldTerrainSnow = false;
    [SerializeField] private bool shouldObjectSnow = false;
    [SerializeField] private float terrainTemperatureThreshold = 2.0f;
    [SerializeField] private float objectTemperatureThreshold = 3.0f;

    [SerializeField] private float terrainSnowLevel;
    [SerializeField] private float terrainSnowLevelDamp = 0.25f;

    [SerializeField] private float objectSnowLevel;
    [SerializeField] private float objectSnowLevelDamp = 0.5f;

    [SerializeField] private float currentTemperature;

    private void Start ()
    {
        terrainMaterial = terrainObject.GetComponent<MeshRenderer> ().material;
    }

    private void Update ()
    {
        currentTemperature = TemperatureController.Instance.Temperature;

        CheckShouldSnow ();
        SetTerrainSnowLevels ();
        SetObjectSnowLevels ();
    }

    private void CheckShouldSnow ()
    {
        if(currentTemperature <= terrainTemperatureThreshold)
        {

        }
        shouldTerrainSnow = currentTemperature <= terrainTemperatureThreshold ? true : false;
        shouldObjectSnow = currentTemperature <= objectTemperatureThreshold ? true : false;
    }

    private void SetTerrainSnowLevels ()
    {
        if (shouldTerrainSnow)
        {
            if (terrainSnowLevel < 1)
                terrainSnowLevel += GameTime.DeltaGameTime * terrainSnowLevelDamp;
            else terrainSnowLevel = 1;
        }
        else
        {
            if (terrainSnowLevel > 0)
                terrainSnowLevel -= GameTime.DeltaGameTime * terrainSnowLevelDamp;
            else terrainSnowLevel = 0;
        }

        terrainMaterial.SetFloat ( "_Displacement", terrainSnowLevel );
    }

    private void SetObjectSnowLevels ()
    {
        if (shouldObjectSnow)
        {
            if (objectSnowLevel < 0.33f)
                objectSnowLevel += GameTime.DeltaGameTime * objectSnowLevelDamp;
            else objectSnowLevel = 0.33f;
        }
        else
        {
            if (objectSnowLevel > 0)
                objectSnowLevel -= GameTime.DeltaGameTime * objectSnowLevelDamp;
            else objectSnowLevel = 0;
        }

        for (int i = 0; i < objectMaterials.Count; i++)
        {
            objectMaterials[i].material.SetFloat ( "_Snow", objectSnowLevel );
        }
    }

    public void SetObjectMaterial(MeshRenderer[] m, bool remove)
    {
        if (!remove)
        {
            for (int i = 0; i < m.Length; i++)
            {
                objectMaterials.Add ( m[i] );
            }
        }
        else
        {
            for (int i = 0; i < m.Length; i++)
            {
                if (objectMaterials.Contains ( m[i] ))
                    objectMaterials.Remove ( m[i] );
            }
        }
    }
}
