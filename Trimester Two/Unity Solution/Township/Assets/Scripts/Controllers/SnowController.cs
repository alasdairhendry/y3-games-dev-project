using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowController : MonoBehaviour
{
    public static SnowController Instance;

    private void Awake ()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy ( this.gameObject );
    }

    [SerializeField] private GameObject terrainObject;
    private Material terrainMaterial;

    private List<Renderer> objectMaterials = new List<Renderer> ();
    //private List<SkinnedMeshRenderer> objectSkinnedMaterials = new List<SkinnedMeshRenderer> ();

    [SerializeField] [Range(0, 1)] private float maxSnowDepth = 0.5f;
    public float MaxSnowDepth { get { return maxSnowDepth; } }

    [SerializeField] private bool shouldTerrainSnow = false;
    [SerializeField] private bool shouldObjectSnow = false;
    [SerializeField] private float terrainTemperatureThreshold = 2.0f;
    [SerializeField] private float objectTemperatureThreshold = 3.0f;

    [SerializeField] private float terrainSnowLevel;
    public float TerrainSnowLevel { get { return terrainSnowLevel; } }

    [SerializeField] private float terrainSnowLevelDamp = 0.25f;

    [SerializeField] private float objectSnowLevel;
    [SerializeField] private float objectSnowLevelDamp = 0.5f;

    [SerializeField] private float currentTemperature;

    [SerializeField] [Range(0.001f, 0.1f)] private float flakeAmount;
    [SerializeField] [Range(0.0f, 1.0f)] private float flakeOpacity;
    [SerializeField] private Shader _removeDepressionsShader;
    private Material _removeDepressionsMaterial;

    [SerializeField] private ParticleSystem snow_Particles;

    private void Start ()
    {
        terrainMaterial = terrainObject.GetComponent<MeshRenderer> ().material;
        _removeDepressionsMaterial = new Material ( _removeDepressionsShader );
    }

    private void Update ()
    {
        currentTemperature = TemperatureController.Instance.Temperature;

        CheckShouldSnow ();
        SetTerrainSnowLevels ();
        SetObjectSnowLevels ();
        RemoveSnowDepressions ();
    }

    private void CheckShouldSnow ()
    {
        if (currentTemperature <= terrainTemperatureThreshold)
        {
            if (terrainSnowLevel == 0)
                shouldTerrainSnow = true;
        }
        else
        {
            if (terrainSnowLevel == maxSnowDepth)
                shouldTerrainSnow = false;
        }

        if (currentTemperature <= objectTemperatureThreshold)
        {
            if (objectSnowLevel == 0)
                shouldObjectSnow = true;
        }
        else
        {
            if (objectSnowLevel == 0.33f)
                shouldObjectSnow = false;
        }

        if (shouldObjectSnow || shouldTerrainSnow)
        {
            if (!snow_Particles.isPlaying)
                snow_Particles.Play ();
        }
        else
        {
            if (snow_Particles.isPlaying)
                snow_Particles.Stop ();
        }

        //shouldTerrainSnow = currentTemperature <= terrainTemperatureThreshold ? true : false;
        //shouldObjectSnow = currentTemperature <= objectTemperatureThreshold ? true : false;
    }

    private void SetTerrainSnowLevels ()
    {
        if (shouldTerrainSnow)
        {
            if (terrainSnowLevel < maxSnowDepth)
                terrainSnowLevel += GameTime.DeltaGameTime * terrainSnowLevelDamp * maxSnowDepth;
            else terrainSnowLevel = maxSnowDepth;
        }
        else
        {
            if (terrainSnowLevel > 0)
                terrainSnowLevel -= GameTime.DeltaGameTime * terrainSnowLevelDamp;
            else terrainSnowLevel = 0;
        }

        terrainMaterial.SetFloat ( "_Displacement", terrainSnowLevel );
        terrainMaterial.SetFloat ( "_MaxDisplacement", maxSnowDepth );
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

    private void RemoveSnowDepressions ()
    {
        _removeDepressionsMaterial.SetFloat ( "_PRNG", Random.value );
        _removeDepressionsMaterial.SetFloat ( "_FlakeAmount", flakeAmount );
        _removeDepressionsMaterial.SetFloat ( "_FlakeOpacity", flakeOpacity );

        //RenderTexture snow = new RenderTexture ( 2048, 2048, 0, RenderTextureFormat.ARGBFloat );
        RenderTexture snow = (RenderTexture)terrainMaterial.GetTexture ( "_Splat" );
        RenderTexture temp = RenderTexture.GetTemporary ( snow.width, snow.height, 0, RenderTextureFormat.ARGBFloat );
        Graphics.Blit ( snow, temp, _removeDepressionsMaterial );
        Graphics.Blit ( temp, snow );
        terrainMaterial.SetTexture ( "_Splat", snow );
        RenderTexture.ReleaseTemporary ( temp );
    }

    public void SetObjectMaterial(Renderer[] m, bool remove)
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
