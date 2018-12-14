using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeBase : MonoBehaviour {

    [SerializeField] protected UIPanel modePanel;
    public bool isActive { get; protected set; }

    protected virtual void Awake ()
    {

    }

    protected virtual void Start ()
    {

    }

    protected virtual void Update ()
    {

    }
	
    public virtual void StartMode ()
    {
        modePanel.Show ();
    }

    public virtual void StopMode ()
    {
        modePanel.Hide ();
    }

    public virtual void Toggle()
    {
        if (isActive) StopMode ();
        else StartMode ();
    }
}
