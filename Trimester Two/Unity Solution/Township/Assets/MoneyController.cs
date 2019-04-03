using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyController : MonoSingleton<MoneyController> {

    /// <summary>
    /// The current money the player has
    /// </summary>
    public int money { get; protected set; } = 0;

    /// <summary>
    /// First int is what was added, second is what we currently have
    /// </summary>
    public System.Action<int, int> onMoneyAdded;

    /// <summary>
    /// First int is what was removed, second is what we currently have
    /// </summary>
    public System.Action<int, int> onMoneyRemoved;

    /// <summary>
    /// Param1 = Current Money
    /// </summary>
    public System.Action<int> onMoneyChanged;

    /// <summary>
    /// Daily transaction for today /// money made today
    /// </summary>
    public System.Action<List<Transaction>, int> onDailyTransactionUpdated;

    /// <summary>
    /// Called whenever our daily income changes
    /// </summary>
    public System.Action<int> onDailyMoneyChanged;

    /// <summary>
    /// All transactions made
    /// </summary>
    public List<Transaction> overallTransactions { get; protected set; } = new List<Transaction> ();

    public List<Transaction> dailyTransactions { get; protected set; } = new List<Transaction> ();
    public int moneyMadeToday { get; protected set; } = 0;

    private void Start ()
    {
        GameTime.onDayChanged += (p, c) => { OnDayChanged (); };
    }

    private void Update ()
    {
        if (Input.GetKeyDown ( KeyCode.Equals ) && !Hotkey.InputFieldIsActive ()) Add ( 1000 );
    }

    public void Start_New ()
    {
        if (GameData.Instance.gameDataType == GameData.GameDataType.New)
        {
            money = 0;
            Add ( GameData.Instance.startingConditions.startingMoney );
        }
    }

    public void Start_Loaded (PersistentData.SaveData data)
    {
        Add ( data.money );
    }

    public static string BeautifyMoney (int money)
    {
        return string.Format ( "{0:n0}", money );
    }

    public void Add(int amount)
    {
        if (amount < 0)
        {
            Debug.LogError ( "Trying to add negative money" );
            amount *= -1;
        }

        money += amount;

        onMoneyChanged?.Invoke ( money );
        onMoneyAdded?.Invoke ( amount, money );
    }

    public void AddWithTransaction(int amount, string reason, Transaction.Category category)
    {
        if (amount < 0)
        {
            Debug.LogError ( "Trying to add negative money" );
            amount *= -1;
        }

        money += amount;        

        Transaction tran = new Transaction ( category, amount, money, true, reason, GameTime.currentDayOfTheGame, GameTime.currentDayOfTheYear, GameTime.currentDayOfTheMonth, GameTime.currentMonth, GameTime.currentYear );
        overallTransactions.Add ( tran );
        dailyTransactions.Add ( tran );
        UpdateMoneyMadeToday ( tran );
        onDailyTransactionUpdated?.Invoke ( dailyTransactions, moneyMadeToday );

        onMoneyChanged?.Invoke ( money );
        onMoneyAdded?.Invoke ( amount, money );
    }

    public void Remove (int amount)
    {
        if (amount < 0)
        {
            Debug.LogError ( "Trying to remove negative money" );
            amount *= -1;
        }

        money -= amount;

        onMoneyChanged?.Invoke ( money );
        onMoneyRemoved?.Invoke ( amount, money );
    }

    public void RemoveWithTransaction(int amount, string reason, Transaction.Category category)
    {
        if(amount < 0)
        {
            Debug.LogError ( "Trying to remove negative money" );
            amount *= -1;
        }

        money -= amount;

        Transaction tran = new Transaction ( category, amount, money, false, reason, GameTime.currentDayOfTheGame, GameTime.currentDayOfTheYear, GameTime.currentDayOfTheMonth, GameTime.currentMonth, GameTime.currentYear );
        overallTransactions.Add ( tran );
        dailyTransactions.Add ( tran );
        UpdateMoneyMadeToday ( tran );
        onDailyTransactionUpdated?.Invoke ( dailyTransactions, moneyMadeToday );

        onMoneyChanged?.Invoke ( money );
        onMoneyRemoved?.Invoke ( amount, money );
    }

    public bool CanAfford(int amount)
    {
        if (amount <= money) return true;

        return false;
    }

    private void UpdateMoneyMadeToday (Transaction t)
    {
        if (t.category == Transaction.Category.Tax || t.category == Transaction.Category.Upkeep)
        {
            if (t.isIncome)
                moneyMadeToday += t.transactionAmount;
            else moneyMadeToday -= t.transactionAmount;

            onDailyMoneyChanged?.Invoke ( moneyMadeToday );
        }
    }

    private void OnDayChanged ()
    {
        onDailyTransactionUpdated?.Invoke ( dailyTransactions, moneyMadeToday );
        moneyMadeToday = 0;
        dailyTransactions.Clear ();
        onDailyMoneyChanged?.Invoke ( moneyMadeToday );
    }
    
    [System.Serializable]
    public class Transaction
    {
        public enum Category { Build, Tax, Upkeep, Trading }

        public Category category;
        public int transactionAmount;
        public int moneyAfterTransaction;
        public bool isIncome;

        public string reason;

        public int overallGameDay;
        public int overallYearDay;
        public int overallMonthDay;
        public int month;
        public int year;

        public Transaction (Category category, int transactionAmount, int moneyAfterTransaction, bool isIncome, string reason, int overallGameDay, int overallYearDay, int overallMonthDay, int month, int year)
        {
            this.category = category;
            this.transactionAmount = transactionAmount;
            this.moneyAfterTransaction = moneyAfterTransaction;
            this.isIncome = isIncome;
            this.reason = reason;
            this.overallGameDay = overallGameDay;
            this.overallYearDay = overallYearDay;
            this.overallMonthDay = overallMonthDay;
            this.month = month;
            this.year = year;
        }
    }
}
