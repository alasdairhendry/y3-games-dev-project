using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD_Toolbar_Panel : UIPanel {

    protected override void Start ()
    {
        base.Start ();        
    }

    public override void Show ()
    {
        base.Show ();
    }

    public override void Toggle ()
    {
        base.Toggle ();
    }

    public override void Hide ()
    {
        base.Hide ();
    }

    public void OnClick_Save ()
    {
        if (string.IsNullOrEmpty ( GameData.Instance.CurrentSaveFileName ))
        {
            Debug.LogError ( "CANNOT SAVE FROM TOOLBAR WHEN NO GAME IS ACTIVE" );
            return;
        }

        SaveLoad.Instance.Save ();
    }

    public void OnClick_Quit ()
    {

    }
}
