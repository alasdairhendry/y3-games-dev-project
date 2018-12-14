using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canvas_HideAll : MonoBehaviour {

	public void ShowHide ()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild ( i ).gameObject.SetActive ( transform.GetChild ( i ).gameObject.activeSelf );
        }
    }
}
