using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class Tooltip : MonoBehaviour {

    [SerializeField] private string toolTip;
    [SerializeField] private HUD_Tooltip_Panel.Tooltip.Preset preset = HUD_Tooltip_Panel.Tooltip.Preset.Information;
    [SerializeField] private EventTrigger eventTrigger;
    private bool added = false;
    private bool active = true;

    private void Start ()
    {
        if (eventTrigger == null) eventTrigger = GetComponent<EventTrigger> ();

        EventTrigger.Entry enter = new EventTrigger.Entry ();
        enter.eventID = EventTriggerType.PointerEnter;
        enter.callback.AddListener ( (e) => { if (active) { FindObjectOfType<HUD_Tooltip_Panel> ().AddTooltip ( toolTip, preset ); added = true; } } );

        EventTrigger.Entry exit = new EventTrigger.Entry ();
        exit.eventID = EventTriggerType.PointerExit;
        exit.callback.AddListener ( (e) => { FindObjectOfType<HUD_Tooltip_Panel> ().RemoveTooltip ( toolTip ); added = false; } );

        eventTrigger.triggers.Add ( enter );
        eventTrigger.triggers.Add ( exit );
    }

    public void SetState(bool state)
    {
        active = state;

        if (!active)
        {
            if (added)
                FindObjectOfType<HUD_Tooltip_Panel> ().RemoveTooltip ( toolTip );
        }
    }

    public void SetTooltip(string tooltip, HUD_Tooltip_Panel.Tooltip.Preset preset)
    {
        if (added)
        {
            FindObjectOfType<HUD_Tooltip_Panel> ().RemoveTooltip ( this.toolTip );
        }

        this.toolTip = tooltip;
        this.preset = preset;

        if (added)
        {
            FindObjectOfType<HUD_Tooltip_Panel> ().AddTooltip ( this.toolTip, this.preset );
        }
    }

    private void OnDestroy ()
    {
        if (added)
            FindObjectOfType<HUD_Tooltip_Panel> ().RemoveTooltip ( toolTip );
    }
}
