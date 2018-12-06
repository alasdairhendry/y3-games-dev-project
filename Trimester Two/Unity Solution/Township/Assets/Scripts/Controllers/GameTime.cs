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
    private static float currentSeconds = 0.0f;
    public static float GetCurrentSecondsPercent { get { return currentSeconds / secondsPerDay; } }

    // The current day of the year - Ranges from 1 - 365
    public static int currentOverallDay { get; protected set; }

    // The current day of the month - Ranges from 1 - 31
    public static int currentDay { get; protected set; }

    // The current month - Ranges from 1 - 12
    public static int currentMonth { get; protected set; }

    // The current year - Ranges from 1 - inf
    public static int currentYear { get; protected set; }

    public static System.Action<int, int> onDayChanged;
    public static System.Action<int, int> onMonthChanged;
    public static System.Action<int, int> onYearChanged;

    // Range between 0 - 1 to determine which part of the year the game starts in.
    private static float gameTimeStart = 0.95f;

    private void Start ()
    {
        Debug.Log ( "lerp " + Mathf.Lerp ( 1, -1, 0.33f ) );
        currentDay = 1;
        currentMonth = 1;
        currentYear = 1;

        SetStartTime ();
    }

    private void SetStartTime ()
    {
        float daysPerYear = 372;
        currentDay = Mathf.RoundToInt ( daysPerYear * gameTimeStart );
        currentOverallDay = currentDay;
        onDayChanged ( currentDay - 1, currentDay );
        currentMonth = Mathf.RoundToInt(currentDay / 31.0f);
        CheckGameTime ();
    }

    private void Update ()
    {
        MonitorGameTick ();
        MonitorGameTime ();
    }

    private void MonitorGameTick ()
    {
        currentGameTickInterval += Time.deltaTime;
        if (currentGameTickInterval >= gameTickInterval)
        {
            currentGameTickInterval = 0.0f;
            currentRelativeTick++;

            if (currentRelativeTick > 19)
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
        if (onDayChanged != null) onDayChanged ( currentDay, currentDay + 1 );
        currentDay++;
        currentOverallDay++;


        if (currentDay >= 32)
        {
            if (onDayChanged != null) onDayChanged ( currentDay - 1, 1 );
            currentDay = 1;

            if (onMonthChanged != null) onMonthChanged ( currentMonth, currentMonth + 1 );
            currentMonth++;

            if (currentMonth >= 13)
            {
                if (onMonthChanged != null) onMonthChanged ( currentMonth - 1, 1 );
                currentMonth = 1;

                if (onYearChanged != null) onYearChanged ( currentYear, currentYear + 1 );
                currentYear++;

                currentOverallDay = 1;
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
}
