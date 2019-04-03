public class Tutorial_GatheringResources : Tutorial
{
    public Tutorial_GatheringResources ()
    {
        this.identifier = "Tutorial_GatheringResources";
        this.header = "Gathering Resources";
        this.stages = 3;
        displayStages = 2;

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
                if (EntityManager.Instance.GetEntitiesByType ( typeof ( Prop_LumberjackHut ) ).Count <= 0)
                {
                    HUD_Tutorial_Panel.Instance.ShowTutorial ( this, 1, string.Format ( textAssets[0].text, Hotkey.GetData ( Hotkey.Function.ToggleProfessions ).GetCommandString ().RTBold () ), true );
                }
                else
                {
                    HUD_Tutorial_Panel.Instance.ShowTutorial ( this, 1, string.Format ( textAssets[1].text, Hotkey.GetData ( Hotkey.Function.ToggleProfessions ).GetCommandString ().RTBold () ), true );
                }              
                break;

            case 1:
                HUD_Tutorial_Panel.Instance.ShowTutorial ( this, 2, string.Format ( textAssets[2].text ), true );
                break;         

            case 2:
                Complete ();
                break;

            default:
                break;
        }
    }
}
