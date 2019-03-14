using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenNeeds : MonoBehaviour {

    public CitizenBase cBase { get; protected set; }
    public List<Need> Needs { get; protected set; }
    public Dictionary<Need.Type, Need> NeedsDictionary { get; protected set; }

    private void Awake ()
    {
        cBase = GetComponent<CitizenBase> ();
        CreateNeeds ();
    }

    private void Start ()
    {
        
    }

    private void CreateNeeds ()
    {
        Needs = new List<Need> ();
        NeedsDictionary = new Dictionary<Need.Type, Need> ();

        CreateNeed ( Need.Type.Health, 1.0f );
        CreateNeed ( Need.Type.Happiness, 1.0f ).OnChange += CalculateHappiness;
        CreateNeed ( Need.Type.Energy, 1.0f ).OnChange += CalculateHappiness;
        CreateNeed ( Need.Type.Warmth, 1.0f ).OnChange += CalculateHappiness;
    }

    private Need CreateNeed (Need.Type type, float defaultValue)
    {
        Need need = new Need ( type, defaultValue );
        Needs.Add ( need );
        NeedsDictionary.Add ( type, need );
        return need;
    }

    private void CalculateHappiness(float value)
    {
        float health = (NeedsDictionary[Need.Type.Energy].currentValue + NeedsDictionary[Need.Type.Warmth].currentValue + NeedsDictionary[Need.Type.Happiness].currentValue);
        health /= 3.0f;
        NeedsDictionary[Need.Type.Health].SetBase ( health );
    }
}
