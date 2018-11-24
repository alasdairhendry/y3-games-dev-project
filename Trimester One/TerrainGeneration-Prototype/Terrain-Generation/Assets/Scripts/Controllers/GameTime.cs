using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTime : MonoBehaviour {

    public static GameTime Instance;

    public static float DeltaGameTime { get { return gameTimeModifier * Time.deltaTime; } }

    private static float gameTimeModifier = 1.0f;
    public static float GameTimeModifier { get { return gameTimeModifier; } }

    [SerializeField] private float gameTickInterval = 0.1f;
    private float currentGameTickInterval = 0.0f;
    private static System.Action OnGameTick;

    private void Awake ()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy ( this.gameObject );
    }

    private void Update ()
    {        
        MonitorGameTick ();
        CheckGameTimeInput ();   
    }

    private void MonitorGameTick ()
    {
        currentGameTickInterval += Time.deltaTime;
        if(currentGameTickInterval >= gameTickInterval)
        {
            currentGameTickInterval = 0.0f;

            if (OnGameTick != null) OnGameTick ();
        }
    }

    private void CheckGameTimeInput ()
    {
        if (Input.GetKeyDown ( KeyCode.Period ))
        {
            if (gameTimeModifier == 1.0f) gameTimeModifier = 2.0f;
            else if (gameTimeModifier == 2.0f) gameTimeModifier = 3.0f;
            else if (gameTimeModifier == 3.0f) gameTimeModifier = 5.0f;
            else if (gameTimeModifier == 5.0f) gameTimeModifier = 1.0f;

            Debug.Log ( "Game Time Speed: " + gameTimeModifier );
        }
    }

    public static void RegisterGameTick (System.Action foo)
    {
        OnGameTick += foo;
    }

    public static void unRegisterGameTick (System.Action foo)
    {
        OnGameTick -= foo;
    }
}
