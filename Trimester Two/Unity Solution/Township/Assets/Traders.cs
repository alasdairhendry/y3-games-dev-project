using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traders : MonoSingleton<Traders> {

    public List<Trader> traders { get; protected set; } = new List<Trader> ();

    public override void Init ()
    {
        CreateTraders ();
    }

    private void CreateTraders ()
    {
        CreateTrader ( () =>
        {
            List<TraderItem> items = new List<TraderItem>
            {

                // Will sell these to the player, even though the trade type is buy, its from the players perspective.
                new TraderItem ( TradingController.TradeType.Buy, 0, 1.0f, 10.0f, 1.5f ),
                new TraderItem ( TradingController.TradeType.Buy, 1, 1.0f, 10.0f, 1.5f ),
                new TraderItem ( TradingController.TradeType.Buy, 5, 1.0f, 10.0f, 1.5f ),
                new TraderItem ( TradingController.TradeType.Buy, 9, 1.0f, 10.0f, 1.5f )
            };

            traders.Add ( new Trader ( "Ali Morrisane", items ) );
        } );

        CreateTrader ( () =>
        {
            List<TraderItem> items = new List<TraderItem>
            {
                // Will sell these to the player, even though the trade type is buy, its from the players perspective.
                new TraderItem ( TradingController.TradeType.Buy, 5, 1.0f, 10.0f, 0.8f )
            };

            traders.Add ( new Trader ( "Awowogei", items ) );
        } );

        CreateTrader ( () =>
        {
            List<TraderItem> items = new List<TraderItem>
            {
                // Will sell these to the player, even though the trade type is buy, its from the players perspective.
                new TraderItem ( TradingController.TradeType.Buy, 2, 1.0f, 10.0f, 1.0f ),
                new TraderItem ( TradingController.TradeType.Buy, 3, 1.0f, 10.0f, 1.0f ),
                new TraderItem ( TradingController.TradeType.Buy, 4, 1.0f, 10.0f, 1.0f ),
                new TraderItem ( TradingController.TradeType.Buy, 10, 1.0f, 10.0f, 0.8f ),

                // Will buy these from the player, even though the trade type is sell, its from the players perspective.
                new TraderItem ( TradingController.TradeType.Sell, 7, 1.0f, 10.0f, 1.0f )
            };

            traders.Add ( new Trader ( "Cap'n Izzy No-Beard", items ) );
        } );

        CreateTrader ( () =>
        {
            List<TraderItem> items = new List<TraderItem>
            {
                // Will sell these to the player, even though the trade type is buy, its from the players perspective.
                new TraderItem ( TradingController.TradeType.Buy, 9, 1.0f, 10.0f, 0.9f )
            };

            traders.Add ( new Trader ( "Bob Smith", items ) );
        } );

        CreateTrader ( () =>
        {
            List<TraderItem> items = new List<TraderItem>
            {
                // Will sell these to the player, even though the trade type is buy, its from the players perspective.
                new TraderItem ( TradingController.TradeType.Buy, 3, 1.0f, 10.0f, 1.0f ),
                new TraderItem ( TradingController.TradeType.Buy, 4, 1.0f, 10.0f, 1.0f ),
                new TraderItem ( TradingController.TradeType.Buy, 5, 1.0f, 10.0f, 1.0f )
            };

            traders.Add ( new Trader ( "Doric", items ) );
        } );

        CreateTrader ( () =>
        {
            List<TraderItem> items = new List<TraderItem>
            {
                // Will buy these from the player, even though the trade type is sell, its from the players perspective.
                new TraderItem ( TradingController.TradeType.Sell, 10, 1.0f, 10.0f, 1.0f )
            };

            traders.Add ( new Trader ( "Drezel The Monk", items ) );
        } );

        CreateTrader ( () =>
        {
            List<TraderItem> items = new List<TraderItem>
            {
                // Will sell these to the player, even though the trade type is buy, its from the players perspective.
                new TraderItem ( TradingController.TradeType.Buy, 7, 1.0f, 10.0f, 0.8f )
            };

            traders.Add ( new Trader ( "Fortunato", items ) );
        } );

        CreateTrader ( () =>
        {
            List<TraderItem> items = new List<TraderItem>
            {
                // Will sell these to the player, even though the trade type is buy, its from the players perspective.
                new TraderItem ( TradingController.TradeType.Buy, 0, 1.0f, 10.0f, 0.9f ),
                new TraderItem ( TradingController.TradeType.Buy, 1, 1.0f, 10.0f, 0.9f ),
                new TraderItem ( TradingController.TradeType.Buy, 2, 1.0f, 10.0f, 0.9f )
            };

            traders.Add ( new Trader ( "Father Urhney", items ) );
        } );

        CreateTrader ( () =>
        {
            List<TraderItem> items = new List<TraderItem>
            {
                // Will buy these from the player, even though the trade type is sell, its from the players perspective.
                new TraderItem ( TradingController.TradeType.Sell, 4, 1.0f, 10.0f, 0.95f ),
                new TraderItem ( TradingController.TradeType.Sell, 7, 1.0f, 10.0f, 0.95f ),
                new TraderItem ( TradingController.TradeType.Sell, 8, 1.0f, 10.0f, 0.95f ),
                new TraderItem ( TradingController.TradeType.Sell, 10, 1.0f, 10.0f, 0.95f )
            };

            traders.Add ( new Trader ( "Duke Horacio", items ) );
        } );

        CreateTrader ( () =>
        {
            List<TraderItem> items = new List<TraderItem>
            {
                // Will buy these from the player, even though the trade type is sell, its from the players perspective.
                new TraderItem ( TradingController.TradeType.Sell, 4, 1.0f, 10.0f, 0.95f ),
                new TraderItem ( TradingController.TradeType.Sell, 7, 1.0f, 10.0f, 0.95f ),
                new TraderItem ( TradingController.TradeType.Sell, 8, 1.0f, 10.0f, 0.95f ),
                new TraderItem ( TradingController.TradeType.Sell, 10, 1.0f, 10.0f, 0.95f )
            };

            traders.Add ( new Trader ( "King Arthur", items ) );
        } );

        CreateTrader ( () =>
        {
            List<TraderItem> items = new List<TraderItem>
            {
                // Will buy these from the player, even though the trade type is sell, its from the players perspective.
                new TraderItem ( TradingController.TradeType.Sell, 4, 1.0f, 10.0f, 0.95f ),
                new TraderItem ( TradingController.TradeType.Sell, 7, 1.0f, 10.0f, 0.95f ),
                new TraderItem ( TradingController.TradeType.Sell, 8, 1.0f, 10.0f, 0.95f ),
                new TraderItem ( TradingController.TradeType.Sell, 10, 1.0f, 10.0f, 0.95f )
            };

            traders.Add ( new Trader ( "King Lathas", items ) );
        } );

        CreateTrader ( () =>
        {
            List<TraderItem> items = new List<TraderItem>
            {
                // Will buy these from the player, even though the trade type is sell, its from the players perspective.
                new TraderItem ( TradingController.TradeType.Sell, 4, 1.0f, 10.0f, 0.95f ),
                new TraderItem ( TradingController.TradeType.Sell, 7, 1.0f, 10.0f, 0.95f ),
                new TraderItem ( TradingController.TradeType.Sell, 8, 1.0f, 10.0f, 0.95f ),
                new TraderItem ( TradingController.TradeType.Sell, 10, 1.0f, 10.0f, 0.95f )
            };

            traders.Add ( new Trader ( "King Roald", items ) );
        } );

        CreateTrader ( () =>
        {
            List<TraderItem> items = new List<TraderItem>
            {
                // Will sell these to the player, even though the trade type is buy, its from the players perspective.
                new TraderItem ( TradingController.TradeType.Buy, 0, 1.0f, 5.0f, 0.5f )
            };

            traders.Add ( new Trader ( "A Monk of Entrana", items ) );
        } );
    }

    private void CreateTrader(System.Action addAction)
    {
        addAction?.Invoke ();
    }

    public Trader GetTrader ()
    {
        return traders[Random.Range ( 0, traders.Count )];
    }
}

