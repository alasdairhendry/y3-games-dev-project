﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class BuildMode : MonoBehaviour {
    
    public bool isActive;

    public System.Action OnActivate;
    public System.Action OnDeactivate;

    private PropData currentPropData;
    private GameObject currentPropOutline;
    private float outlineYRotation;

    [SerializeField] private Material[] outlineMaterials;

    public void SetActive(bool state)
    {
        if (currentPropOutline == null) currentPropOutline = new GameObject ( "propOutline" );
        isActive = state;
        TriggerCallback ();
    }

    public void Toggle ()
    {
        isActive = !isActive;
        TriggerCallback ();
    }

    public void SetPropData (PropData data)
    {
        DestroyPropOutline ();

        currentPropData = data;
        GameObject g = Instantiate ( data.Prefab );
        g.transform.SetParent ( currentPropOutline.transform );
        g.transform.localPosition = Vector3.zero;
        g.transform.localEulerAngles = Vector3.zero;
        g.transform.localScale = Vector3.one;

        currentPropOutline.transform.rotation = Quaternion.identity;
        outlineYRotation = 0.0f;
    }

    private void Update ()
    {
        if (!isActive) return;
        if (currentPropOutline == null) return;

        if (Input.GetKeyDown ( KeyCode.D ))
        {            
            Destroy ( GameObject.Find ( "PlacedObject: House" ) );
        }

        Ray ray = Camera.main.ScreenPointToRay ( Input.mousePosition );
        RaycastHit hit;
        int mask = 1 << 9;

        if (Physics.Raycast ( ray, out hit, 10000, mask ))
        {
            MovementHandle ( hit.point );
            GraphicsHandle ( hit.point );

            if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftControl))
                PlaceHandle (hit.point);
        }

        RotationHandle ();
    }

    private void MovementHandle (Vector3 point)
    {        
        currentPropOutline.transform.position = point;
    }

    private void RotationHandle ()
    {
        Vector3 r = currentPropOutline.transform.localEulerAngles;
        if (Input.GetKey ( KeyCode.LeftShift ))
            outlineYRotation -= (Input.GetKey ( KeyCode.R ) ? 1 : 0) * Time.deltaTime * 75.0f;
        else outlineYRotation += (Input.GetKey ( KeyCode.R ) ? 1 : 0) * Time.deltaTime * 75.0f;
        r.y = outlineYRotation;

        currentPropOutline.transform.rotation = Quaternion.Slerp ( currentPropOutline.transform.rotation, Quaternion.Euler ( r ), Time.deltaTime * 20 );
    }

    private void GraphicsHandle (Vector3 point)
    {
        if (SampleEligibility(point, 1))
        {
            Renderer[] r = currentPropOutline.GetComponentsInChildren<Renderer> ();
            foreach (Renderer renderer in r)
            {
                renderer.material = outlineMaterials[0];
            }
        }
        else
        {
            Renderer[] r = currentPropOutline.GetComponentsInChildren<Renderer> ();
            foreach (Renderer renderer in r)
            {
                renderer.material = outlineMaterials[1];
            }
        }
    }    

    private void PlaceHandle (Vector3 point)
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if(SampleEligibility(point, 1))
        {
            GameObject go = Instantiate ( currentPropData.Prefab );
            go.transform.position = point;
            go.transform.rotation = currentPropOutline.transform.rotation;
            go.transform.name = "PlacedObject: " + currentPropData.name;
            Prop prop = go.GetComponent<Prop> ();
            prop.Place (currentPropData);
        }
    }

    private bool SampleNavMesh (Vector3 point, int mask)
    {
        NavMeshHit hit;

        if (NavMesh.SamplePosition ( point, out hit, 1.0f, NavMesh.AllAreas ))
        {
            if (hit.mask == mask)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private bool SampleCollider ()
    {
        return !currentPropOutline.GetComponentInChildren<SpatialCollider> ().IsColliding;
    }

    private bool SampleEligibility (Vector3 point, int mask)
    {
        if (SampleNavMesh ( point, 1 ) && SampleCollider ())
        {
            return true;
        }
        else return false;
    }

    private void TriggerCallback ()
    {
        if (isActive) if (OnActivate != null) OnActivate ();
        if (!isActive) if (OnDeactivate != null) OnDeactivate ();

        DestroyPropOutline ();
    }   
    
    private void DestroyPropOutline ()
    {
        if (currentPropOutline.transform.childCount > 0)
            Destroy ( currentPropOutline.transform.GetChild ( 0 ).gameObject );
    }
}