using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunController : MonoBehaviour {

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
    }
}
