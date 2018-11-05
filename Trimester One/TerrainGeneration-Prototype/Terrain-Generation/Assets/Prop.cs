using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour {

    public PropData data { get; protected set; }
    protected Buildable buildable;

	public virtual void Place (PropData data)
    {
        this.data = data;
        buildable = GetComponent<Buildable> ().Begin ();
    }
}
