using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEBUG_CHARACTER_SPAWNER : MonoBehaviour {

    [SerializeField] Vector3 spawnPosition;
    [SerializeField] private GameObject prefab;

    public void SpawnCharacter ()
    {
        if (FindObjectsOfType<Character> ().Length == 0)
            Instantiate ( prefab, spawnPosition, Quaternion.identity );
        
    }
}
