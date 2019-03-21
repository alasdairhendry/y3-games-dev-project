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
        Debug.Log ( "SetShowInventoryPopups " + state );
        preferences.showInventoryPopups = state;
        onChange?.Invoke ();
    }
	
    [System.Serializable]
    public class Preferences
    {
        public bool showCitizenIcons = true;
        public bool showPropIcons = true;
        public bool showInventoryPopups = true;
    }
}