public class Trader
{
    public Trader (string name, List<TraderItem> items)
    {
        this.name = name;
        this.items = items;
    }

    public string name { get; protected set; }
    public List<TraderItem> items { get; protected set; } = new List<TraderItem> ();

    public List<TradeProposalGroup> GetDeal ()
    {
        StaticExtensions.ShuffleList ( items );

        List<TradeProposalEntry> tradeEntries = new List<TradeProposalEntry> ();
        List<TradeProposalGroup> groups = new List<TradeProposalGroup> ();

        foreach (TraderItem item in items)
        {
            if (tradeEntries.Count >= 4) break;

            if (item.tradeType == TradingController.TradeType.Buy)
            {
                tradeEntries.Add ( new TradeProposalEntry ( item.tradeType, item.itemID, Mathf.Floor ( Random.Range ( item.minQuantity, item.maxQuantity ) ), item.inflation ) );
            }
            else
            {
                float value = Mathf.Floor ( Random.Range ( item.minQuantity, item.maxQuantity ) );

                if (WarehouseController.Instance.Inventory.GetAvailableQuantity ( item.itemID ) < value) continue;

                tradeEntries.Add ( new TradeProposalEntry ( item.tradeType, item.itemID, value, item.inflation ) );
                WarehouseController.Instance.Inventory.ReserveItemQuantity ( item.itemID, value );
            }
        }

        StaticExtensions.ShuffleList ( tradeEntries );

        while (tradeEntries.Count > 0)
        {
            int maxValue = Mathf.Min ( (int)3, tradeEntries.Count );
            int range = Random.Range ( (int)1, maxValue );
            List<TradeProposalEntry> groupEntries = new List<TradeProposalEntry> ();

            for (int i = 0; i < range; i++)
            {
                groupEntries.Add ( tradeEntries[i] );
                tradeEntries.RemoveAt ( i );
            }

            groups.Add ( new TradeProposalGroup ( groupEntries ) );
        }

        //for (int i = 0; i < tradeEntries.Count; i++)
        //{
        //    groups.Add ( new TradeProposalGroup ( new List<TradeProposalEntry> () { tradeEntries[i] } ) );
        //}

        return groups;
    }
}

