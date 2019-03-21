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

    private DEBUG_DrawSnowDepressionsWithMouse snowDepressions;

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

    [SerializeField] private float currentTemperature = 30.0f;

    [SerializeField] [Range(0.001f, 0.1f)] private float flakeAmount;
    [SerializeField] [Range(0.0f, 1.0f)] private float flakeOpacity;
    [SerializeField] private Shader _removeDepressionsShader;
    private Material _removeDepressionsMaterial;

    [SerializeField] private ParticleSystem snow_Particles;

    private void Start ()
    {
        terrainMaterial = terrainObject.GetComponent<MeshRenderer> ().material;
        _removeDepressionsMaterial = new Material ( _removeDepressionsShader );
        snowDepressions = FindObjectOfType<DEBUG_DrawSnowDepressionsWithMouse> ();
    }

    public void CheckTemperature ()
    {
        currentTemperature = TemperatureController.Temperature;
        CheckShouldSnow ();
    }

    private void Update ()
    {
        SetTerrainSnowLevels ();
        SetObjectSnowLevels ();
        RemoveSnowDepressions ();
    }

    private void CheckShouldSnow ()
    {
        if (currentTemperature <= terrainTemperatureThreshold)
        {
            if (terrainSnowLevel == 0)
            {
                shouldTerrainSnow = true;
                //ResetDepressions ();
            }
        }
        else
        {
            if (terrainSnowLevel == maxSnowDepth)
            {
                if(currentTemperature > terrainTemperatureThreshold + 2)
                {
                    shouldTerrainSnow = false;
                }
            }
        }

        if (currentTemperature <= objectTemperatureThreshold)
        {
            if (objectSnowLevel == 0)
                shouldObjectSnow = true;
        }
        else
        {
            if (objectSnowLevel == 0.33f)
            {
                if(currentTemperature > objectTemperatureThreshold + 2)
                {
                    shouldObjectSnow = false;
                }
            }
                //shouldObjectSnow = false;
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
    }

    private void SetTerrainSnowLevels ()
    {
        if (shouldTerrainSnow)
        {
            if (terrainSnowLevel < maxSnowDepth)
            {
                terrainSnowLevel += GameTime.DeltaGameTime * terrainSnowLevelDamp * maxSnowDepth;
                terrainMaterial.SetFloat ( "_Displacement", terrainSnowLevel );
                terrainMaterial.SetFloat ( "_MaxDisplacement", maxSnowDepth );
            }
            else terrainSnowLevel = maxSnowDepth;
        }
        else
        {
            if (terrainSnowLevel > 0)
            {
                terrainSnowLevel -= GameTime.DeltaGameTime * terrainSnowLevelDamp;
                terrainMaterial.SetFloat ( "_Displacement", terrainSnowLevel );
                terrainMaterial.SetFloat ( "_MaxDisplacement", maxSnowDepth );
            }
            else terrainSnowLevel = 0;
        }
    }

    private void SetObjectSnowLevels ()
    {
        if (shouldObjectSnow)
        {
            if (objectSnowLevel < 0.33f)
            {
                objectSnowLevel += GameTime.DeltaGameTime * objectSnowLevelDamp;
                for (int i = 0; i < objectMaterials.Count; i++)
                {
                    if (objectMaterials[i] != null)
                        objectMaterials[i].material.SetFloat ( "_Snow", objectSnowLevel );
                    else objectMaterials.RemoveAt ( i );
                }
            }
            else objectSnowLevel = 0.33f;
        }
        else
        {
            if (objectSnowLevel > 0)
            {
                objectSnowLevel -= GameTime.DeltaGameTime * objectSnowLevelDamp;

                for (int i = 0; i < objectMaterials.Count; i++)
                {
                    if (objectMaterials[i] != null)
                        objectMaterials[i].material.SetFloat ( "_Snow", objectSnowLevel );
                    else objectMaterials.RemoveAt ( i );
                }
            }
            else objectSnowLevel = 0;
        }
    }

    public void DrawDepression (float brushSize, float strength, Vector3 position)
    {
        if (shouldTerrainSnow)
        {
            snowDepressions.DrawDepression ( brushSize, strength, position );
        }
    }

    private void RemoveSnowDepressions ()
    {
        if (!shouldTerrainSnow) return;

        _removeDepressionsMaterial.SetFloat ( "_PRNG", Random.value );
        _removeDepressionsMaterial.SetFloat ( "_FlakeAmount", flakeAmount );
        _removeDepressionsMaterial.SetFloat ( "_FlakeOpacity", flakeOpacity );

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
                if (m[i] == null) continue;

                objectMaterials.Add ( m[i] );                
                m[i].material.SetFloat ( "_Snow", objectSnowLevel );
                
            }
        }
        else
        {
            for (int i = 0; i < m.Length; i++)
            {
                if (m[i] == null) continue;

                if (objectMaterials.Contains ( m[i] ))
                    objectMaterials.Remove ( m[i] );
            }
        }
    }
}
