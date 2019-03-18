using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopoutNotification : MonoBehaviour {

    public static PopoutNotification Instance;

    [SerializeField] private GameObject prefab;

    private void Awake ()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy ( this.gameObject );
    }

    public void AddPopout (string text, int fontSize, FontStyle style, Color textColour, Transform transform, float offset)
    {
        GameObject go = Instantiate ( prefab );
        go.transform.SetParent ( this.transform );

        go.GetComponent<UIWorldToTarget> ().SetTarget ( transform, offset );

        go.transform.Find ( "Content" ).Find ( "SpriteBackground" ).gameObject.SetActive ( false );

        Text t = go.GetComponentInChildren<Text> ();
        t.text = text;
        t.fontSize = fontSize;
        t.fontStyle = style;
        t.color = textColour;
    }

    public void AddPopout (string text, int fontSize, FontStyle style, Color textColour, Transform transform, float offset, Sprite sprite, float scale)
    {
        GameObject go = Instantiate ( prefab );
        go.transform.SetParent ( this.transform );

        go.GetComponent<UIWorldToTarget> ().SetTarget ( transform, offset );

        Text t = go.GetComponentInChildren<Text> ();
        t.text = text;
        t.fontSize = fontSize;
        t.fontStyle = style;
        t.color = textColour;

        go.transform.Find ( "Content" ).Find ( "SpriteBackground" ).GetChild ( 0 ).GetComponent<Image> ().sprite = sprite;
        go.transform.Find ( "Content" ).Find ( "SpriteBackground" ).GetComponent<LayoutElement> ().preferredWidth = scale;
        go.transform.Find ( "Content" ).Find ( "SpriteBackground" ).GetComponent<LayoutElement> ().preferredHeight = scale;
    }

}
