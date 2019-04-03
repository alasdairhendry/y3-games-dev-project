using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Tutorial
{
    protected Tutorial () { }

    public string identifier { get; protected set; }
    public string header { get; protected set; }
    public int stages { get; protected set; } = 0;
    public int displayStages { get; protected set; } = 0;

    public int currentStage { get; protected set; } = 0;
    public bool hasBegun { get; protected set; } = false;
    public bool isComplete { get; protected set; } = false;

    protected List<TextAsset> textAssets = new List<TextAsset> ();

    protected void LoadTextAssets ()
    {
        textAssets = Resources.LoadAll<TextAsset> ( "Tutorial/TextAssets/" + identifier ).ToList ();
    }

    public void SetStage (int newStage)
    {
        currentStage = newStage;
        OnSetStage ( currentStage );
    }

    public void IncrementStage ()
    {
        currentStage++;
        OnSetStage ( currentStage );
    }

    public abstract void Begin ();

    protected abstract void OnSetStage (int stage);

    public virtual void Complete ()
    {
        isComplete = true;
        currentStage = stages;
    }
}
