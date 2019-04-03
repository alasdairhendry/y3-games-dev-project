using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameData : MonoSingleton<GameData> {

    public enum GameDataType { New, Loaded }
    public GameDataType gameDataType { get; protected set; } = GameDataType.New;

    public List<StartingCondition> startingConditionsData { get; protected set; } = new List<StartingCondition> ();
    public StartingCondition startingConditions { get; protected set; }

    public List<List<KeyValuePair<int, float>>> startingResourcesData { get; protected set; } = new List<List<KeyValuePair<int, float>>> ();

    [SerializeField] [Range ( 0, 3 )] private int startingConditionIndex = 0;
    public int StartingConditionIndex { get { return startingConditionIndex; } }

    [SerializeField] private AnimationCurve temperatureCurve;
    public AnimationCurve TemperatureCurve { get { return temperatureCurve; } }

    [SerializeField] private AnimationCurve mortalityCurve;
    public AnimationCurve MortalityCurve { get { return mortalityCurve; } }

    public StartingCondition ReturnCustomCondition ()
    {
        return startingConditionsData[3];
    }

    public string CurrentSaveFileName { get; protected set; }

    public PersistentData.SaveData GetSaveData { get { return PersistentData.Load ( CurrentSaveFileName ); } }

    private void Awake ()
    {
        startingResourcesData.Add ( new List<KeyValuePair<int, float>> () {
            new KeyValuePair<int, float>(0, 42),    // Wood
            new KeyValuePair<int, float>(1, 12),    // Brick
            new KeyValuePair<int, float>(2, 10),    // Stone
            new KeyValuePair<int, float>(8, 6),     // Tools
            new KeyValuePair<int, float>(10, 24)    // Fish
        } );

        startingResourcesData.Add ( new List<KeyValuePair<int, float>> () {
            new KeyValuePair<int, float>(0, 30),    // Wood
            new KeyValuePair<int, float>(1, 6),     // Brick
            new KeyValuePair<int, float>(2, 6),     // Stone
            new KeyValuePair<int, float>(8, 2),     // Tools
            new KeyValuePair<int, float>(10, 16)    // Fish
        } );

        startingResourcesData.Add ( new List<KeyValuePair<int, float>> () {
            new KeyValuePair<int, float>(0, 12),    // Wood
            new KeyValuePair<int, float>(1, 4),     // Brick
            new KeyValuePair<int, float>(10, 8)    // Fish
        } );

        StartingCondition easy = new StartingCondition ()
        {
            identifier = "Easy",
            startingMoney = 10000,
            startingFamilies = 4,

            startingResources = startingResourcesData[0],
            startingResourcesRichness = Richness.Plentiful,

            startingSeason = SeasonController.Season.Spring,

            maximumChildren = 4,
            fertilityModifier = 1.5f,
            mortalityRate = 0.5f,

            pilgrimChance = 1.5f            
        };

        StartingCondition medium = new StartingCondition ()
        {
            identifier = "Medium",
            startingMoney = 7500,
            startingFamilies = 3,

            startingResources = startingResourcesData[1],
            startingResourcesRichness = Richness.Abundant,

            startingSeason = SeasonController.Season.Summer,

            maximumChildren = 3,
            fertilityModifier = 1.0f,
            mortalityRate = 1.0f,

            pilgrimChance = 1.0f
        };

        StartingCondition hard = new StartingCondition ()
        {
            identifier = "Hard",
            startingMoney = 4000,
            startingFamilies = 2,

            startingResources = startingResourcesData[2],
            startingResourcesRichness = Richness.Sparse,

            startingSeason = SeasonController.Season.Winter,

            maximumChildren = 2,
            fertilityModifier = 0.5f,
            mortalityRate = 1.5f,

            pilgrimChance = 0.5f
        };

        StartingCondition custom = new StartingCondition ()
        {
            identifier = "Custom",
            startingMoney = 10000,
            startingFamilies = 0,

            startingResources = startingResourcesData[0],
            startingResourcesRichness = Richness.Plentiful,

            startingSeason = SeasonController.Season.Spring,

            maximumChildren = 4,
            fertilityModifier = 1.5f,
            mortalityRate = 0.5f,

            pilgrimChance = 1.5f
        };

        startingConditionsData.Add ( easy );
        startingConditionsData.Add ( medium );
        startingConditionsData.Add ( hard );
        startingConditionsData.Add ( custom );

        startingConditions = startingConditionsData[startingConditionIndex];
    }

    public void SetGameDataType (GameDataType type, string saveName)
    {
        this.gameDataType = type;
        CurrentSaveFileName = saveName;
    }

    public void SetStartingConditions(int index)
    {
        startingConditionIndex = index;
        startingConditions = startingConditionsData[index];
    }

    public void Load_StartingConditionsAsCustom (StartingCondition data)
    {
        int index = startingConditionsData.IndexOf ( startingConditionsData.First ( x => x.identifier == data.identifier ) );
        //Debug.Log ( index );
        if (index == 3)
        {
            SetStartingConditions ( 3 );
            ReturnCustomCondition ().identifier = "Custom";
            ReturnCustomCondition ().startingMoney = data.startingMoney;
            ReturnCustomCondition ().startingSeason = data.startingSeason;
            ReturnCustomCondition ().startingResourcesRichness = data.startingResourcesRichness;
            ReturnCustomCondition ().startingResources = data.startingResources;
            ReturnCustomCondition ().startingFamilies = data.startingFamilies;
            ReturnCustomCondition ().maximumChildren = data.maximumChildren;
            ReturnCustomCondition ().fertilityModifier = data.fertilityModifier;
            ReturnCustomCondition ().mortalityRate = data.mortalityRate;
            ReturnCustomCondition ().pilgrimChance = data.pilgrimChance;
        }
        else
        {
            SetStartingConditions ( index );
        }
    }

    [System.Serializable]
    public class StartingCondition
    {
        public string identifier = "";
        public int startingMoney;

        public SeasonController.Season startingSeason = SeasonController.Season.Summer;
        public Richness startingResourcesRichness = Richness.Abundant;
        public List<KeyValuePair<int, float>> startingResources = new List<KeyValuePair<int, float>> ();

        public int startingFamilies;
        public int maximumChildren;
        public float fertilityModifier;
        public float mortalityRate;

        public float pilgrimChance;

        //public StartingCondition (string identifier, int startingMoney, SeasonController.Season startingSeason, int maximumChildren, float fertilityModifier, float mortalityRate, int startingFamilies, float pilgrimChance)
        //{
        //    this.identifier = identifier;
        //    this.startingMoney = startingMoney;
        //    this.startingSeason = startingSeason;
        //    this.maximumChildren = maximumChildren;
        //    this.fertilityModifier = fertilityModifier;
        //    this.mortalityRate = mortalityRate;
        //    this.startingFamilies = startingFamilies;
        //    this.pilgrimChance = pilgrimChance;
        //}
    }
}
