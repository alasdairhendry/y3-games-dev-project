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
    private static System.Action OnGameTick;

    private static float secondsPerDay = 10.0f;
    private static float currentSeconds = 0.0f;
    public static float GetCurrentSecondsPercent { get { return currentSeconds / secondsPerDay; } }

    public static int currentDay { get; protected set; }
    public static int currentMonth { get; protected set; }
    public static int currentYear { get; protected set; }

    private void Start ()
    {
        currentDay = 1;
        currentMonth = 1;
        currentYear = 1;
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

            if (OnGameTick != null) OnGameTick ();
        }
    }

    private void MonitorGameTime ()
    {
        currentSeconds += DeltaGameTime;

        if (currentSeconds >= secondsPerDay)
        {
            currentSeconds = 0.0f;

            currentDay++;

            if (currentDay >= 32)
            {
                currentDay = 1;

                currentMonth++;

                if (currentMonth >= 13)
                {
                    currentMonth = 1;

                    currentYear++;
                }
            }
        }
    }

    public static void ModifyGameSpeed (bool down)
    {
        Debug.Log ( down );
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

    public static void RegisterGameTick (System.Action foo)
    {
        OnGameTick += foo;
    }

    public static void UnRegisterGameTick (System.Action foo)
    {
        OnGameTick -= foo;
    }
}
