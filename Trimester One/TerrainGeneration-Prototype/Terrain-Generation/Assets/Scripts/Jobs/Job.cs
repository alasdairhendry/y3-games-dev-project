using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Job {

    //public string Name { get; protected set; }
    //public bool Open { get; protected set; }
    //public bool Complete { get; protected set; }
    //public float TimeRequired { get; protected set; }

    public string Name;
    public bool Complete;
    public float TimeRequired;
    public bool Open { get; protected set; }
    public enum WorkerType { Builder, Gatherer, Farmer }

    protected Character character;
    
    public System.Action onComplete;

    public Job () { }

    public Job (string name, bool open, float timeRequired, System.Action onComplete)
    {
        this.Name = name;
        this.Open = open;
        this.TimeRequired = timeRequired;
        this.onComplete = onComplete;
    }

    public virtual void OnCharacterAccept (Character character) { this.character = character; Open = false; }

    public virtual void OnCharacterLeave() { this.character.OnJob_Complete (); this.character = null; this.Open = true; this.onComplete = null; }

    public virtual void DoJob (float deltaGameTime)
    {
        if (Complete) return;

        TimeRequired -= deltaGameTime;

        if (TimeRequired <= 0) OnComplete ();
    }

    protected virtual void OnComplete ()
    {
        this.character.OnJob_Complete ();
        this.character = null;

        Open = false;
        Complete = true;

        JobController.RemoveJob ( this );
        if (onComplete != null) onComplete ();
    }  
}
