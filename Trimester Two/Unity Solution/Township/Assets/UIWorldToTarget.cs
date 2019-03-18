using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldToTarget : MonoBehaviour {

    private Transform target;
    private float offset;
    private RectTransform rect;
    private Vector3 position;

    private void Start ()
    {
        rect = GetComponent<RectTransform> ();
    }

    public void SetTarget(Transform target, float offset)
    {
        this.target = target;
        this.offset = offset;
    }

    private void Update () {
        if (target == null) return;

        position = Camera.main.WorldToScreenPoint ( target.position + (Vector3.up * offset) );
        position.x = Mathf.Lerp ( 0.0f, 1920.0f, position.x / Screen.width );
        position.y = Mathf.Lerp ( 0.0f, 1080.0f, position.y / Screen.height );

        rect.anchoredPosition = new Vector2 ( position.x, position.y );
    }
}
