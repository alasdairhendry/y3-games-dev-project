using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconDisplayer : MonoBehaviour {

    public enum Displayer { Citizen, Prop, RawMaterial }
    public enum IconType { Inventory, JobWaiting, Warning, Other, NoHouse }

    [SerializeField] private Displayer displayType = Displayer.Prop;

    private List<IconType> displayedIcons = new List<IconType> ();
    private List<GameObject> displayIconsGameObjects = new List<GameObject> ();

    [SerializeField] private Vector3 offset;
    private Vector2 scaleRange = new Vector2 ( 0.0035f, 0.075f );
    private Vector2 distanceRange = new Vector2 ( 20.0f, 500.0f );
    //private Vector2 minMaxScale = new Vector2 ();
    //private Vector2 minMaxDistance = new Vector2 ();

    private RectTransform canvasRect;
    private CanvasGroup canvasGroup;
    private Transform rootPanel;
    private bool canvasCreated = false;

    private bool active = true;

    private void Start ()
    {
        //SetScaling ();
    }

    private void Update ()
    {
        if (!canvasCreated) return;

        if (!active)
        {
            switch (displayType)
            {
                case Displayer.Citizen:
                    if (GamePreferences.Instance.preferences.showCitizenIcons) { SetState ( true ); }
                    break;
                case Displayer.Prop:
                    if (GamePreferences.Instance.preferences.showPropIcons) { SetState ( true ); }
                    break;
                case Displayer.RawMaterial:
                    break;
            }
            return;
        }

        switch (displayType)
        {
            case Displayer.Citizen:
                if (!GamePreferences.Instance.preferences.showCitizenIcons) { SetState ( false ); }
                    break;
            case Displayer.Prop:
                if (!GamePreferences.Instance.preferences.showPropIcons) { SetState ( false ); }
                break;
            case Displayer.RawMaterial:
                break;
        }

        if (displayIconsGameObjects.Count > 0)
        {
            SetScaling ();
        }


    }

    private void SetScaling ()
    {
        float distance = Vector3.Distance ( transform.position, Camera.main.transform.position );
        float scale = Mathf.Lerp ( scaleRange.x, scaleRange.y, Mathf.InverseLerp ( distanceRange.x, distanceRange.y, distance ) );
        canvasRect.transform.localScale = new Vector3 ( scale, scale, scale );
    }

    private void CreateCanvas ()
    {
        GameObject go = Instantiate ( Resources.Load<GameObject> ( "UI/IconDisplay_Canvas_Prefab" ) );
        canvasRect = go.GetComponent<RectTransform> ();
        canvasGroup = go.GetComponent<CanvasGroup> ();
        go.transform.SetParent ( this.transform );

        canvasRect.anchoredPosition3D = offset;
        canvasRect.localEulerAngles = Vector3.zero;

        rootPanel = go.transform.GetChild ( 0 );
        canvasCreated = true;

        SetScaling ();
    }

    public void AddIconGeneric (IconType type, Sprite sprite = null)
    {
        if(type == IconType.Other) { Debug.LogError ( "Use AddIconSpecific for type Other" ); return; }
        if (!canvasCreated) CreateCanvas ();
        if (displayedIcons.Contains ( type )) return;

        GameObject go = Instantiate ( Resources.Load<GameObject> ( "UI/IconDisplay_Icon_Prefab" ) );
        go.transform.SetParent ( rootPanel );

        RectTransform rect = go.GetComponent<RectTransform> ();
        rect.anchoredPosition3D = Vector3.zero;
        rect.localEulerAngles = Vector3.zero;
        rect.localScale = Vector3.one;

        go.GetComponentsInChildren<Image> ()[1].sprite = GetIcon ( type );
        displayedIcons.Add ( type );
        displayIconsGameObjects.Add ( go );
    }

    public TextIcon AddTextIcon(Sprite sprite, float padding)
    {
        if (!canvasCreated) CreateCanvas ();

        GameObject go = Instantiate ( Resources.Load<GameObject> ( "UI/IconDisplay_TextIcon_Prefab" ) );
        go.transform.SetParent ( rootPanel );

        RectTransform rect = go.GetComponent<RectTransform> ();
        rect.anchoredPosition3D = Vector3.zero;
        rect.localEulerAngles = Vector3.zero;
        rect.localScale = Vector3.one;

        displayedIcons.Add ( IconType.Other );
        displayIconsGameObjects.Add ( go );

        TextIcon icon = new TextIcon ( go, go.GetComponentsInChildren<Image> ()[1], go.GetComponentInChildren<Text> (), padding );
        icon.image.sprite = sprite;
        return icon;

    }

    public void RemoveIconByType(IconType type)
    {
        for (int i = 0; i < displayIconsGameObjects.Count; i++)
        {
            if (i < 0 || i >= displayIconsGameObjects.Count) continue;
            if (displayIconsGameObjects[i] == null) continue;
            if (displayIconsGameObjects[i].GetComponentsInChildren<Image> ()[1] == null) continue;

            if (displayIconsGameObjects[i].GetComponentsInChildren<Image> ()[1].sprite == GetIcon ( type ))
            {
                Destroy ( displayIconsGameObjects[i] );
                displayIconsGameObjects.RemoveAt ( i );
                displayedIcons.RemoveAt ( i );
            }
        }
    }

    public void RemoveIconByObject(GameObject go)
    {
        for (int i = 0; i < displayIconsGameObjects.Count; i++)
        {
            if (i < 0 || i >= displayIconsGameObjects.Count) continue;
            if (displayIconsGameObjects[i] == null) continue;
            if (displayIconsGameObjects[i].GetComponentsInChildren<Image> ()[1] == null) continue;

            if (displayIconsGameObjects[i] == go)
            {
                Destroy ( displayIconsGameObjects[i] );
                displayIconsGameObjects.RemoveAt ( i );
                displayedIcons.RemoveAt ( i );
            }
        }
    }

    private Sprite GetIcon(IconType type)
    {
        switch (type)
        {
            case IconType.Inventory:
                return Resources.Load<Sprite> ( "UI/shoppingBasket" );

            case IconType.JobWaiting:
                return Resources.Load<Sprite> ( "UI/wrench" );

            case IconType.Warning:
                return Resources.Load<Sprite> ( "UI/shoppingBasket" );

            case IconType.NoHouse:
                return Resources.Load<Sprite> ( "UI/house" );

            default:
                return Resources.Load<Sprite> ( "UI/shoppingBasket" );
        }
    }

    private void SetState(bool state)
    {
        active = state;
        canvasGroup.alpha = (state) ? 1 : 0;
    }

    public class TextIcon
    {
        public GameObject go;
        public Image image;
        public Text text;

        public TextIcon (GameObject go, Image image, Text text, float padding)
        {
            this.go = go;
            this.image = image;
            this.text = text;

            this.image.GetComponent<RectTransform> ().offsetMin = new Vector2 ( padding, padding );
            this.image.GetComponent<RectTransform> ().offsetMax = new Vector2 ( -padding, -padding );
        }
    }
}
