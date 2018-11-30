using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class UIPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    [Header ( "Panel Group" )]
    [SerializeField] protected int groupID;
    [SerializeField] protected bool isDefault;
    public KeyValuePair<int, bool> GroupDetails { get { return new KeyValuePair<int, bool> ( groupID, isDefault ); } }

    [Header("Options")]
    [SerializeField] private bool showOnAwake;
    [SerializeField] private bool blockRaycasts;
    [SerializeField] private bool isStatic = true;
    [SerializeField] private bool clampToScreen = true;

    //protected PanelGroup panelGroup;
    protected CanvasGroup cGroup;
    protected bool active = false;

    protected RectTransform parentRectTransform;
    protected RectTransform rectTransform;
    protected Vector3 targetAnchoredPosition = Vector3.zero;
    protected Vector3 targetAnchorOffset = Vector3.zero;

    protected bool mouseIsOver = false;

    protected virtual void Update ()
    {
        if (!active) return;
        if (!isStatic)
        {
            SetAnchoredPosition ();
            MovePanel ();
        }
    }

    protected virtual void SetAnchoredPosition () { Debug.LogError ( "This panel is not static, but nothing is setting its target position" ); }

    protected virtual void MovePanel ()
    {
        if (mouseIsOver && Input.GetKey(KeyCode.LeftShift)) return;
        Vector3 targetPosition = targetAnchoredPosition + targetAnchorOffset;

        if (clampToScreen)
        {
            targetPosition.x = Mathf.Clamp ( targetPosition.x, 32, 1920.0f - rectTransform.sizeDelta.x - 32 );
            targetPosition.y = Mathf.Clamp ( targetPosition.y, 32 + rectTransform.sizeDelta.y, 1080.0f - 64 );
        }

        rectTransform.anchoredPosition = Vector3.Slerp ( rectTransform.anchoredPosition, targetPosition, Time.deltaTime * 5.0f );
    }

    protected virtual void Start ()
    {
        cGroup = GetComponent<CanvasGroup> ();
        parentRectTransform = GetComponentInParent<RectTransform> ();
        rectTransform = GetComponent<RectTransform> ();
        if (showOnAwake) Show (); else Hide ();
    }

	public virtual void Show ()
    {        
        active = true;
        if (cGroup == null) cGroup = GetComponent<CanvasGroup> ();
        cGroup.alpha = 1;

        if (blockRaycasts)
            cGroup.blocksRaycasts = true;
        else cGroup.blocksRaycasts = false;


        //panelGroup.SetActivePanel ( this );
    }

    public virtual void Toggle ()
    {
        active = !active;

        if (active) Show (); else Hide ();
    }

    public virtual void Hide ()
    {
        //Debug.Log ( gameObject.name );
        active = false;

        if(cGroup == null) cGroup = GetComponent<CanvasGroup> ();

        cGroup.alpha = 0;
        cGroup.blocksRaycasts = false;

        //if (!isDefault)
        //    panelGroup.RemoveActivePanel ( this );
        //return;
    }

    void IPointerEnterHandler.OnPointerEnter (PointerEventData eventData)
    {
        mouseIsOver = true;
    }

    void IPointerExitHandler.OnPointerExit (PointerEventData eventData)
    {
        mouseIsOver = false;
    }
}
