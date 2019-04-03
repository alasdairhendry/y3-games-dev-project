using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Trading : Tutorial {

    private string traderName;

    public Tutorial_Trading ()
    {
        this.identifier = "Tutorial_Trading";
        this.header = "Trading & Merchants";
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
                TradingController.Instance.onTradeBegin += OnTradeBegin;
                break;

            case 1:
                HUD_Tutorial_Panel.Instance.ShowTutorial ( this, 1, string.Format ( textAssets[0].text, traderName.RTBlack ().RTBold (), GameData.Instance.CurrentSaveFileName.RTBold ().RTBlack () ), true );
                break;

            case 2:
                Complete ();
                break;

            default:
                break;
        }
    }

    private void OnTradeBegin (List<TradeProposalGroup> tradeGroups, Trader trader)
    {
        if(currentStage == 0)
        {
            traderName = trader.name;
            IncrementStage ();
            TradingController.Instance.onTradeBegin -= OnTradeBegin;
        }       
    }

}
