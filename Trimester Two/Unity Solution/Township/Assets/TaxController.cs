using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaxController : MonoSingleton<TaxController> {

    public float BaseGlobalPercentage { get; protected set; } = 0.125f;
    public float GlobalTaxModifier { get; protected set; } = 1.0f;

    public float MinGlobalModifier { get; protected set; } = 0.8f;
    public float MaxGlobalModifier { get; protected set; } = 1.2f;

    public float MinLocalModifier { get; protected set; } = 0.8f;
    public float MaxLocalModifier { get; protected set; } = 1.1f;

    //public float DefaultTax { get; protected set; } = 250.0f;
    //public float DailyTaxRequirement { get { return DefaultTax * Percentage; } 
    public Dictionary<ProfessionType, TaxBand> taxBandsByProfession { get; protected set; } = new Dictionary<ProfessionType, TaxBand> ();

    public List<TaxBand> taxBands = new List<TaxBand> ();

    public override void Init ()
    {
        base.Init ();

        taxBands.Add ( new TaxBand ( "Children", 0.0f, MinLocalModifier, MaxLocalModifier, 1.0f, ProfessionType.None ) );
        taxBands.Add ( new TaxBand ( "Students", 25.0f, MinLocalModifier, MaxLocalModifier, 1.0f, ProfessionType.Student ) );
        taxBands.Add ( new TaxBand ( "Workers", 250.0f, MinLocalModifier, MaxLocalModifier, 1.0f, ProfessionType.Worker ) );
        taxBands.Add ( new TaxBand ( "Gatherers", 200.0f, MinLocalModifier, MaxLocalModifier, 1.0f, ProfessionType.Fisherman, ProfessionType.Lumberjack, ProfessionType.Miner, ProfessionType.Quarryman, ProfessionType.Vintner ) );
        taxBands.Add ( new TaxBand ( "Producers", 150.0f, MinLocalModifier, MaxLocalModifier, 1.0f, ProfessionType.Blacksmith, ProfessionType.Charcoal_Burner, ProfessionType.Stonemason, ProfessionType.Winemaker ) );
    }

    public void Start_Loaded (PersistentData.SaveData data)
    {
        GlobalTaxModifier = data.GlobalTaxModifier;
        //taxBands.Clear ();
        for (int i = 0; i < data.taxBands.Count; i++)
        {
            taxBands[i].SetTaxModifier ( data.taxBands[i].Modifier );
        }
    }

    public TaxBand GetTaxBand (ProfessionType type)
    {
        if (taxBandsByProfession.ContainsKey ( type ))
        {
            return taxBandsByProfession[type];
        }
        else
        {
            for (int i = 0; i < taxBands.Count; i++)
            {
                if (taxBands[i].professions.Contains ( type ))
                {
                    taxBandsByProfession.Add ( type, taxBands[i] );
                    return taxBands[i];
                }
            }

            Debug.LogError ( "ERROR - TAX BAND DOESNT EXIST FOR PROFESSION - " + type.ToString () );
            return null;
        }
    }

    public TaxBand GetTaxBand (string bandName)
    {
        for (int i = 0; i < taxBands.Count; i++)
        {
            if (taxBands[i].BandName == bandName)
            {
                return taxBands[i];
            }
        }

        Debug.LogError ( "ERROR - TAX BAND DOESNT EXIST FOR PROFESSION - " + bandName );
        return null;
    }

    public float GetRequiredTax (ProfessionType type)
    {
        float bandTax = GetTaxBand ( type ).BaseSetTax;
        return bandTax * (BaseGlobalPercentage * GlobalTaxModifier);
    }

    public float GetRequiredTaxWithGlobalModifier (ProfessionType type)
    {
        float bandTax = GetTaxBand ( type ).BaseSetTax;
        return bandTax * (BaseGlobalPercentage);
    }

    public float GetIncomeDailyTax ()
    {
        float dailyTax = 0.0f;

        for (int i = 0; i < taxBands.Count; i++)
        {
            for (int x = 0; x < taxBands[i].professions.Count; x++)
            {
                int count = ProfessionController.Instance.professionsByCitizen[taxBands[i].professions[x]].Count;
                dailyTax += count * GetRequiredTax ( taxBands[i].professions[x] );
            }
        }

        return dailyTax;
    }

    public void SetGlobalModifier(float value)
    {
        GlobalTaxModifier = value;
        GlobalTaxModifier = Mathf.Clamp ( GlobalTaxModifier, MinGlobalModifier, MaxGlobalModifier );
    }

    public void ResetGlobalModifier ()
    {
        GlobalTaxModifier = 1.0f;
    }

    public void SetTaxBandModifier(TaxBand taxBand, float value)
    {
        taxBand.SetTaxModifier ( value );
    }

    public void ResetTaxBandModifier(TaxBand taxBand)
    {
        taxBand.ResetModifier ();
    }

    [System.Serializable]
    public class TaxBand
    {
        public TaxBand (string bandName, float defaultTax, float minModifier, float maxModifier, float modifier, params ProfessionType[] professions)
        {
            this.BandName = bandName;
            this.professions = professions.ToList();

            MinModifier = minModifier;
            MaxModifier = maxModifier;
            Modifier = modifier;
            
            DefaultTax = defaultTax;
            BaseSetTax = DefaultTax;
            //CurrentTax = DefaultTax * Modifier;

            //MinTax = DefaultTax * minModifier;
            //MaxTax = DefaultTax * maxModifier;
        }

        public string BandName { get; protected set; } = "";
        public List<ProfessionType> professions { get; protected set; } = new List<ProfessionType> ();
        //public float MinTax { get; protected set; } = 250.0f;
        //public float MaxTax { get; protected set; } = 250.0f;

        public float MinModifier { get; protected set; } = 250.0f;
        public float MaxModifier { get; protected set; } = 250.0f;

        public float Modifier { get; protected set; } = 1.0f;

        public float DefaultTax { get; protected set; } = 250.0f;

        public float BaseSetTax { get; protected set; } = 250.0f;
        //public float CurrentTax { get; protected set; } = 250.0f;

        public void SetTaxModifier(float modifier)
        {
            this.Modifier = modifier;
            this.Modifier = Mathf.Clamp ( this.Modifier, this.MinModifier, this.MaxModifier );
            BaseSetTax = DefaultTax * this.Modifier;
        }

        public void ResetModifier ()
        {
            this.Modifier = 1.0f;
            BaseSetTax = DefaultTax * this.Modifier;
        }
    }
}
