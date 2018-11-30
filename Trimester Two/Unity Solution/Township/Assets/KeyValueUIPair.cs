using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyValueUIPair : MonoBehaviour {

    public enum Type { Text, Input, Colour }
    private Type type;

    private System.Func<string> GetValueFoo;
    private string key;
    private string value;

    public GameObject SetData(System.Func<string> GetValueFoo, string key )
    {
        this.key = key;
        this.GetValueFoo = GetValueFoo;

        GetValue ();
        SetData ();

        GameTime.RegisterGameTick ( () => { GetValue (); } );
        GameTime.RegisterGameTick ( () => { SetData (); } );
        
        return this.gameObject;
    }

    private void GetValue ()
    {
        if (this == null) return;
        value = GetValueFoo ();
    }

    private void SetData ()
    {
        if (this == null) return;
        GetComponentsInChildren<Text> ()[0].text = key;
        GetComponentsInChildren<Text> ()[1].text = value;
    }

    private void OnDestroy ()
    {
        GameTime.UnRegisterGameTick ( () => { GetValue (); } );
        GameTime.UnRegisterGameTick ( () => { SetData (); } );
    }

    //public GameObject SetData (string key, string value, Transform parent)
    //{        
    //    switch (type)
    //    {
    //        case Type.Text:
    //            GetComponentsInChildren<Text> ()[0].text = key;
    //            GetComponentsInChildren<Text> ()[1].text = value;
    //            break;
    //        case Type.Input:
    //            break;
    //        case Type.Colour:
    //            break;
    //    }

    //    this.transform.SetParent ( parent );
    //    return this.gameObject;
    //}

    //public GameObject SetData (System.Action<string> action, string key, Transform parent)
    //{
    //    tickUpdateData = action;

    //    this.key = key;

    //    tickUpdateData (key);

    //    this.transform.SetParent ( parent );
    //    return this.gameObject;
    //}

}
