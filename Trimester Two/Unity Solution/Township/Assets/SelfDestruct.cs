using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SelfDestruct : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField] private float lifetime;
    [SerializeField] private bool playOnAwake;
    [SerializeField] private bool inactiveOnHover = false;
    private bool isHovered = false;
    private System.Action onDestroyAction;
    private bool isPlaying = false;

    private void Awake ()
    {
        if (playOnAwake) isPlaying = true;
    }
	
	private void Update ()
    {
        if (inactiveOnHover)
        {
            if (isHovered) return;
        }

        if (isPlaying)
        {
            lifetime -= Time.deltaTime;

            if(lifetime <= 0.0f)
            {
                Destroy ();
            }
        }	
	}

    public void SetLifetime(float lifeTime)
    {
        this.lifetime = lifeTime;
    }

    public void RegisterOnDestroy(System.Action action)
    {
        onDestroyAction += action;
    }

    public void Play()
    {
        if (!isPlaying)
            isPlaying = true;
    }

    public void DestroyNow ()
    {
        Destroy ();
    }

    private void Destroy ()
    {
        onDestroyAction?.Invoke ();
        Destroy ( this.gameObject );
    }

    void IPointerEnterHandler.OnPointerEnter (PointerEventData eventData)
    {
        isHovered = true;
    }

    void IPointerExitHandler.OnPointerExit (PointerEventData eventData)
    {
        isHovered = false;
    }
}
