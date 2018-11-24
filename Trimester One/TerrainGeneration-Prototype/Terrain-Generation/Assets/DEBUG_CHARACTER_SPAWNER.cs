using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEBUG_CHARACTER_SPAWNER : MonoBehaviour {

    [SerializeField] Vector3 spawnPosition;
    [SerializeField] private GameObject prefab;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown ( KeyCode.C ))
        {
            Instantiate ( prefab, spawnPosition, Quaternion.identity );
        }
	}
}
