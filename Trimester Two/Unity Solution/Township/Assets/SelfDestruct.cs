using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SelfDestruct : MonoBehaviour {

    [SerializeField] private float lifetime;
    [SerializeField] private bool playOnAwake;
    private System.Action onDestroyAction;
    private bool isPlaying = false;

    private void Awake ()
    {
        if (playOnAwake) isPlaying = true;
    }
	
	private void Update ()
    {
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
        if (onDestroyAction != null) onDestroyAction ();
        Destroy ( this.gameObject );
    }
}
