using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradingController : MonoSingleton<TradingController>
{
    public enum TradeType { Buy, Sell }

    public bool marketPlaceExists { get; protected set; } = false;
    public bool tradingIsActive { get; protected set; } = true;

    public float baseTradeChance { get; protected set; } = 0.05f;
    public int currentRolls { get; protected set; } = 0;

    public System.Action<List<TradeProposalGroup>, Trader> onTradeBegin;

    public void Start ()
    {
        //GameTime.onDayChanged += OnDayChanged;

        EntityManager.Instance.onEntityCreated += OnEntityCreated;
        EntityManager.Instance.onEntityDestroyed += OnEntityDestroyed;
    }    

    private void Update ()
    {
        if(Input.GetKeyDown(KeyCode.Y) && !Hotkey.InputFieldIsActive ())
        {
            OnDayChanged ( 0, 0 );
        }
    }

    public void SetMarketPlaceTrading(bool state)
    {
        tradingIsActive = state;                    
    }

    private void OnDayChanged(int p, int c)
    {
        if (!marketPlaceExists) return;
        if (!tradingIsActive) return;

        bool rollSuccess = false;

        for (int i = 0; i < currentRolls; i++)
        {
            float randomValue = Random.value;

            if (randomValue > baseTradeChance)
            {
                Debug.Log ( "Rolling trade chance " + i + " out of " + currentRolls + ". Rolled " + randomValue.ToString ( "0.00" ) + " out of " + baseTradeChance.ToString ( "0.00" ) + " - Failed" );
                rollSuccess = false;
                continue;
            }

            Debug.Log ( "Rolling trade chance " + i + " out of " + currentRolls + ". Rolled " + randomValue.ToString ( "0.00" ) + " out of " + baseTradeChance.ToString ( "0.00" ) + " - Success" );
            rollSuccess = true;
            break;
        }

        if (!rollSuccess)
        {
            return;
        }

        Trader trader = Traders.Instance.GetTrader ();
        List<TradeProposalGroup> tradeGroups = trader.GetDeal ();

        if (tradeGroups.Count <= 0)
        {
            HUD_Notification_Panel.Instance.AddNotification ( "A merchant visited but didn't want to trade", HUD_Notification_Panel.NotificationSprite.Warning, null );
            return;
        }
        else
        {
            Hud_Trading_Panel.Instance.Show ();
            Hud_Trading_Panel.Instance.SetDeal ( tradeGroups, trader );
            GameTime.SetToSlowestSpeed ();
            onTradeBegin?.Invoke ( tradeGroups, trader );
        }
    }

    private void OnEntityCreated (GameObject go)
    {
        Prop_MarketPlace marketPlace = go.GetComponent<Prop_MarketPlace> ();

        if (marketPlace != null)
        {
            if (marketPlace.buildable.IsComplete)
            {
                marketPlaceExists = true;
                currentRolls++;
            }
            else
            {
                marketPlace.buildable.onComplete += () => { marketPlaceExists = true; currentRolls++; };
            }
        }
    }

    private void OnEntityDestroyed (GameObject go)
    {
        if (go.GetComponent<Prop_MarketPlace> ())
        {
            if (EntityManager.Instance.GetEntitiesByType ( typeof ( Prop_MarketPlace ) ).Count <= 0)
                marketPlaceExists = false;

            currentRolls--;

            if (currentRolls <= 0) currentRolls = 0;
        }
    }
}