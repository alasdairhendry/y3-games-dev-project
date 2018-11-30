using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_PopOut_Panel : MonoBehaviour {

    [SerializeField] private GameObject popOutOptionPrefab;
    [SerializeField] private int optionCount;

    private void Start ()
    {
        Create ();
    }

    [ContextMenu("Create")]
    public void Create ()
    {
        float radialFill = 1.0f / (float)optionCount;

        for (int i = 0; i < optionCount; i++)
        {
            CreateOption ( radialFill, i * (360.0f / (float)optionCount) );
        }
    }

    private void CreateOption(float radialFill, float angle)
    {
        GameObject go = Instantiate ( popOutOptionPrefab );
        go.GetComponentsInChildren<Image> ()[0].fillAmount = radialFill;
        go.GetComponentsInChildren<Image> ()[1].transform.localEulerAngles = new Vector3 ( 0.0f, 0.0f, angle - 45.0f );
        go.GetComponent<RectTransform> ().localEulerAngles = new Vector3 ( 0.0f, 0.0f, angle );
        go.transform.SetParent ( this.transform );
        go.GetComponent<RectTransform> ().anchoredPosition = Vector3.zero;
        go.GetComponent<RectTransform> ().localScale = Vector3.one;
    }

}
