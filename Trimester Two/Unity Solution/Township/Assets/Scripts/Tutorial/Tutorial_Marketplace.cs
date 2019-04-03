using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Marketplace : Tutorial {

    public Tutorial_Marketplace ()
    {
        this.identifier = "Tutorial_Marketplace";
        this.header = "The Marketplace";
        this.stages = 3;
        displayStages = 1;

        this.currentStage = 0;
        this.isComplete = false;

        LoadTextAssets ();
        Begin ();
    }

    public override void Begin ()
    {
        if (hasBegun) return;

        hasBegun = true;
        SetStage ( 0 );
    }

    protected override void OnSetStage (int stage)
    {
        switch (stage)
        {
            case 0:
                EntityManager.Instance.onEntityCreated += OnEntityCreated;
                break;

            case 1:
                HUD_Tutorial_Panel.Instance.ShowTutorial ( this, 1, string.Format ( textAssets[0].text ), true );
                break;

            case 2:
                Complete ();
                break;

            default:
                break;
        }
    }

    private void OnEntityCreated(GameObject go)
    {
        if (go.GetComponent<Prop_MarketPlace> () != null)
        {
            if(currentStage == 0)
            {
                IncrementStage ();
                EntityManager.Instance.onEntityCreated -= OnEntityCreated;
            }
        }
    }
}
