using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenAnimation : MonoBehaviour {

    public CitizenBase cBase { get; protected set; }
    public Animator animator { get; protected set; }

    [SerializeField] private AnimationClip walkAdultClip;
    [SerializeField] private AnimationClip walkChildClip;

    private void Awake ()
    {
        cBase = GetComponent<CitizenBase> ();
        animator = GetComponent<Animator> ();
    }

    private void Start () {
        SetWalkClip ( true );
	}
	
	private void Update () {
		
	}

    public void SetWalkClip(bool adult)
    {
        AnimatorOverrideController controller = new AnimatorOverrideController ( animator.runtimeAnimatorController );        

        Debug.Log ( controller["Walking"] );
        //Debug.Log ( animator.runtimeAnimatorController["Walk"] );
        //if (adult)
        //{
        //    animator.getcur
        //}
    }
}
