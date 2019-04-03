using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_PlaceInitialProps : Tutorial
{
    public Tutorial_PlaceInitialProps ()
    {
        this.identifier = "Tutorial_PlaceInitialProps";
        this.header = "Placing Your First Buildings";
        this.stages = 7;
        this.displayStages = 6;

        this.currentStage = 0;
        this.isComplete = false;

        LoadTextAssets ();
    }

    public override void Begin ()
    {
        SetStage ( 0 );
    }

    protected override void OnSetStage (int stage)
    {
        switch (stage)
        {
            case 0:
                EntityManager.Instance.onEntityCreated += OnEntityCreated;
                HUD_Tutorial_Panel.Instance.ShowTutorial ( this, 1, string.Format ( textAssets[0].text, GameData.Instance.CurrentSaveFileName.RTBold ().RTBlack () ), true );
                break;

            case 1:
                HUD_Tutorial_Panel.Instance.ShowTutorial ( this, 2, string.Format ( textAssets[1].text ), true );
                break;

            case 2:
                HUD_Tutorial_Panel.Instance.ShowTutorial ( this, 3, string.Format ( textAssets[2].text ), false );
                break;

            case 3:
                HUD_Tutorial_Panel.Instance.ShowTutorial ( this, 4, string.Format ( textAssets[3].text ), false );
                break;

            case 4:
                HUD_Tutorial_Panel.Instance.ShowTutorial ( this, 5, string.Format ( textAssets[4].text, Hotkey.GetData ( Hotkey.Function.ToggleResources ).GetCommandString ().RTBold ().RTBlack (), GameData.Instance.startingConditions.startingFamilies.ToString ( "0" ) ), false );
                break;

            case 5:
                HUD_Tutorial_Panel.Instance.ShowTutorial ( this, 6, string.Format ( textAssets[5].text, Hotkey.GetData ( Hotkey.Function.ToggleJobs ).GetCommandString ().RTBold ().RTBlack () ), true );
                break;

            case 6:
                Complete ();
                break;

            default:
                break;
        }
    }

    private void OnEntityCreated (GameObject go)
    {
        if (go.GetComponent<Prop_Townpost> () != null)
        {
            if (currentStage < 2) SetStage ( 3 );
            else if (currentStage == 2) IncrementStage ();

            return;
        }

        if (go.GetComponent<Prop_Warehouse> () != null)
        {
            if (currentStage < 3) SetStage ( 4 );
            else if (currentStage == 3) IncrementStage ();

            return;
        }

        if (go.GetComponent<Prop_House> () != null)
        {
            if (currentStage < 4) SetStage ( 5 );
            else if (currentStage == 4) IncrementStage ();

            return;
        }
    }

    public override void Complete ()
    {
        base.Complete ();

        EntityManager.Instance.onEntityCreated -= OnEntityCreated;
        TutorialController.Instance.Begin ( 1 );
    }
}
