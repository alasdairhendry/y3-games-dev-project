//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[ExecuteInEditMode]
public class World : MonoBehaviour
{

    [HideInInspector] public WorldData worldData;
    [HideInInspector] public NoiseData noiseData;
    [HideInInspector] public TextureData textureData;
    [HideInInspector] public BiomeData biomeData;
    [HideInInspector] public EnvironmentData environmentData;

    //[SerializeField] private _DEBUG_NAV_DATA DEBUG_NAV_DATA;
    private bool createWithOldNav = false;

    [SerializeField] private Material terrainMaterial;
    [SerializeField] private GameObject terrainMeshObject;

    [Space]

    [SerializeField] private bool DEBUG_MESH = true;
    [SerializeField] private bool DEBUG_EMERGE = true;
    [SerializeField] private bool DEBUG_BIOME = true;
    [SerializeField] private bool DEBUG_ENV = true;
    [SerializeField] private bool DEBUG_NAV = true;

    public Richness richness { get; protected set; }

    public float[,] heightMap;
    private Mesh terrainMesh;

    private LoadState terrainLoadState = new LoadState("terrainLoadState");
    [SerializeField] private bool isGenerating = false;

    public System.Action OnTerrainBeginGenerate;
    public System.Action<int, int> OnTerrainGenerateStateChange;
    public System.Action OnTerrainEndGenerate;
    /// <summary>
    /// Order of events
    /// 
    /// Generate Mesh
    /// Generate Water
    /// Generate Environment: 
    ///                         Trees
    ///                         Rocks
    ///                         Bushes
    /// Generate NavMesh Surface
    /// </summary>

    private void Start()
    {
        terrainLoadState.onStart += () => { isGenerating = true; if (OnTerrainBeginGenerate != null) OnTerrainBeginGenerate(); };
        terrainLoadState.onStageComplete += OnTerrainGenerateStateChange;
        terrainLoadState.onComplete += () => { isGenerating = false; if (OnTerrainEndGenerate != null) OnTerrainEndGenerate(); };
    }

    public void SetRichness(Richness richness)
    {
        this.richness = richness;
    }

    public void Create(bool oldNav)
    {
        if (isGenerating) return;
        createWithOldNav = oldNav;
        DestroyEnvironmentObjects ();

        Seed.CreateSeed(noiseData.seed);
        AddLoadStages();
        StartCoroutine(CreateWorld());
    }

    private void DestroyEnvironmentObjects ()
    {
        for (int i = 0; i < transform.Find("Environment").childCount; i++)
        {
            Destroy ( transform.Find ( "Environment" ).GetChild ( i ).gameObject );
        }
    }

    private void AddLoadStages()
    {
        terrainLoadState.ResetStage();

        terrainLoadState.AddStage("chm");

        if (DEBUG_MESH)
            terrainLoadState.AddStage("cm");

        if (DEBUG_EMERGE)
            terrainLoadState.AddStage("dm");

        if (DEBUG_BIOME)
            terrainLoadState.AddStage("cb");

        if (DEBUG_ENV)
            terrainLoadState.AddStage("ce");

        if (DEBUG_NAV)
            terrainLoadState.AddStage("cnm");
    }

    private IEnumerator CreateWorld()
    {
        terrainLoadState.Begin();
        Debug.Log("CreateWorld: Start");
        yield return CreateHeightMap();

        if (DEBUG_MESH)
            yield return CreateMesh();

        if (DEBUG_EMERGE)
            yield return DisplayMesh(2.0f, 1.0f);
        else terrainMeshObject.GetComponent<MeshRenderer>().enabled = true;

        if (DEBUG_BIOME)
            yield return CreateBiomes();

        if (DEBUG_ENV)
            yield return CreateEnvironment();

        if (DEBUG_NAV)
            yield return CreateNavMeshSurface();
        yield return null;
        Debug.Log("CreateWorld: Finished");
    }

    private IEnumerator CreateHeightMap()
    {
        Debug.Log("CreateHeightMap: Start");
        if (worldData == null) { Debug.LogError("No World Data"); yield break; }
        if (noiseData == null) { Debug.LogError("No Noise Data"); yield break; }
        if (textureData == null) { Debug.LogError("No Texture Data"); yield break; }
        if (terrainMaterial == null) { Debug.LogError("No Material Data"); yield break; }

        yield return MapGenerator.GenerateMap(worldData, noiseData, textureData, terrainMaterial, this);
        Debug.Log("HeightMap Length: (" + heightMap.GetLength(0) + ", " + heightMap.GetLength(1) + ")");
        Debug.Log("CreateHeightMap: Finished");
        terrainLoadState.UpdateStage("chm", true);
        yield return null;
    }

