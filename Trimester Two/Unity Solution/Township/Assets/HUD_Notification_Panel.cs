using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Notification_Panel : MonoSingleton<HUD_Notification_Panel> {

    private int max = 3;
    private int count = 0;

    [SerializeField] private Transform content;
    [SerializeField] private GameObject prefab;

    private Queue<Notification> queue = new Queue<Notification> ();

    public void AddNotification (string text, Sprite sprite, Inspectable inspectable)
    {
        if (sprite == null) sprite = NotificationSprite.Information;
        queue.Enqueue ( new Notification ( text, sprite, inspectable ) );
        CheckNotifications ();
    }

    private void RemoveNotification ()
    {
        count--;
        CheckNotifications ();
    }

    private void CheckNotifications ()
    {
        if (queue.Count <= 0) return;

        if(count < max)
        {
            CreateNotification ( queue.Dequeue () );
        }
    }

    private void CreateNotification(Notification n)
    {
        GameObject go = Instantiate ( prefab );
        go.transform.SetParent ( content );
        go.transform.SetAsFirstSibling ();

        TextMeshProUGUI t = GetComponentInChildren<TextMeshProUGUI> ();
        t.text = n.text;

        if (t.isTextTruncated)
            t.gameObject.AddComponent<Tooltip> ().SetTooltip ( n.text, HUD_Tooltip_Panel.Tooltip.Preset.Information );

        go.transform.GetChild ( 0 ).Find ( "Sprite" ).GetComponent<Image> ().sprite = n.sprite;

        go.GetComponent<SelfDestruct> ().RegisterOnDestroy ( RemoveNotification );

        Button[] buttons = go.GetComponentsInChildren<Button> ();

        //Close 
        buttons[0].onClick.AddListener ( () => { go.GetComponent<SelfDestruct> ().DestroyNow (); } );

        // Focus
        buttons[1].onClick.AddListener ( () => { n.inspectable?.InspectAndLockCamera (); } );
        count++;

    }

    private class Notification
    {
        public string text;
        public Sprite sprite;
        public Inspectable inspectable;

        public Notification (string text, Sprite sprite, Inspectable inspectable)
        {
            this.text = text;
            this.sprite = sprite;
            this.inspectable = inspectable;
        }
    }

    public abstract class NotificationSprite
    {
        public static Sprite Information { get { return Resources.Load<Sprite> ( "UI/notification" ); } }
        public static Sprite Warning { get { return Resources.Load<Sprite> ( "UI/Warning" ); } }
        public static Sprite Error { get { return Resources.Load<Sprite> ( "UI/error" ); } }
    }
}
