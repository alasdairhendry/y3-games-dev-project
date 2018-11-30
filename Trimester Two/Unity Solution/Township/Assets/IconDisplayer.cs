using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconDisplayer : MonoBehaviour {

    public enum IconType { Inventory, JobWaiting, Warning }

    private List<IconType> displayedIcons = new List<IconType> ();
    private List<GameObject> displayIconsGameObjects = new List<GameObject> ();

    [SerializeField] private Vector3 offset;
    private float minScale = 0.01f;
    private float maxScale = 0.10f;
    private float minDistance = 50.0f;
    private float maxDistance = 500.0f;
    private Vector2 minMaxScale = new Vector2 ();
    private Vector2 minMaxDistance = new Vector2 ();

    private RectTransform canvasRect;
    private Transform rootPanel;
    private bool canvasCreated = false;

    private void Update ()
    {
        if (displayedIcons.Count > 0)
        {
            SetScaling ();
        }
    }

    private void SetScaling ()
    {
        float distance = Vector3.Distance ( transform.position, Camera.main.transform.position );
        float scale = Mathf.Lerp ( minScale, maxScale, Mathf.InverseLerp ( minDistance, maxDistance, distance ) );
        canvasRect.transform.localScale = new Vector3 ( scale, scale, scale );
    }

    private void CreateCanvas ()
    {
        GameObject go = Instantiate ( Resources.Load<GameObject> ( "UI/IconDisplay_Canvas_Prefab" ) );
        canvasRect = go.GetComponent<RectTransform> ();
        go.transform.SetParent ( this.transform );

        canvasRect.anchoredPosition3D = offset;
        canvasRect.localEulerAngles = Vector3.zero;

        rootPanel = go.transform.GetChild ( 0 );
        canvasCreated = true;
    }

    public void AddIcon (IconType type, Sprite sprite = null)
    {
        if (displayedIcons.Contains ( type )) return;
        if (!canvasCreated) CreateCanvas ();

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

    public void RemoveIcon(IconType type)
    {
        for (int i = 0; i < displayIconsGameObjects.Count; i++)
        {
            if(displayIconsGameObjects[i].GetComponentsInChildren<Image>()[1].sprite == GetIcon ( type ))
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

            default:
                return Resources.Load<Sprite> ( "UI/shoppingBasket" );
        }
    }
}
