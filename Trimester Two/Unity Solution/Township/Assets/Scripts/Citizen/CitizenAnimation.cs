using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenAnimation : MonoBehaviour {

    public CitizenBase cBase { get; protected set; }
    public Animator animator { get; protected set; }
    private AnimatorOverrideController overrideController;

    [SerializeField] private AnimationClip walkAdultClip;
    [SerializeField] private AnimationClip walkChildClip;

    public enum AnimationState { Idle, Walking, Carrying, Cart, Fishing, Squashing }

    private AnimationState animationState = AnimationState.Idle;
    public AnimationState SetAnimationState { set { animationState = value; ChangeAnimationState ( value ); } }

    public enum AxeUseAnimation { Chopping, Splitting }

    private void Awake ()
    {
        cBase = GetComponent<CitizenBase> ();
        animator = GetComponent<Animator> ();

        overrideController = new AnimatorOverrideController ( animator.runtimeAnimatorController );
        animator.runtimeAnimatorController = overrideController;
    }
	
	private void Update () {
		
	}

    public void SetWalkClip(bool adult)
    {
        if (adult)
        {
            List<KeyValuePair<AnimationClip, AnimationClip>> anims = new List<KeyValuePair<AnimationClip, AnimationClip>> ();

            foreach (AnimationClip a in overrideController.animationClips)
            {
                if (a.name == "Citizen_Child_Animation_Walk")
                {
                    anims.Add ( new KeyValuePair<AnimationClip, AnimationClip> ( a, walkAdultClip ) );
                    overrideController.ApplyOverrides ( anims );
                    break;
                }
            }    
        }
        else
        {
            List<KeyValuePair<AnimationClip, AnimationClip>> anims = new List<KeyValuePair<AnimationClip, AnimationClip>> ();

            foreach (AnimationClip a in overrideController.animationClips)
            {                
                if (a.name == "Citizen_Animation_Walk")
                {
                    anims.Add ( new KeyValuePair<AnimationClip, AnimationClip> ( a, walkChildClip ) );                    
                    overrideController.ApplyOverrides ( anims );
                    break;
                }
            }
        }
    }

    public void ChangeAnimationState (AnimationState state)
    {
        switch (state)
        {
            case AnimationState.Idle:
                animator.SetBool ( "isWalking", false );
                animator.SetBool ( "isCarrying", false );
                animator.SetBool ( "isWheelbarrow", false );
                animator.SetBool ( "isSquashing", false );
                animator.SetBool ( "isFishing", false );
                animator.SetFloat ( "IdleState", 0 );
                break;

            case AnimationState.Walking:
                animator.SetBool ( "isWalking", true );
                animator.SetBool ( "isFishing", false );
                animator.SetBool ( "isSquashing", false );
                animator.SetFloat ( "IdleState", 0 );
                break;

            case AnimationState.Carrying:
                animator.SetBool ( "isWalking", true );
                animator.SetBool ( "isCarrying", true );
                animator.SetBool ( "isFishing", false );
                animator.SetBool ( "isSquashing", false );
                animator.SetFloat ( "IdleState", 0 );
                break;

            case AnimationState.Cart:
                animator.SetBool ( "isWalking", true );
                animator.SetBool ( "isWheelbarrow", true );
                animator.SetBool ( "isSquashing", false );
                animator.SetBool ( "isFishing", false );
                animator.SetFloat ( "IdleState", 1 );
                break;

            case AnimationState.Fishing:
                animator.SetBool ( "isWalking", false );
                animator.SetBool ( "isWheelbarrow", false );
                animator.SetBool ( "isSquashing", false );
                animator.SetBool ( "isFishing", true );
                animator.SetFloat ( "IdleState", 1 );
                break;

            case AnimationState.Squashing:
                animator.SetBool ( "isWalking", false );
                animator.SetBool ( "isWheelbarrow", false );
                animator.SetBool ( "isFishing", false );
                animator.SetBool ( "isSquashing", true );
                animator.SetFloat ( "IdleState", 1 );
                break;

            default:
                break;
        }
    }

    public void SetState_Chopping (bool state)
    {
        animator.SetBool ( "isChopping", state );
        SetAnimationState = AnimationState.Idle;
    }

    public void SetState_Splitting (bool state)
    {
        animator.SetBool ( "isSplitting", state );
        SetAnimationState = AnimationState.Idle;
    }

    public void SetState_Pickaxing(bool state)
    {
        animator.SetBool ( "isPickaxe", state );
        SetAnimationState = AnimationState.Idle;
    }

    public void SetInjuryState (float healthValue)
    {
        animator.SetFloat ( "Health", healthValue );
    }
}
