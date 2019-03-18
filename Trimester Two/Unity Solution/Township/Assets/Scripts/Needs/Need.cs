using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Need {

    public enum Type { Energy, Happiness, Warmth, Health }

    public Type type { get; protected set; }
    public float baseValue { get; protected set; }  // Unmodified
    public float currentValue { get; protected set; }   // With modifiers applied
    public List<float> modifiers { get; protected set; }
    public System.Action<float> OnChange;

    public Need (Type type, float defaultValue)
    {
        this.type = type;
        this.baseValue = defaultValue;
        this.currentValue = defaultValue;
        this.modifiers = new List<float> ();
    }

    public float IncreaseValue(float amount = float.MaxValue)
    {
        if (amount == float.MaxValue) amount = GameTime.DeltaGameTime / GameTime.SecondsPerDay * 0.30f;

        baseValue += amount;
        baseValue = Mathf.Clamp01 ( baseValue );
        return CalculateModifiers ();
    }

    public float DecreaseValue(float amount = float.MaxValue)
    {
        if (amount == float.MaxValue) amount = GameTime.DeltaGameTime / GameTime.SecondsPerDay * 0.075f;
        

        baseValue -= amount;
        baseValue = Mathf.Clamp01 ( baseValue );
        return CalculateModifiers ();
    }

    public float SetBase(float value)
    {
        baseValue = value;
        baseValue = Mathf.Clamp01 ( baseValue );
        return CalculateModifiers ();
    }

    public float AddModifier(float amount)
    {
        modifiers.Add ( amount );
        return CalculateModifiers ();
    }

    public float RemoveModifier(float amount)
    {
        if (modifiers.Contains ( amount ))
        {
            modifiers.Remove ( amount );
            return CalculateModifiers ();
        }
        return CalculateModifiers ();
    }

    private float CalculateModifiers ()
    {
        float newValue = baseValue;

        for (int i = 0; i < modifiers.Count; i++)
        {
            newValue *= modifiers[i];
        }

        newValue = Mathf.Clamp01 ( newValue );

        currentValue = newValue;
        if (OnChange != null) OnChange ( currentValue );
        return currentValue;
    }
}
