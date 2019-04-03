using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovableSiblingPanel : MonoBehaviour, IPointerDownHandler {

    [SerializeField] private Transform transformToMove;    

    void IPointerDownHandler.OnPointerDown (PointerEventData eventData)
    {
        BringToFront ();
    }

    public void BringToFront ()
    {
        transformToMove.SetAsLastSibling ();
    }
}
