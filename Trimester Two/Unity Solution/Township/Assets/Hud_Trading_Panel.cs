using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hud_Trading_Panel : UIPanel
{
    public static Hud_Trading_Panel Instance;

    private void Awake ()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy ( this.gameObject );
    }

    [SerializeField] private Transform content;

    [SerializeField] private TMP_Text traderName;

    [SerializeField] private GameObject prefab_Header;
    [SerializeField] private GameObject prefab_Group;
    [SerializeField] private GameObject prefab_Descriptions;
    [SerializeField] private GameObject prefab_Entry;
    [SerializeField] private GameObject prefab_Buttons;
    [SerializeField] private GameObject prefab_Seperator;

    private Trader currentTrader;
    private List<TradeProposalGroup> currentDeals = new List<TradeProposalGroup> ();

    public override void Show ()
    {
        base.Show ();
    }

    public override void Hide ()
    {
        base.Hide ();
    }

    public void SetDeal (List<TradeProposalGroup> groups, Trader trader)
    {
        if (currentDeals.Count > 0) return;

        DestroyPreviousDealObjects ();

        currentDeals = groups;
        currentTrader = trader;

        traderName.text = (currentTrader.name).RTBlack().RTBold() + "\nwould like to trade with you";

        if (currentDeals.Count == 0)
        {
            OnDealFinished ();
        }

        for (int i = 0; i < groups.Count; i++)
        {
            GameObject seperator = null;

            if (i != 0)
            {
                seperator = Instantiate ( prefab_Seperator, content );
            }

            GameObject header = Instantiate ( prefab_Header, content );
            header.GetComponent<TMP_Text> ().text = "Deal " + (i + 1).ToString ( "00" );

            GameObject group = Instantiate ( prefab_Group, content );
            GameObject descriptions = Instantiate ( prefab_Descriptions, group.transform );
            TMP_Text[] textDescriptions = descriptions.GetComponentsInChildren<TMP_Text> ();
            textDescriptions[0].text = "Item";
            textDescriptions[1].text = "Description";
            textDescriptions[2].text = "#";
            textDescriptions[3].text = "each";
            textDescriptions[4].text = "total";

            for (int x = 0; x < groups[i].entries.Count; x++)
            {
                GameObject entry = Instantiate ( prefab_Entry, group.transform );

                entry.transform.Find ( "Image" ).GetComponent<Image> ().sprite = ResourceManager.Instance.GetResourceByID ( groups[i].entries[x].itemID ).image;

                TMP_Text[] textEntries = entry.GetComponentsInChildren<TMP_Text> ();

                float price = 0;

                if (groups[i].entries[x].tradeType == TradingController.TradeType.Buy)
                {
                    textEntries[0].text = "Buy " + ResourceManager.Instance.GetResourceByID ( groups[i].entries[x].itemID ).name + " from merchant";
                    price = ResourceManager.Instance.GetResourceByID ( groups[i].entries[x].itemID ).baseBuyPrice;
                }
                else
                {
                    textEntries[0].text = "Sell " + ResourceManager.Instance.GetResourceByID ( groups[i].entries[x].itemID ).name + " to merchant";
                    price = ResourceManager.Instance.GetResourceByID ( groups[i].entries[x].itemID ).baseSellPrice;
                }

                textEntries[1].text = groups[i].entries[x].itemQuantity.ToString ( "00" );
                textEntries[2].text = Mathf.FloorToInt ( (price * groups[i].entries[x].inflation) ).ToString ( "00" );

                if (groups[i].entries[x].tradeType == TradingController.TradeType.Buy)
                    textEntries[3].text = "<color=red>" + Mathf.FloorToInt ( groups[i].entries[x].itemQuantity * price * groups[i].entries[x].inflation ).ToString ( "00" ) + "</color>";
                else
                    textEntries[3].text = "<color=green>" + Mathf.FloorToInt ( groups[i].entries[x].itemQuantity * price * groups[i].entries[x].inflation ).ToString ( "00" ) + "</color>";
            }

            GameObject buttonsPanel = Instantiate ( prefab_Buttons, group.transform );

            if (groups[i].overallCost >= 0)
                buttonsPanel.transform.Find ( "OverallCost_Text" ).GetComponent<TMP_Text> ().text = "This deal will cost you <color=red>" + MoneyController.BeautifyMoney ( groups[i].overallCost ) + "</color> gold.";
            else
                buttonsPanel.transform.Find ( "OverallCost_Text" ).GetComponent<TMP_Text> ().text = "You will earn <color=green>" + MoneyController.BeautifyMoney ( groups[i].overallCost * -1 ) + "</color> gold from this deal.";

            Button[] buttons = buttonsPanel.GetComponentsInChildren<Button> ();

            int y = i;
            TradeProposalGroup deal = groups[y];

            buttons[0].onClick.AddListener ( () => { AcceptDeal ( deal, true, () => { Destroy ( group ); Destroy ( header ); Destroy ( seperator ); }, true ); } );
            buttons[1].onClick.AddListener ( () => { DeclineDeal ( deal, () => { Destroy ( group ); Destroy ( header ); Destroy ( seperator ); }, true ); } );
        }
    }

    private void DestroyPreviousDealObjects ()
    {
        for (int i = 0; i < content.childCount; i++)
        {
            Destroy ( content.GetChild ( i ).gameObject );
        }
    }

    private bool AcceptDeal (TradeProposalGroup deal, bool showDialogue, System.Action destroy, bool removeFromDeals)
    {
        Debug.Log ( "Accepting deal " + deal.overallCost );
        if (MoneyController.Instance.CanAfford ( deal.overallCost ))
        {
            for (int i = 0; i < deal.entries.Count; i++)
            {
                if (deal.entries[i].tradeType == TradingController.TradeType.Buy)
                {
                    WarehouseController.Instance.Inventory.AddItemQuantity ( deal.entries[i].itemID, deal.entries[i].itemQuantity );
                    MoneyController.Instance.RemoveWithTransaction ( deal.overallCost, "Traded with " + currentTrader.name, MoneyController.Transaction.Category.Trading );
                }
                else
                {
                    WarehouseController.Instance.Inventory.TakeReservedItemQuantity ( deal.entries[i].itemID, deal.entries[i].itemQuantity );
                    MoneyController.Instance.AddWithTransaction ( deal.overallCost, "Traded with " + currentTrader.name, MoneyController.Transaction.Category.Trading );
                }
            }

            if (removeFromDeals)
            {
                destroy?.Invoke ();
                currentDeals.Remove ( deal );
                CheckDeal ();
            }

            return true;
        }
        else
        {
            if (showDialogue)
                HUD_Dialogue_Panel.Instance.ShowDialogue ( "Not Enough Gold", "You do not have enough gold to afford this deal. Consider other deals first or decline this one.", new DialogueButton ( DialogueButton.Preset.Okay, null ) );

            return false;
        }
    }

    private void DeclineDeal (TradeProposalGroup deal, System.Action destroy, bool removeFromDeals)
    {
        Debug.Log ( "Declining deal " + deal.overallCost );

        for (int i = 0; i < deal.entries.Count; i++)
        {
            if (deal.entries[i].tradeType == TradingController.TradeType.Sell)
            {
                WarehouseController.Instance.Inventory.TakeReservedItemQuantity ( deal.entries[i].itemID, deal.entries[i].itemQuantity );
                WarehouseController.Instance.Inventory.AddItemQuantity ( deal.entries[i].itemID, deal.entries[i].itemQuantity );
            }
        }

        if (removeFromDeals)
        {
            destroy?.Invoke ();
            currentDeals.Remove ( deal );
            CheckDeal ();
        }
    }

    private void CheckDeal ()
    {
        if (currentDeals.Count <= 0)
        {
            OnDealFinished ();
        }
    }

    private void OnDealFinished ()
    {
        DestroyPreviousDealObjects ();
        currentDeals = new List<TradeProposalGroup> ();
        currentTrader = null;
        Hide ();
    }

    public void OnClick_All (bool accept)
    {
        if (currentDeals.Count <= 0) return;

        if (accept)
        {
            bool dealsUnfinished = false;
            for (int i = 0; i < currentDeals.Count; i++)
            {
                if (!AcceptDeal ( currentDeals[i], false, null, false ))
                {
                    dealsUnfinished = true;
                }
            }

            if (dealsUnfinished)
            {
                HUD_Dialogue_Panel.Instance.ShowDialogue ( "Some Deals Missed", "You did not have enough gold to accept every deal. These deals have been declined.", new DialogueButton ( DialogueButton.Preset.Okay, null ) );
            }
        }
        else
        {
            for (int i = 0; i < currentDeals.Count; i++)
            {
                DeclineDeal ( currentDeals[i], null, false );
            }
        }

        OnDealFinished ();
    }
}
