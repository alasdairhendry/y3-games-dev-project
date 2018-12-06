using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_CreateMap_Panel : UIPanel
{
    [SerializeField] private bool autoStart = true;
    [SerializeField] private List<Presets> presets = new List<Presets> ();
    [SerializeField] private Button CreateMap_Button;
    [SerializeField] private Button PlayMap_Button;
    [SerializeField] private InputField seed_IField;
    [SerializeField] private Dropdown mapType_DDown;
    [SerializeField] private Dropdown resources_DDown;

    protected override void Start ()
    {
        base.Start ();
        GenerateMapTypeDDown ();
        GenerateResourcesDDown ();
        World world = FindObjectOfType<World> ();
        world.OnTerrainBeginGenerate += OnTerrainGenerationBegin;
        world.OnTerrainEndGenerate += OnTerrainGenerationEnd;

        if (autoStart) OnClick_Skip ();
    }

    private void GenerateMapTypeDDown ()
    {
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData> ();
        for (int p = 0; p < presets.Count; p++)
        {
            options.Add ( new Dropdown.OptionData ( presets[p].presetName ) );
        }

        mapType_DDown.ClearOptions ();
        mapType_DDown.AddOptions ( options );
    }

    private void GenerateResourcesDDown ()
    {
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData> ();

        options.Add ( new Dropdown.OptionData ( Richness.Sparse.ToString () ) );
        options.Add ( new Dropdown.OptionData ( Richness.Abundant.ToString () ) );
        options.Add ( new Dropdown.OptionData ( Richness.Plentiful.ToString () ) );

        resources_DDown.ClearOptions ();
        resources_DDown.AddOptions ( options );
    }

    public void OnClick_CreateMap ()
    {
        World world = SetWorldDatas ();
        if (string.IsNullOrEmpty ( seed_IField.text )) seed_IField.text = "0";
        world.noiseData.seed = int.Parse ( seed_IField.text );
        world.Create ();
    }

    public void OnClick_CreateNavMesh ()
    {
        //World world = SetWorldDatas ();
        //if (string.IsNullOrEmpty ( seed_IField.text )) seed_IField.text = "0";
        //world.noiseData.seed = int.Parse ( seed_IField.text );
        //world.Create (true);
    }

    public void OnClick_PlayMap ()
    {
        Hide ();

        World world = FindObjectOfType<World> ();
        world.Create_NavMesh (() => {            
            FindObjectOfType<World> ().DEBUG_UpdateShaderParams ();
            FindObjectOfType<HUD_LoadingOverlay_Panel> ().Hide ();
            FindObjectOfType<HUD_Toolbar_Panel> ().Show ();
            FindObjectOfType<HUD_GameTime_Panel> ().Show ();
            //FindObjectOfType<CameraMovement> ().PanTo ( new Vector3 ( 0.0f, 16.0f, -24.5f ) );
            FindObjectOfType<CameraMovement> ().PanTo ( new Vector3 ( 0.0f, 110.84f, -110.41f ) );
        } );
    }

    public void OnClick_Skip ()
    {
        Hide ();
        FindObjectOfType<World> ().DEBUG_UpdateShaderParams ();
        FindObjectOfType<HUD_LoadingOverlay_Panel> ().Hide ();
        FindObjectOfType<HUD_Toolbar_Panel> ().Show ();
        FindObjectOfType<HUD_GameTime_Panel> ().Show ();
        //FindObjectOfType<CameraMovement> ().PanTo ( new Vector3 ( 0.0f, 16.0f, -24.5f ) );
        FindObjectOfType<CameraMovement> ().PanTo ( new Vector3 ( 0.0f, 110.84f, -110.41f ) );
        FindObjectOfType<World> ().SetTemperatures ();
    }

    private World SetWorldDatas ()
    {
        for (int p = 0; p < presets.Count; p++)
        {
            if (mapType_DDown.options[mapType_DDown.value].text == presets[p].presetName)
            {
                World world = GameObject.FindObjectOfType<World> ();
                world.worldData = presets[p].worldData;
                world.noiseData = presets[p].noiseData;
                world.textureData = presets[p].textureData;
                world.SetRichness ( EnumCollection.richness[resources_DDown.options[resources_DDown.value].text] );
                return world;
            }
        }

        Debug.LogError ( "No Preset Found" );
        return null;
    }

    private void OnTerrainGenerationBegin ()
    {
        PlayMap_Button.gameObject.SetActive ( false );
    }

    private void OnTerrainGenerationEnd ()
    {
        PlayMap_Button.gameObject.SetActive ( true );      
    }

    [System.Serializable]
    public class Presets
    {
        public string presetName;
        public WorldData worldData;
        public NoiseData noiseData;
        public TextureData textureData;
    }
}
