using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGraphics : MonoBehaviour {

    private CharacterMovement characterMovement;
    [SerializeField] private GameObject marketCartGraphics;
    [SerializeField] private GameObject satchelGraphics;
    [SerializeField] private GameObject axeGraphics;
    [SerializeField] private Transform[] axePlaceholders;

    private void Start ()
    {
        characterMovement = GetComponent<CharacterMovement> ();
        GetComponent<Character> ().Inventory.RegisterOnInventoryChanged ( OnInventoryChanged );        
    }

    public void OnInventoryChanged(ResourceInventory inventory)
    {
        if (inventory.IsEmpty ())
        {
            if (satchelGraphics != null)
                satchelGraphics.SetActive ( false );

            GetComponent<IconDisplayer> ().RemoveIcon ( IconDisplayer.IconType.Inventory );
            characterMovement.SetAnimationState = CharacterMovement.AnimationState.Walking;
        }
        else
        {
            if (GetComponent<Character> ().GetCurrentJob.IsNull ())
            {
                characterMovement.SetAnimationState = CharacterMovement.AnimationState.Walking;
                GetComponent<IconDisplayer> ().AddIcon ( IconDisplayer.IconType.Inventory );         

                if (satchelGraphics != null)
                    satchelGraphics.SetActive ( true );
            }
            //else
            //{
            //    

            //    if (marketCartGraphics != null)
            //        marketCartGraphics.SetActive ( true );
            //}

        }
    }

    public void OnUseAxeAction(bool useAxe)
    {
        if (useAxe)
        {
            axeGraphics.transform.SetParent ( axePlaceholders[1] );
            axeGraphics.transform.localEulerAngles = Vector3.zero;
            axeGraphics.transform.localPosition = Vector3.zero;
            characterMovement.SetUsingTool ( useAxe );
        }
        else
        {
            axeGraphics.transform.SetParent ( axePlaceholders[0] );
            axeGraphics.transform.localEulerAngles = Vector3.zero;
            axeGraphics.transform.localPosition = Vector3.zero;
            characterMovement.SetUsingTool ( useAxe );
        }
    }

    public void SetUsingCart(bool state)
    {
        if (marketCartGraphics != null)
            marketCartGraphics.SetActive ( state );

        characterMovement.SetAnimationState = state == true ? CharacterMovement.AnimationState.Cart : CharacterMovement.AnimationState.Idle;
    }
}
