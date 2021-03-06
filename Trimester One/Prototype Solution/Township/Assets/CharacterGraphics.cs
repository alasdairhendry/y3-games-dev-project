﻿using System.Collections;
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
        GetComponent<Character> ().Inventory.RegisterOnInventoryChanged ( OnInventoryChanged );
    }

    public void OnInventoryChanged(ResourceInventory inventory)
    {
        if (inventory.IsEmpty ())
        {
            satchelGraphics.SetActive ( false );
            GetComponent<IconDisplayer> ().RemoveIcon ( IconDisplayer.IconType.Inventory );
        }
        else
        {
            satchelGraphics.SetActive ( true );

            if (GetComponent<Character> ().GetCurrentJob.IsNull ())
                GetComponent<IconDisplayer> ().AddIcon ( IconDisplayer.IconType.Inventory );
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
