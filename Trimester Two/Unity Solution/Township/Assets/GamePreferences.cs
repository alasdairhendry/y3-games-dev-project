using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePreferences : MonoSingleton<GamePreferences> {

    public Preferences preferences { get; protected set; }
    public System.Action onChange;

    public override void Init ()
    {
        preferences = new Preferences ();
    }

    public void Load(Preferences data)
    {
        preferences = data;
    }

    public void ShowCitizenIcons(bool state)
    {
        preferences.showCitizenIcons = state;
        onChange?.Invoke ();
    }

    public void ShowPropIcons(bool state)
    {
        preferences.showPropIcons = state;
        onChange?.Invoke ();
    }

    public void ShowInventoryPopups (bool state)
    {
        preferences.showInventoryPopups = state;
        onChange?.Invoke ();
    }

    public void showNotification_Birthday (bool state)
    {
        preferences.showNotification_Birthday = state;
        onChange?.Invoke ();
    }

    public void showNotification_SpecialBirthday (bool state)
    {
        preferences.showNotification_SpecialBirthday = state;
        onChange?.Invoke ();
    }

    public void showNotification_Pregnancy (bool state)
    {
        preferences.showNotification_Pregnancy = state;
        onChange?.Invoke ();
    }

    public void showNotification_Birth (bool state)
    {
        preferences.showNotification_Birth = state;
        onChange?.Invoke ();
    }

    [System.Serializable]
    public class Preferences
    {
        public bool showCitizenIcons = true;
        public bool showPropIcons = true;
        public bool showInventoryPopups = true;

        public bool showNotification_Birthday = true;
        public bool showNotification_SpecialBirthday = true;
        public bool showNotification_Pregnancy = true;
        public bool showNotification_Birth = true;
    }
}
