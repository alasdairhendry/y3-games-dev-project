using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop_MarketPlace : Prop {

    protected override void OnPlaced ()
    {
        base.OnPlaced ();

        SetInspectable ();
    }

    protected override void SetInspectable ()
    {
        base.SetInspectable ();

        Inspectable.SetDestroyAction ( () =>
        {
            if (EntityManager.Instance.GetEntitiesByType ( this.GetType () ).Count <= 1)
            {
                HUD_Dialogue_Panel.Instance.ShowDialogue ( "Are you sure?", 
                                                            "This is your only MarketPlace.\n" +
                                                            "Without a MarketPlace you will be unable to trade with Merchants.\n\n" +
                                                            "Destroy the MarketPlace?",
                    new DialogueButton ( DialogueButton.Preset.Yes, () => { DestroyProp (); } ),
                    new DialogueButton ( DialogueButton.Preset.No, null ) );
                return;
            }

            DestroyProp ();
        }, false, "Bulldoze" );
        
        Inspectable.SetAdditiveAction ( () =>
        {
            HUD_EntityInspection_Citizen_Panel panel = FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ();

            panel.AddButtonData ( () =>
            {
                TradingController.Instance?.SetMarketPlaceTrading ( !TradingController.Instance.tradingIsActive );

                if (TradingController.Instance?.tradingIsActive == true)
                    HUD_Notification_Panel.Instance.AddNotification ( "Merchants will now visit your Marketplaces", HUD_Notification_Panel.NotificationSprite.Information, this.Inspectable );
                else 
                    HUD_Notification_Panel.Instance.AddNotification ( "Merchants will no longer visit your Marketplaces", HUD_Notification_Panel.NotificationSprite.Warning, this.Inspectable );

            }, "Toggle Trading", "Any" );            
        } );
    }
}
