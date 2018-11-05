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
    public bool Open;
    public bool Complete;
    public float TimeRequired;

    public Character character;

    //public List<Job> waitingOn = new List<Job> ();
    public System.Action onComplete;

    public Job () { }

    public Job (string name, bool open, float timeRequired)
    {
        this.Name = name;
        this.Open = open;
        this.TimeRequired = timeRequired;
    }

    public virtual void OnEnter (Character character) { this.character = character; }

    public virtual void OnExit () { }

    public virtual void DoJob (float deltaTime)
    {
        if (Complete) return;

        TimeRequired -= deltaTime;

        if (TimeRequired <= 0) OnComplete ();
    }

    public virtual void OnComplete ()
    {
        Complete = true;
        if (onComplete != null) onComplete ();
    }

    //public void AddDependancy (Job job)
    //{
    //    waitingOn.Add ( job );
    //    job.onComplete += () => { RemoveDependancy ( job ); };
    //}

    //public void RemoveDependancy(Job job)
    //{
    //    waitingOn.Remove ( job );
    //    if (waitingOn.Count <= 0) Open = true;
    //}
}
