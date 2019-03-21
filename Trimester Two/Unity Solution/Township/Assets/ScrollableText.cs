using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(EventTrigger))]
public class ScrollableText : MonoBehaviour {

    private RectTransform parent;
    private RectTransform text;
    [SerializeField] private float speed;
    [SerializeField] private float startDelay;
    [SerializeField] private float currentDelay = 0;
    [SerializeField] private int stage = 0;

    private bool isActive = false;

	private void Start () {
        parent = GetComponent<RectTransform> ();
        text = transform.GetChild(0).GetComponent<RectTransform> ();

        EventTrigger trigger = GetComponent<EventTrigger> ();

        EventTrigger.Entry onEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter }; ;
        onEnter.callback.AddListener ( (eventData) => { SetEnabled ( true ); } );
        trigger.triggers.Add ( onEnter );

        EventTrigger.Entry onExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit }; ;
        onExit.callback.AddListener ( (eventData) => { SetEnabled ( false ); } );
        trigger.triggers.Add ( onExit );
    }
	
	private void Update () {
        if (!isActive) return;
        if (text.sizeDelta.x <= parent.sizeDelta.x) return;

        if (stage == 0)
        {
            currentDelay += Time.deltaTime;

            if(currentDelay >= startDelay)
            {
                currentDelay = 0.0f;
                stage++;
            }
        }
        else if(stage == 1)
        {
            float targetX = text.sizeDelta.x - parent.sizeDelta.x;
            targetX *= -1;
            text.anchoredPosition += new Vector2 ( targetX, 0.0f ) * ((Time.deltaTime * speed) / (targetX * -1.0f));

            if(text.anchoredPosition.x <= targetX)
            {
                currentDelay = 0.0f;
                stage++;
            }
        }
        else if(stage == 2)
        {

        }
	}

    private void SetEnabled(bool state)
    {
        if (state)
        {
            currentDelay = 0.0f;
            stage = 0;
            isActive = true;
        }
        else
        {
            isActive = false;
            text.anchoredPosition = new Vector2 ( 0.0f, text.anchoredPosition.y );
        }
    }
}
