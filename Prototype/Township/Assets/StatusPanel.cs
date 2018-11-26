using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusPanel : MonoBehaviour {

    [SerializeField] private CanvasGroup group;

	// Use this for initialization
	void Start () {
        GameObject.FindObjectOfType<TerrainGenerator>().onBeginGenerate += OnBegin;
        GameObject.FindObjectOfType<TerrainGenerator>().onEndGenerate += OnEnd;
        group.alpha = 0;
    }
	
	void OnBegin()
    {
        group.alpha = 1;
    }

    void OnEnd()
    {
        group.alpha = 0;
    }
}
