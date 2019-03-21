using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTime : MonoBehaviour
{
    public static float DeltaGameTime { get { return gameTimeModifier * Time.deltaTime; } }

    private static bool isPaused = false;
    public static bool IsPaused { get { return isPaused; } }

    private static float gameTimeModifier = 1.0f;
    public static float GameTimeModifier { get { return gameTimeModifier; } }

    [SerializeField] private float gameTickInterval = 0.1f;
    private float currentGameTickInterval = 0.0f;
    private static int currentRelativeTick = 0;
    private static System.Action<int> OnGameTick;

    private static float secondsPerDay = 10.0f;
    public static float SecondsPerDay { get { return secondsPerDay; } }

    private static float currentSeconds = 0.0f;
    public static float GetCurrentSecondsPercent { get { return currentSeconds / secondsPerDay; } }

    /// <summary>
    /// The day it is since the start of the game. This does not reset at all.
    /// </summary>
    public static int currentDayOfTheGame { get; protected set; }

    /// <summary>
    /// The day it is since the start of the year. This does not reset every month, but does reset every year.
    /// </summary>
    public static int currentDayOfTheYear { get; protected set; }

    /// <summary>
    /// The current day of the month that it is. This ranges from 1 to 31 (inclusive)
    /// </summary>
    public static int currentDayOfTheMonth { get; protected set; }

    // The current month - Ranges from 1 - 12
    /// <summary>
    /// The current month of the year. This ranges from 1 to 12 (inclusive)
    /// </summary>
    public static int currentMonth { get; protected set; }

    /// <summary>
    /// The current year it is. This starts from 1 and increments every 12 months
    /// </summary>
    public static int currentYear { get; protected set; }

    public static System.Action<int, int> onDayChanged;
    public static System.Action<int, int> onMonthChanged;
    public static System.Action<int, int> onYearChanged;

    // Range between 0 - 1 to determine which part of the year the game starts in.
    //private static float gameTimeStart = 0.95f; // Winter
    //private static float gameStartMonth = 2;

    private void Awake ()
    {
        if (GameData.Instance.gameDataType == GameData.GameDataType.New)
        {
            // TODO : remember to do this for Load data type
            onMonthChanged += SeasonController.UpdateSeason;

            onDayChanged += (v1, v2) => { TemperatureController.CalculateTemperature (); };
            onDayChanged += (v1, v2) => { SnowController.Instance?.CheckTemperature (); };

            currentDayOfTheMonth = 1;
            currentMonth = (int)Mathf.Clamp ( SeasonController.GetSeasonData ( GameData.Instance.startingConditions.startingSeason )[0], 1, 12 );
            currentYear = 1;

            currentDayOfTheYear = Mathf.RoundToInt ( 372.0f * ((float)currentMonth / 12.0f) ) - 30;
            currentDayOfTheGame = currentDayOfTheYear;

            TemperatureController.CalculateTemperature ();
            onMonthChanged?.Invoke ( StaticExtensions.Loop ( currentMonth - 1, 1, 12 ), currentMonth );
        }
        else
        {
            // Wait for saveData to send us info
        }
    }

    public static void Load (PersistentData.GameTimeData data)
    {
        onMonthChanged += SeasonController.UpdateSeason;

        onDayChanged += (v1, v2) => { TemperatureController.CalculateTemperature (); };
        onDayChanged += (v1, v2) => { SnowController.Instance?.CheckTemperature (); };

        currentDayOfTheMonth = data.dayOfMonth;
        currentMonth = data.month;
        currentYear = data.year;

        currentDayOfTheYear = data.dayOfYear;
        currentDayOfTheGame = data.dayOfGame;

        TemperatureController.CalculateTemperature ();
        onMonthChanged?.Invoke ( StaticExtensions.Loop ( currentMonth - 1, 1, 12 ), currentMonth );
    }

    private void Update ()
    {
        MonitorGameTick ();

        MonitorGameTime ();

        //if (Input.GetKeyDown ( KeyCode.Equals )) TemperatureController.SetTemp ( 1 );
        //if (Input.GetKeyDown ( KeyCode.Minus )) TemperatureController.SetTemp ( -1 );
    }

    private void MonitorGameTick ()
    {
        currentGameTickInterval += Time.deltaTime;
        if (currentGameTickInterval >= gameTickInterval)
        {
            currentGameTickInterval = 0.0f;
            currentRelativeTick++;

            if (currentRelativeTick > 999)
                currentRelativeTick = 0;

            if (OnGameTick != null) OnGameTick (currentRelativeTick);
        }
    }

    private void MonitorGameTime ()
    {
        currentSeconds += DeltaGameTime;

        if (currentSeconds >= secondsPerDay)
        {
            currentSeconds = 0.0f;

            CheckGameTime ();
        }
    }

    private void CheckGameTime ()
    {
        if (currentDayOfTheMonth < 31)
        {
            onDayChanged?.Invoke ( currentDayOfTheMonth, currentDayOfTheMonth + 1 );
            currentDayOfTheMonth++;
            currentDayOfTheYear++;
        }
        else 
        {
            onDayChanged?.Invoke ( currentDayOfTheMonth - 1, 1 );
            currentDayOfTheMonth = 1;
            currentDayOfTheYear++;

            if(currentMonth < 12)
            {
                onMonthChanged?.Invoke ( currentMonth, currentMonth + 1 );
                currentMonth++;
            }
            else
            {
                onMonthChanged?.Invoke ( 12, 1 );
                currentMonth = 1;

                onYearChanged?.Invoke ( currentYear, currentYear + 1 );
                currentYear++;

                Debug.Log ( string.Format ( "Year finished - {0} days complete", currentDayOfTheYear ) );
                currentDayOfTheYear = 1;
            }
        }        
    }

    public static void ModifyGameSpeed (bool down)
    {
        if (!down)
        {
            if (gameTimeModifier == 1.0f) gameTimeModifier = 2.0f;
            else if (gameTimeModifier == 2.0f) gameTimeModifier = 3.0f;
            else if (gameTimeModifier == 3.0f) gameTimeModifier = 5.0f;
        }
        else
        {
            if (gameTimeModifier == 2.0f) gameTimeModifier = 1.0f;
            else if (gameTimeModifier == 3.0f) gameTimeModifier = 2.0f;
            else if (gameTimeModifier == 5.0f) gameTimeModifier = 3.0f;
        }
    }

    public static void PausePlay ()
    {
        isPaused = !isPaused;
    }

    public static void RegisterGameTick (System.Action<int> foo)
    {
        OnGameTick += foo;
    }

    public static void UnRegisterGameTick (System.Action<int> foo)
    {
        OnGameTick -= foo;
    }

    private void OnDestroy ()
    {
        onMonthChanged -= SeasonController.UpdateSeason;

    }
}
