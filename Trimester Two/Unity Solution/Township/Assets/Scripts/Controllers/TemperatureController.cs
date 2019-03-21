using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TemperatureController {

    public static Vector2 temperatureVariance { get; private set; } = new Vector2 ( -2f, 2f );

    public static float temperatureMin { get; private set; } = 10.0f;
    public static float temperatureMax { get; private set; } = 15.0f;
    public static float Temperature { get; private set; } = float.MinValue;

    //private static bool calcTmp = true;

    public static void CalculateTemperature ()
    {
        //if (!calcTmp) return;

        float timeOfYearPercentage = GameTime.currentDayOfTheYear / 372.0f;        
        float temperaturePercentage = GameData.Instance.TemperatureCurve.Evaluate ( timeOfYearPercentage );        
        float _temperature = Mathf.Lerp ( temperatureMin, temperatureMax, temperaturePercentage );        

        _temperature += Random.Range ( temperatureVariance.x, temperatureVariance.y );

        Temperature = _temperature;
    }

    public static void SetAverageTemperate(float min, float max)
    {
        temperatureMin = min;
        temperatureMax = max;
        CalculateTemperature ();
    }

    //public static void SetTemp(float amount)
    //{
    //    calcTmp = false;
    //    Temperature += amount;
    //    SnowController.Instance?.CheckTemperature ();
    //}
}
