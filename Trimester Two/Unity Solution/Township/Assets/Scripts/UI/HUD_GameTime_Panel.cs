using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_GameTime_Panel : UIPanel {

    [SerializeField] private Slider gameTimeSlider;
    [SerializeField] private Text dataLabel;
    [SerializeField] private Text seasonLabel;
    [SerializeField] private Text temperatureLabel;

    [SerializeField] private Text gameTimeModifierLabel;

    protected override void Update ()
    {
        base.Update ();
        UpdateSlider ();
        UpdateDate ();
    }

    private void UpdateSlider ()
    {
        gameTimeSlider.value = GameTime.GetCurrentSecondsPercent;
    }

    private void UpdateDate ()
    {        
        dataLabel.text = "Day " + GameTime.currentDayOfTheMonth + " Month " + GameTime.currentMonth + " Year " + GameTime.currentYear + " (" + SeasonController.CurrentSeason.ToString () + ")";
        seasonLabel.text = SeasonController.CurrentSeason.ToString ();
        temperatureLabel.text = TemperatureController.Temperature.ToString ( "00" ) + "* C";
    }

    public void OnClick_SpeedDown ()
    {
        GameTime.ModifyGameSpeed (true);
        gameTimeModifierLabel.text = GameTime.GameTimeModifier.ToDescription ( new Dictionary<float, string> () { { 1.0f, "Normal" }, { 2.0f, "Fast" }, { 3.0f, "Faster" }, { 5.0f, "Fastest" } } );
    }

    public void OnClick_SpeedUp ()
    {
        GameTime.ModifyGameSpeed (false);
        gameTimeModifierLabel.text = GameTime.GameTimeModifier.ToDescription ( new Dictionary<float, string> () { { 1.0f, "Normal" }, { 2.0f, "Fast" }, { 3.0f, "Faster" }, { 5.0f, "Fastest" } } );
    }

    public void OnClick_PausePlay ()
    {
        GameTime.PausePlay ();
    }

}
