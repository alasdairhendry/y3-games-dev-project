using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGraphics : MonoBehaviour {

    private CharacterMovement characterMovement;
    [SerializeField] private GameObject satchelGraphics;
    [SerializeField] private GameObject axeGraphics;
    [SerializeField] private Transform[] axePlaceholders;

    private void Start ()
    {
        characterMovement = GetComponent<CharacterMovement> ();
    }

    public void OnInventoryChanged(ResourceInventory inventory)
    {
        if (inventory.IsEmpty ())
        {
            satchelGraphics.SetActive ( false );
            Debug.Log ( "OnInventoryChanged " + "false" );
        }
        else
        {
            satchelGraphics.SetActive ( true );
            Debug.Log ( "OnInventoryChanged " + "true" );
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
}