    private IEnumerator CreateMesh()
    {
        Debug.Log("CreateMesh: Start");
        if (heightMap == null) { Debug.LogError("No Height Map"); yield break; }
        if (worldData == null) { Debug.LogError("No World Data"); yield break; }

        yield return MeshGenerator.GenerateTerrainMesh(heightMap, worldData.heightMultiplier, worldData.heightCruve, 1, worldData.useFlatShading, this, terrainMeshObject.transform.localScale.x);
        terrainMeshObject.GetComponent<MeshFilter>().sharedMesh = terrainMesh;
        terrainMeshObject.GetComponent<MeshCollider>().sharedMesh = terrainMesh;
        terrainLoadState.UpdateStage("cm", true);
        Debug.Log("CreateMesh: Finished");
    }

    private IEnumerator DisplayMesh(float time, float targetY)
    {
        float x = 0.0f;
        float y = 0.0f;
        Vector3 lScale = terrainMeshObject.transform.localScale;
        terrainMeshObject.transform.localScale = new Vector3(lScale.x, 0.0f, lScale.z);
        terrainMeshObject.GetComponent<MeshRenderer>().enabled = true;

        while (x < time)
        {
            x += Time.deltaTime;
            y = Mathf.Lerp(0.0f, targetY, x / time);
            terrainMeshObject.transform.localScale = new Vector3(lScale.x, y, lScale.z);

            yield return null;
        }

        terrainLoadState.UpdateStage("dm", true);
    }

    private IEnumerator CreateBiomes()
    {
        yield return BiomeGenerator.CreateBiomes(heightMap, biomeData, this);
        terrainLoadState.UpdateStage("cb", true);
    }

    private IEnumerator CreateEnvironment()
    {
        yield return EnvironmentGenerator.GenerateEnvironment(environmentData, this);
        yield return null;
        terrainLoadState.UpdateStage("ce", true);
    }

    private IEnumerator CreateNavMeshSurface()
    {
        NavMeshData dat = AssetDatabase.LoadAssetAtPath<NavMeshData> ( "Assets/NavData.asset" );
        if (createWithOldNav && dat != null)
        {
            NavMesh.RemoveAllNavMeshData ();
            NavMesh.AddNavMeshData ( dat );            
            terrainLoadState.UpdateStage ( "cnm", true );

            yield break;
            //if(DEBUG_NAV_DATA.data == null)
            //{
            //    Debug.LogError ( "No nav data - building nav mesh" );

            //    GetComponent<NavMeshSurface> ().BuildNavMesh ();
            //    DEBUG_NAV_DATA.data = GetComponent<NavMeshSurface> ().navMeshData;

            //}
            //NavMesh.AddNavMeshData ( DEBUG_NAV_DATA.data );
            //GetComponent<NavMeshSurface> ().RemoveData ();
            //GetComponent<NavMeshSurface> ().navMeshData = DEBUG_NAV_DATA.data;
            //GetComponent<NavMeshSurface> ().AddData ();
        }
        else
        {
            GetComponent<NavMeshSurface> ().BuildNavMesh ();
            AssetDatabase.CreateAsset ( GetComponent<NavMeshSurface> ().navMeshData, "Assets/NavData.asset" );
            yield return null;
            terrainLoadState.UpdateStage ( "cnm", true );
        }
    }

    public void SetHeightMap(float[,] heightMap)
    {
        this.heightMap = heightMap;
    }

    public void SetMesh(Mesh mesh)
    {
        this.terrainMesh = mesh;
    }

    public void SpawnMeshGraphic(string path, Vector3 position, Quaternion rotation, Transform parent)
    {
        GameObject p = Resources.Load<GameObject>(path) as GameObject;
        GameObject i = Instantiate(p, position, Quaternion.Euler(0.0f, Random.value * 360.0f, 0.0f), transform.Find("Environment"));
        int tMask = 1 << 9;
        RaycastHit hit;
        if (Physics.Raycast(position, Vector3.down, out hit, 5000, tMask))
        {
            i.transform.position = hit.point - new Vector3(0.0f, 0.5f, 0.0f);
        }

    }
}


