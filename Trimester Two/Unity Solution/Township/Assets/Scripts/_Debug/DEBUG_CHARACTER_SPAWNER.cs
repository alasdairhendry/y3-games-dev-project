using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DEBUG_CHARACTER_SPAWNER : MonoBehaviour {

    [SerializeField] Vector3 spawnPosition;
    [SerializeField] private GameObject prefab;
    private int spawnCounter = 1;

    public void SpawnCharacter ()
    {
        GameObject go = Instantiate ( prefab, spawnPosition, Quaternion.identity );
        go.GetComponent<NavMeshAgent> ().avoidancePriority = spawnCounter;

        spawnCounter++;

        if (spawnCounter > 100) spawnCounter = 1;
    }
}
