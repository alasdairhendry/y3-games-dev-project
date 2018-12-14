using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunController : MonoBehaviour {

    public static SunController Instance;

    private void Awake ()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy ( this.gameObject );
    }

    public enum Time { Day, Night }
    public Time time { get; protected set; }
    [SerializeField] private int dayTimeStarts = 1;
    [SerializeField] private int dayTimeEnds = 21;

    [SerializeField] private Color dayColour;
    [SerializeField] private Color nightColour;
    [SerializeField] private float dayIntensity;
    [SerializeField] private float nightIntensity;

    private new Light light;
    private bool isNightTime = false;
    private bool switching = false;

    [SerializeField] private float switchTime = 1.0f;
    private float currentSwitchCounter = 0.0f;

    public System.Action<Time> onSwitch;

    private void Start ()
    {
        light = GetComponent<Light> ();
    }

    private void Update ()
    {
        if(GameTime.currentDay >= dayTimeEnds)
        {
            if (!isNightTime)
            {
                isNightTime = true;
                switching = true;                
            }
        }
        else
        {
            if (isNightTime)
            {
                isNightTime = false;
                switching = true;                
            }
        }

        SwitchToDayTime ();
        SwitchToNightTime ();
    }

    private void SwitchToDayTime ()
    {
        if (!switching) return;
        if (isNightTime) return;

        currentSwitchCounter += GameTime.DeltaGameTime;

        light.color = Color.Lerp ( nightColour, dayColour, currentSwitchCounter / switchTime );
        light.intensity = Mathf.Lerp ( nightIntensity, dayIntensity, currentSwitchCounter / switchTime );

        if (currentSwitchCounter >= switchTime)
        {
            currentSwitchCounter = 0.0f;
            switching = false;
        }

        time = Time.Day;
        if (onSwitch != null) onSwitch ( time );
    }

    private void SwitchToNightTime ()
    {
        if (!switching) return;
        if (!isNightTime) return;

        currentSwitchCounter += GameTime.DeltaGameTime;

        light.color = Color.Lerp ( dayColour, nightColour, currentSwitchCounter / switchTime );
        light.intensity = Mathf.Lerp ( dayIntensity, nightIntensity, currentSwitchCounter / switchTime );

        if (currentSwitchCounter >= switchTime)
        {
            currentSwitchCounter = 0.0f;
            switching = false;
        }

        time = Time.Night;
        if (onSwitch != null) onSwitch ( time );
    }    
}