public class TraderItem
{
    public TraderItem (TradingController.TradeType tradeType, int itemID, float minQuantity, float maxQuantity, float inflation)
    {
        this.tradeType = tradeType;
        this.itemID = itemID;
        this.minQuantity = minQuantity;
        this.maxQuantity = maxQuantity;
        this.inflation = inflation;
    }

    public TradingController.TradeType tradeType { get; protected set; }
    public int itemID { get; protected set; }
    public float minQuantity { get; protected set; }
    public float maxQuantity { get; protected set; }
    public float inflation { get; protected set; }
}

public class TradeProposalGroup
{
    public TradeProposalGroup (List<TradeProposalEntry> entries)
    {
        this.entries = entries;

        foreach (TradeProposalEntry item in entries)
        {
            if(item.tradeType == TradingController.TradeType.Buy)
            {
                overallCost += Mathf.FloorToInt ( item.itemQuantity * (ResourceManager.Instance.GetResourceByID ( item.itemID ).baseBuyPrice * item.inflation) );
            }
            else
            {
                overallCost -= Mathf.FloorToInt ( item.itemQuantity * (ResourceManager.Instance.GetResourceByID ( item.itemID ).baseSellPrice * item.inflation) );
            }
        }
    }

    public List<TradeProposalEntry> entries { get; protected set; } = new List<TradeProposalEntry> ();
    public int overallCost { get; protected set; } = 0;

    //public System.Action OnAccept;
    //public System.Action OnDecline;

    //public void Accept ()
    //{
    //    if (MoneyController.Instance.CanAfford ( overallCost ))
    //    {

    //    }
    //}

    //public void Decline ()
    //{

    //}
}

public class TradeProposalEntry
{
    public TradeProposalEntry (TradingController.TradeType tradeType, int itemID, float itemQuantity, float inflation)
    {
        this.tradeType = tradeType;
        this.itemID = itemID;
        this.itemQuantity = itemQuantity;
        this.inflation = inflation;
    }

    public TradingController.TradeType tradeType { get; protected set; }
    public int itemID { get; protected set; }
    public float itemQuantity { get; protected set; }
    public float inflation { get; protected set; }
}
