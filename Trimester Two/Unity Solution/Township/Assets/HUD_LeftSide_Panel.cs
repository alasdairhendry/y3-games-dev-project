using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD_LeftSide_Panel : UIPanel {

    [SerializeField] private TextMeshProUGUI tmp_Coins;
    [SerializeField] private TextMeshProUGUI tmp_CoinsPerDay;

    [SerializeField] private TextMeshProUGUI tmp_Citizens;
    [SerializeField] private TextMeshProUGUI tmp_Temperature;

    [SerializeField] private TextMeshProUGUI tmp_DailyIncome;
    [SerializeField] private TextMeshProUGUI tmp_DailyBuilding;
    [SerializeField] private TextMeshProUGUI tmp_DailyTrading;
    [SerializeField] private TextMeshProUGUI tmp_DailyUpkeep;
    [SerializeField] private TextMeshProUGUI tmp_DailyTax;

    private float currentMoney = 0;

    private float lerpFactor = 0.0f;
    private float initialMoney = 0;
    private float targetMoney = 0;

    protected override void Start ()
    {
        base.Start ();

        UpdateCurrentCoins ( MoneyController.Instance.money );
        UpdateCoinsPerDay ( MoneyController.Instance.moneyMadeToday );
        UpdateTransactions ( MoneyController.Instance.dailyTransactions, MoneyController.Instance.moneyMadeToday );
        UpdateTemperature ();
        UpdateCitizens ();

        MoneyController.Instance.onMoneyChanged += UpdateCurrentCoins;
        MoneyController.Instance.onDailyMoneyChanged += UpdateCoinsPerDay;
        MoneyController.Instance.onDailyTransactionUpdated += UpdateTransactions;
        CitizenController.Instance.onCitizenBornOrDied += (cBase, alive) => { UpdateCitizens (); };
        ProfessionController.Instance.onRefreshProfessions += UpdateCitizens;
    }

    protected override void Update ()
    {
        base.Update ();

        UpdateTemperature ();

        if(currentMoney < targetMoney)
        {
            currentMoney += lerpFactor * (Time.deltaTime);

            if(currentMoney >= targetMoney)
            {
                currentMoney = targetMoney;
            }
        }
        else if( currentMoney > targetMoney)
        {
            currentMoney -= lerpFactor * (Time.deltaTime);

            if (currentMoney <= targetMoney)
            {
                currentMoney = targetMoney;
            }
        }

        tmp_Coins.text = MoneyController.BeautifyMoney ( Mathf.RoundToInt ( currentMoney ) );

        //if (currentMoney != targetMoney)
        //{
        //    currentMoney = Mathf.RoundToInt ( Mathf.Lerp ( currentMoney, targetMoney, Time.deltaTime * 9.0f ) );
        //    tmp_Coins.text = MoneyController.BeautifyMoney ( currentMoney );
        //}
    }

    private void UpdateCurrentCoins(int newMoney)
    {
        initialMoney = currentMoney;
        targetMoney = (float)newMoney;

        lerpFactor = (targetMoney - initialMoney);
        if (lerpFactor < 0) lerpFactor *= -1.0f;
        if (lerpFactor < 5) lerpFactor *= 2.0f;
    }

    private void UpdateCoinsPerDay (int moneyMadeToday)
    {
        if (moneyMadeToday < 0)
        {
            tmp_CoinsPerDay.GetComponentInParent<Image> ().sprite = Resources.Load<Sprite> ( "UI/decrease" );
            tmp_CoinsPerDay.color = Color.red;
        }
        else
        {
            tmp_CoinsPerDay.GetComponentInParent<Image> ().sprite = Resources.Load<Sprite> ( "UI/increase" );
            tmp_CoinsPerDay.color = Color.green;
        }

        tmp_CoinsPerDay.text = MoneyController.BeautifyMoney ( moneyMadeToday );
    }

    private void UpdateTransactions(List<MoneyController.Transaction> transactions, int dailyMoney)
    {
        int building = transactions.FindAll ( x => x.category == MoneyController.Transaction.Category.Build && x.isIncome ).Sum ( x => x.transactionAmount );
        building -= transactions.FindAll ( x => x.category == MoneyController.Transaction.Category.Build && !x.isIncome ).Sum ( x => x.transactionAmount );
        SetText ( ref tmp_DailyBuilding, building );

        int trading = transactions.FindAll ( x => x.category == MoneyController.Transaction.Category.Trading && x.isIncome ).Sum ( x => x.transactionAmount );
        trading -= transactions.FindAll ( x => x.category == MoneyController.Transaction.Category.Trading && !x.isIncome ).Sum ( x => x.transactionAmount );
        SetText ( ref tmp_DailyTrading, trading );

        int upkeep = transactions.FindAll ( x => x.category == MoneyController.Transaction.Category.Upkeep && x.isIncome ).Sum ( x => x.transactionAmount );
        upkeep -= transactions.FindAll ( x => x.category == MoneyController.Transaction.Category.Upkeep && !x.isIncome ).Sum ( x => x.transactionAmount );
        SetText ( ref tmp_DailyUpkeep, upkeep );

        int taxing = transactions.FindAll ( x => x.category == MoneyController.Transaction.Category.Tax && x.isIncome ).Sum ( x => x.transactionAmount );
        taxing -= transactions.FindAll ( x => x.category == MoneyController.Transaction.Category.Tax && !x.isIncome ).Sum ( x => x.transactionAmount );
        SetText ( ref tmp_DailyTax, taxing );

        SetText ( ref tmp_DailyIncome, building + trading + upkeep + taxing );
    }

    private void SetText(ref TextMeshProUGUI text, int amount)
    {
        if (amount >= 0)
        {
            text.color = ColourGroupController.Instance.GetColour ( "UI_TextGreen" );
            text.text = "+ " + amount;
        }
        else
        {
            text.color = ColourGroupController.Instance.GetColour ( "UI_TextRed" );
            text.text = "" + amount;
        }

        
    }

    private void UpdateTemperature ()
    {
        tmp_Temperature.text = TemperatureController.Temperature.ToString ( "00" ) + " C";
    }

    private void UpdateCitizens ()
    {
        int babies = ProfessionController.Instance.professionsByCitizen[ProfessionType.None].Count;
        int students = ProfessionController.Instance.professionsByCitizen[ProfessionType.Student].Count;
        int workers = CitizenController.Instance.activeCitizens.Count - (babies + students);
        tmp_Citizens.text = string.Format ( "{0} / {1} / {2}", babies, students, workers );
    }
}
