using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wobblable : MonoBehaviour {

    [SerializeField] private float breakSpeed = 2.0f;    
    [SerializeField] private float gravityAcceleration = 0.5f;
    private float currentGravitySpeed = 0.0f;
    private float currentGravity = 0.0f;

    [SerializeField] private float time = 1.0f;
    private float currentTime = 0.0f;

    private float breakage = -1;
    private bool isBreaking = false;

    private Material material;

    private System.Action OnComplete;

    [ContextMenu("Do")]
    public void Break (System.Action onComplete)
    {
        if (isBreaking) return;
        this.OnComplete += onComplete;

        isBreaking = true;
    }

    private void Check ()
    {
        currentTime += GameTime.DeltaGameTime;
        breakage += GameTime.DeltaGameTime * breakSpeed;
        currentGravitySpeed += GameTime.DeltaGameTime * gravityAcceleration;
        currentGravity -= currentGravitySpeed;     


        if(currentTime>= time)
        {
            currentTime = 0.0f;
            breakage = 0.0f;
            currentGravity = 0.0f;
            currentGravitySpeed = 0.0f;
            isBreaking = false;

            if (OnComplete != null) OnComplete ();
        }
        material.SetFloat ( "_Breakage", Mathf.Sin ( breakage ) );
        material.SetFloat ( "_Gravity", currentGravity);
    }

	// Use this for initialization
	void Start () {
        material = GetComponentInChildren<Renderer> ().material;
	}
	
	// Update is called once per frame
	void Update () {
        if (!isBreaking) return;
        Check ();
	}
}
