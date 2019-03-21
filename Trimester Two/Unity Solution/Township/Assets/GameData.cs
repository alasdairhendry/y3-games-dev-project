using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoSingleton<GameData> {

    public enum GameDataType { New, Loaded }
    public GameDataType gameDataType { get; protected set; } = GameDataType.New;

    private List<StartingCondition> startingConditionsData = new List<StartingCondition> ();
    public StartingCondition startingConditions { get; protected set; }

    [SerializeField] [Range ( 0, 3 )] private int startingConditionIndex = 0;

    [SerializeField] private AnimationCurve temperatureCurve;
    public AnimationCurve TemperatureCurve { get { return temperatureCurve; } }

    private void Awake ()
    {
        startingConditionsData.Add ( new StartingCondition ( "Easy", SeasonController.Season.Spring ) );
        startingConditionsData.Add ( new StartingCondition ( "Medium", SeasonController.Season.Summer ) );
        startingConditionsData.Add ( new StartingCondition ( "Hard", SeasonController.Season.Winter ) );
        startingConditionsData.Add ( new StartingCondition ( "Custom", SeasonController.Season.Winter ) );
        startingConditions = startingConditionsData[startingConditionIndex];
    }

    public void SetGameDataType (GameDataType type)
    {
        this.gameDataType = type;
    }

    public class StartingCondition
    {
        public string identifier = "";
        public SeasonController.Season startingSeason = SeasonController.Season.Summer;

        public StartingCondition (string identifier, SeasonController.Season startingSeason)
        {
            this.identifier = identifier;
            this.startingSeason = startingSeason;
        }
    }
}
