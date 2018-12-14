using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class KeyValueUIPair : MonoBehaviour {

    public enum Type { Text, Dropdown }
    private Type type;

    private System.Func<string> GetValueFoo;
    private string key;
    private string value;

    public GameObject SetData(System.Func<string> GetValueFoo, string key )
    {
        this.key = key;
        this.GetValueFoo = GetValueFoo;

        GetValue (0);
        SetData (0);

        GameTime.RegisterGameTick ( (relativeTick) => { GetValue ( relativeTick ); } );
        GameTime.RegisterGameTick ( (relativeTick) => { SetData ( relativeTick ); } );
        
        return this.gameObject;
    }

    public void SetType(Type type)
    {
        this.type = type;
    }

    private void GetValue (int relativeTick)
    {
        if (this == null) return;
        value = GetValueFoo ();
    }

    private void SetData (int relativeTick)
    {
        if (this == null) return;
        GetComponentsInChildren<Text> ()[0].text = key;

        if (type == Type.Text)
            GetComponentsInChildren<Text> ()[1].text = value;
        else if (type == Type.Dropdown)
            GetComponentInChildren<Dropdown> ().value = GetComponentInChildren<Dropdown> ().options.FindIndex ( x => x.text == value );
    }

    private void OnDestroy ()
    {
        GameTime.UnRegisterGameTick ( (relativeTick) => { GetValue ( relativeTick ); } );
        GameTime.UnRegisterGameTick ( (relativeTick) => { SetData ( relativeTick ); } );
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
