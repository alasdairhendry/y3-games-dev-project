using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Tooltip_Panel : UIPanel {

    public static HUD_Tooltip_Panel Instance;

    [SerializeField] private GameObject tooltipPrefab;
    private List<Tooltip> activeTooltips = new List<Tooltip> ();
    private float widestTooltip = 0;

    private void Awake ()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy ( this.gameObject );
    }

    protected override void Update ()
    {
        base.Update ();

        if (!Hotkey.MouseMoved && !CameraMovement.CameraMoved) return;

        CalculateWidth ();
    }

    private void CalculateWidth ()
    {
        widestTooltip = 0.0f;

        for (int i = 0; i < activeTooltips.Count; i++)
        {
            float w = activeTooltips[i].gameObject.GetComponentInChildren<RectTransform> ().sizeDelta.x;
            if (w > widestTooltip)
            {
                widestTooltip = w;
            }
        }
    }

    protected override void SetAnchoredPosition ()
    {
        base.targetAnchorOffset = new Vector3 ( 32.0f, -32.0f, 0.0f );
        base.targetAnchoredPosition = Input.mousePosition;
    }

    protected override void MovePanel ()
    {
        if (mouseIsOver && Hotkey.GetKey ( Hotkey.Function.HaltUI )) return;
        Vector3 targetPosition = targetAnchoredPosition + targetAnchorOffset;

        if (clampToScreen)
        {
            targetPosition.x = Mathf.Clamp ( targetPosition.x, 32, 1920.0f - widestTooltip - 32 );
            targetPosition.y = Mathf.Clamp ( targetPosition.y, 32 + rectTransform.sizeDelta.y, 1080.0f - 64 );
        }

        rectTransform.anchoredPosition = Vector3.Slerp ( rectTransform.anchoredPosition, targetPosition, Time.deltaTime * 5.0f );
    }

    public GameObject AddTooltip (string message, Tooltip.Preset preset)
    {
        if (activeTooltips.Exists ( x => x.message == message )) { return activeTooltips.Find ( x => x.message == message ).gameObject; }

        Tooltip tooltip = new Tooltip ( message, preset );

        GameObject go = Instantiate ( tooltipPrefab );
        go.name = "Tooltip_ " + activeTooltips.Count;
        go.transform.SetParent ( transform.GetChild ( 0 ) );

        go.GetComponent<Image> ().color = tooltip.backgroundColour;
        go.GetComponentInChildren<Text> ().color = tooltip.foregroundColour;
        go.GetComponentInChildren<Shadow> ().effectColor = tooltip.foregroundShadowColour;
        go.GetComponentInChildren<Text> ().text = message;

        tooltip.gameObject = go;
        activeTooltips.Add ( tooltip );
        return go;
    }

    public void UpdateTooltip(GameObject go, string message)
    {
        Tooltip t = activeTooltips.Find ( x => x.gameObject == go );
        if (t == null) return;

        t.message = message;
        t.gameObject.GetComponentInChildren<Text> ().text = message;
    }

    public void RemoveTooltip (string message)
    {
        if (activeTooltips.Exists ( x => x.message == message ))
        {
            Tooltip t = activeTooltips.Find ( x => x.message == message );
            activeTooltips.Remove ( t );
            Destroy ( t.gameObject );
        }
    }

    public void RemoveTooltip(GameObject go)
    {
        if(activeTooltips.Exists(x => x.gameObject == go ))
        {
            activeTooltips.Remove ( activeTooltips.Find ( x => x.gameObject == go ) );
            Destroy ( go );
        }
    }
	
    public class Tooltip
    {
        public enum Preset { Information, Warning, Error }
        public string message;
        public Color foregroundColour;
        public Color foregroundShadowColour;
        public Color backgroundColour;
        public Sprite sprite;
        public GameObject gameObject;

        public Tooltip(string message, Preset preset)
        {
            this.message = message;
            SetPreset ( preset );
        }

        private void SetPreset(Preset preset)
        {
            switch (preset)
            {
                case Preset.Information:
                    foregroundColour = Color.black;
                    foregroundShadowColour = Color.white;

                    backgroundColour = Color.white;
                    break;

                case Preset.Warning:
                    foregroundColour = Color.black;
                    foregroundShadowColour = Color.white;

                    backgroundColour = Color.yellow;
                    break;

                case Preset.Error:
                    foregroundColour = Color.black;
                    foregroundShadowColour = Color.white;

                    backgroundColour = Color.red;
                    break;

                default:
                    foregroundColour = Color.black;
                    foregroundShadowColour = Color.white;

                    backgroundColour = Color.white;
                    break;
            }
        }
    }


}
