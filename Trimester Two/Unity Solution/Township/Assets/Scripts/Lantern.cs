using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lantern : MonoBehaviour {

    private Light lanternLight;
    private float defaultIntensity = 0;
    private float damping = 0.0f;
    private bool lanternShouldbeOn = false;

    private void Awake ()
    {
        lanternLight = GetComponentInChildren<Light> ();       
        defaultIntensity = lanternLight.intensity;
        lanternLight.intensity = 0.0f;

        SetUsingLantern ( SunController.Instance.time );
        SunController.Instance.onSwitch += SetUsingLantern;
    }

    private void Update ()
    {
        UpdateLanternWeight ();
    }

    private void UpdateLanternWeight ()
    {
        if (lanternShouldbeOn)
        {
            if (damping == 1.0f) return;

            damping += GameTime.DeltaGameTime;

            if (damping >= 1.0f) damping = 1.0f;

            lanternLight.intensity = damping * defaultIntensity;
        }
        else
        {
            if (damping == 0.0f) return;

            damping -= GameTime.DeltaGameTime;

            if (damping <= 0.0f) damping = 0.0f;

            lanternLight.intensity = damping * defaultIntensity;
        }
    }

    private void SetUsingLantern (SunController.Time time)
    {
        if (time == SunController.Time.Day)
        {
            lanternShouldbeOn = false;
        }
        else
        {
            lanternShouldbeOn = true;
        }
    }

    private void OnDestroy ()
    {
        SunController.Instance.onSwitch -= SetUsingLantern;
    }
}
