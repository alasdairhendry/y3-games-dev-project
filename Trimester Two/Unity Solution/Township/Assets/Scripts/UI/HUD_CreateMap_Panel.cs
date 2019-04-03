using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HUD_CreateMap_Panel : UIPanel
{
    [SerializeField] private bool autoStart = true;
    //[SerializeField] private List<Presets> presets = new List<Presets> ();
    [SerializeField] private Button CreateMap_Button;
    [SerializeField] private Button PlayMap_Button;
    [SerializeField] private TMP_InputField name_IField;
    [SerializeField] private TMP_InputField seed_IField;
    [SerializeField] private Dropdown mapType_DDown;
    [SerializeField] private Dropdown resources_DDown;

    [Header ("Starting Conditions")]
    [SerializeField] private Dropdown preset_DDown;
    [SerializeField] private TMP_InputField money_IField;
    [SerializeField] private Dropdown season_DDown;
    [SerializeField] private Dropdown warehouseResources_DDown;
    [SerializeField] private TMP_InputField families_IField;
    [SerializeField] private TMP_InputField children_IField;

    [SerializeField] private Slider fertility_Slider;
    [SerializeField] private TextMeshProUGUI fertilityValue;

    [SerializeField] private Slider mortality_Slider;
    [SerializeField] private TextMeshProUGUI mortalityValue;

    [SerializeField] private Slider pilgrim_Slider;
    [SerializeField] private TextMeshProUGUI pilgrimValue;

    protected override void Start ()
    {
        base.Start ();
        GenerateMapTypeDDown ();
        GenerateResourcesDDown ();

        GeneratePresetsDDown ();
        GenerateSeasonsDDown ();
        GenerateWarehouseDDown ();

        SetupStartingConditions ();
        UpdateStartingConditions ( 0 );

        World world = FindObjectOfType<World> ();
        world.OnTerrainBeginGenerate += OnTerrainGenerationBegin;
        world.OnTerrainEndGenerate += OnTerrainGenerationEnd;

        GetComponentInChildren<TabGroup> ().onTabChanged += (go, name) =>
        {
            if (name == "New Game")
                GetComponent<RectTransform> ().sizeDelta = new Vector2 ( 512, 502.28f );
            else 
                GetComponent<RectTransform> ().sizeDelta = new Vector2 ( 512, 436.0f );
        };
    }

    private void GenerateMapTypeDDown ()
    {
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData> ();
        for (int p = 0; p < WorldPresets.Instance.Presets.Count; p++)
        {
            options.Add ( new Dropdown.OptionData ( WorldPresets.Instance.Presets[p].presetName ) );
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
        resources_DDown.value = 1;
    }

    private void GeneratePresetsDDown ()
    {
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData> ();

        options.Add ( new Dropdown.OptionData ( "Easy" ) );
        options.Add ( new Dropdown.OptionData ( "Medium" ) );
        options.Add ( new Dropdown.OptionData ( "Hard" ) );
        options.Add ( new Dropdown.OptionData ( "Custom" ) );

        preset_DDown.ClearOptions ();
        preset_DDown.AddOptions ( options );
    }

    private void GenerateSeasonsDDown ()
    {
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData> ();

        options.Add ( new Dropdown.OptionData ( SeasonController.Season.Spring.ToString () ) );
        options.Add ( new Dropdown.OptionData ( SeasonController.Season.Summer.ToString () ) );
        options.Add ( new Dropdown.OptionData ( SeasonController.Season.Autumn.ToString () ) );
        options.Add ( new Dropdown.OptionData ( SeasonController.Season.Winter.ToString () ) );

        season_DDown.ClearOptions ();
        season_DDown.AddOptions ( options );
    }

    private void GenerateWarehouseDDown ()
    {
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData> ();

        options.Add ( new Dropdown.OptionData ( Richness.Plentiful.ToString () ) );
        options.Add ( new Dropdown.OptionData ( Richness.Abundant.ToString () ) );
        options.Add ( new Dropdown.OptionData ( Richness.Sparse.ToString () ) );

        warehouseResources_DDown.ClearOptions ();
        warehouseResources_DDown.AddOptions ( options );
    }

    private bool isUserChange = false;

    private void SetupStartingConditions ()
    {
        preset_DDown.onValueChanged.AddListener ( (i) =>
        {
            if (GameData.Instance.StartingConditionIndex == 3 && i == 3) return;

            GameData.Instance.SetStartingConditions ( i );
            UpdateStartingConditions ( i );
        } );


        #region money

        TMP_InputField.OnChangeEvent moneyEvent = new TMP_InputField.OnChangeEvent ();
        moneyEvent.AddListener ( (s) =>
        {
            if (GameData.Instance.StartingConditionIndex == 3)
                GameData.Instance.ReturnCustomCondition ().startingMoney = int.Parse ( s );
        } );
        money_IField.onValueChanged = moneyEvent;

        #endregion
        #region season

        season_DDown.onValueChanged.AddListener ( (i) => {
            if (GameData.Instance.StartingConditionIndex == 3)
                GameData.Instance.ReturnCustomCondition ().startingSeason = (SeasonController.Season)i;
        } );

        #endregion
        #region warehouse

        warehouseResources_DDown.onValueChanged.AddListener ( (i) => {
            if (GameData.Instance.StartingConditionIndex == 3)
            {
                GameData.Instance.ReturnCustomCondition ().startingResources = GameData.Instance.startingResourcesData[i];
                GameData.Instance.ReturnCustomCondition ().startingResourcesRichness = (Richness)(2 - i);
            }
        } );

        #endregion
        #region familes

        TMP_InputField.OnChangeEvent familiesEvent = new TMP_InputField.OnChangeEvent ();
        familiesEvent.AddListener ( (s) =>
        {
            if (GameData.Instance.StartingConditionIndex == 3)
                GameData.Instance.ReturnCustomCondition ().startingFamilies = int.Parse ( s );
        } );
        families_IField.onValueChanged = familiesEvent;

        #endregion
        #region children

        TMP_InputField.OnChangeEvent childrenEvent = new TMP_InputField.OnChangeEvent ();
        childrenEvent.AddListener ( (s) =>
        {
            if (GameData.Instance.StartingConditionIndex == 3)
                GameData.Instance.ReturnCustomCondition ().maximumChildren = int.Parse ( s );
        } );
        children_IField.onValueChanged = childrenEvent;

        #endregion
        #region fertility

        fertility_Slider.onValueChanged.AddListener ( (f) =>
        {
            if (GameData.Instance.StartingConditionIndex == 3)
                GameData.Instance.ReturnCustomCondition ().fertilityModifier = f;
        } );

        #endregion
        #region mortality

        mortality_Slider.onValueChanged.AddListener ( (f) =>
        {
            if (GameData.Instance.StartingConditionIndex == 3)
                GameData.Instance.ReturnCustomCondition ().mortalityRate = f;
        } );

        #endregion
        #region fertility

        pilgrim_Slider.onValueChanged.AddListener ( (f) =>
        {
            if (GameData.Instance.StartingConditionIndex == 3)
                GameData.Instance.ReturnCustomCondition ().pilgrimChance = f;
        } );

        #endregion
    }

    private void RemoveStartupConditionListeners ()
    {
        preset_DDown.onValueChanged.RemoveAllListeners ();
        money_IField.onValueChanged.RemoveAllListeners ();
        season_DDown.onValueChanged.RemoveAllListeners ();
        families_IField.onValueChanged.RemoveAllListeners ();
        children_IField.onValueChanged.RemoveAllListeners ();
        fertility_Slider.onValueChanged.RemoveAllListeners ();
        mortality_Slider.onValueChanged.RemoveAllListeners ();
        pilgrim_Slider.onValueChanged.RemoveAllListeners ();
    }

    private void UpdateStartingConditions (int index)
    {
        GameData.StartingCondition cond = GameData.Instance.startingConditionsData[index];

        bool shouldBeActive = false;

        if(index == 3)
        {
            shouldBeActive = true;
        }
        else
        {
            shouldBeActive = false;
        }

        money_IField.interactable = shouldBeActive;
        season_DDown.interactable = shouldBeActive;
        warehouseResources_DDown.interactable = shouldBeActive;
        families_IField.interactable = shouldBeActive;
        children_IField.interactable = shouldBeActive;
        fertility_Slider.interactable = shouldBeActive;
        mortality_Slider.interactable = shouldBeActive;
        pilgrim_Slider.interactable = shouldBeActive;

        //preset_DDown.value = index;
        money_IField.text = cond.startingMoney.ToString ();
        season_DDown.value = (int)cond.startingSeason;

        warehouseResources_DDown.value = GameData.Instance.startingResourcesData.IndexOf ( cond.startingResources );

        families_IField.text = cond.startingFamilies.ToString ();
        children_IField.text = cond.maximumChildren.ToString ();

        fertility_Slider.value = cond.fertilityModifier;
        mortality_Slider.value = cond.mortalityRate;
        pilgrim_Slider.value = cond.pilgrimChance;

        fertilityValue.text = cond.fertilityModifier.ToString ();
        mortalityValue.text = cond.mortalityRate.ToString ();
        pilgrimValue.text = cond.pilgrimChance.ToString ();
    }

    public void OnClick_CreateMap ()
    {
        World world = SetWorldDatas ();
        if (string.IsNullOrEmpty ( seed_IField.text )) seed_IField.text = "0";
        world.noiseData.seed = int.Parse ( seed_IField.text );
        world.CreateFrom_New ();
    }

    private World SetWorldDatas ()
    {
        for (int p = 0; p < WorldPresets.Instance.Presets.Count; p++)
        {
            if (mapType_DDown.options[mapType_DDown.value].text == WorldPresets.Instance.Presets[p].presetName)
            {
                World world = GameObject.FindObjectOfType<World> ();
                WorldPresets.Instance.SetPresets ( p, world );

                world.SetRichness ( EnumCollection.richness[resources_DDown.options[resources_DDown.value].text] );
                return world;
            }
        }

        Debug.LogError ( "No Preset Found" );
        return null;
    }

    public void OnClick_PlayMap ()
    {
        if (string.IsNullOrEmpty ( name_IField.text ))
        {
            HUD_Dialogue_Panel.Instance.ShowDialogue ( "Town Name Invalid", "Give you town a name so your citizens know where they are from!",
                new DialogueButton ( DialogueButton.Preset.Okay, () => { EventSystem.current.SetSelectedGameObject ( name_IField.gameObject ); } ) );

            return;
        }

        if (PersistentData.SaveFileExists ( name_IField.text ))
        {
            HUD_Dialogue_Panel.Instance.ShowDialogue ( "Town Name Exists", "A town already exists with that name.\n Do you want to overwrite this save file?",
            new DialogueButton ( DialogueButton.Preset.Yes, () => { PersistentData.DeleteSaveFile ( name_IField.text ); CreateMap (); } ),
            new DialogueButton ( DialogueButton.Preset.No, () => { EventSystem.current.SetSelectedGameObject ( name_IField.gameObject ); } ) );

            return;
        }

        CreateMap ();
    }

    private void CreateMap ()
    {
        Hide ();

        GameData.Instance.SetGameDataType ( GameData.GameDataType.New, name_IField.text );

        World world = FindObjectOfType<World> ();
        world.Create_NavMesh ( () => {
            FindObjectOfType<World> ().DEBUG_UpdateShaderParams ();
            FindObjectOfType<HUD_LoadingOverlay_Panel> ().Hide ();
            FindObjectOfType<HUD_LeftSide_Panel> ().Show ();
            FindObjectOfType<HUD_Toolbar_Panel> ().Show ();
            FindObjectOfType<HUD_GameTime_Panel> ().Show ();

            // The default
            //FindObjectOfType<CameraMovement> ().PanTo ( new Vector3 ( 0.0f, 110.84f, -110.41f ) );
            TutorialController.Instance.Start_NewGame ();
            FindObjectOfType<CameraMovement> ().PanTo ( new Vector3 ( 0.0f, 16.0f, -15.5f ) );
        } );
    }

    //public void OnClick_Skip ()
    //{
    //    Hide ();
    //    FindObjectOfType<World> ().DEBUG_UpdateShaderParams ();
    //    FindObjectOfType<World> ().DEBUG_UpdateNavMesh ();
    //    FindObjectOfType<HUD_LoadingOverlay_Panel> ().Hide ();
    //    FindObjectOfType<HUD_LeftSide_Panel> ().Show ();
    //    FindObjectOfType<HUD_Toolbar_Panel> ().Show ();
    //    FindObjectOfType<HUD_GameTime_Panel> ().Show ();
    //    FindObjectOfType<CameraMovement> ().PanTo ( new Vector3 ( 0.0f, 16.0f, -24.5f ) );
    //    //FindObjectOfType<CameraMovement> ().PanTo ( new Vector3 ( 0.0f, 110.84f, -110.41f ) );
    //    FindObjectOfType<World> ().SetTemperatures ();
    //}

    public void OnLoad ()
    {
        Hide ();
        FindObjectOfType<HUD_LoadingOverlay_Panel> ().Hide ();
        FindObjectOfType<HUD_LeftSide_Panel> ().Show ();
        FindObjectOfType<HUD_Toolbar_Panel> ().Show ();
        FindObjectOfType<HUD_GameTime_Panel> ().Show ();
    }

    private void OnTerrainGenerationBegin ()
    {
        CreateMap_Button.interactable = false;
        PlayMap_Button.interactable = false;
    }

    private void OnTerrainGenerationEnd ()
    {
        CreateMap_Button.interactable = true;
        PlayMap_Button.gameObject.SetActive ( true );
        PlayMap_Button.interactable = true;
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
