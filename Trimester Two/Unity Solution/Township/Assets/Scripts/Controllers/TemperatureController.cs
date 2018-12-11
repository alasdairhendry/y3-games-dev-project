using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemperatureController : MonoBehaviour {

    public static TemperatureController Instance;

    [SerializeField] private AnimationCurve temperatureCurve;
    [SerializeField] private Vector2 temperatureVariance = new Vector2 ( -2f, 2f );

    private float temperatureMin = 0;
    private float temperatureMax = 0;

    private float temperature = float.MinValue;
    public float Temperature { get { return temperature; } }

    //public System.Action<float, float> onTemperatureChange;

    private void Awake ()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy ( this.gameObject );
    }

    private void Start ()
    {
        GameTime.onDayChanged += UpdateTemperature;

        if (temperature == float.MinValue)
            CalculateTemperature ();
    }

    private void UpdateTemperature (int previousDay, int currentDay) {
        //CalculateTemperature ();
    }
    
    private void CalculateTemperature ()
    {        
        float timeOfYearPercentage = GameTime.currentOverallDay / 372.0f;        
        float temperaturePercentage = temperatureCurve.Evaluate ( timeOfYearPercentage );        
        float _temperature = Mathf.Lerp ( temperatureMin, temperatureMax, temperaturePercentage );        

        _temperature += Random.Range ( temperatureVariance.x, temperatureVariance.y );

        Debug.Log ( "temp" + _temperature );
        //if (onTemperatureChange != null) onTemperatureChange ( temperature, _temperature );

        temperature = _temperature;
    }

    public void SetAverageTemperate(float min, float max)
    {
        temperatureMin = min;
        temperatureMax = max;
    }

    public void SetTemp (float amount)
    {
        temperature += amount;
    }
}
