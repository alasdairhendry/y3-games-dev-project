using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CitizenFootsteps : MonoBehaviour {

    private CitizenMovement cMovement;
    private bool state;

    [SerializeField] private GameObject prefab;
    private List<GameObject> footsteps = new List<GameObject> ();

    public void SetState(bool state, CitizenBase cBase)
    {
        this.state = state;
        this.cMovement = cBase.CitizenMovement;

        if (state)
        {
            cMovement.onPathConfirmed += UpdatePath;
            cMovement.onReachedPath += OnReachedPath;
            cMovement.onDestinationCleared += ClearFootsteps;
        }
        else
        {
            cMovement.onPathConfirmed -= UpdatePath;
            cMovement.onReachedPath -= OnReachedPath;
            cMovement.onDestinationCleared -= ClearFootsteps;
        }

        if (state)
        {
            UpdatePath ();
        }
        else
        {
            ClearFootsteps ();
        }
    }

    private void UpdatePath ()
    {
        if (!state) return;

        ClearFootsteps ();

        NavMeshPath path;

        if (cMovement.Path != null)
            path = cMovement.Path;
        else if (cMovement.GetAgentPath != null)
            path = cMovement.GetAgentPath;
        else { Debug.LogError ( "No path at all" ); ClearFootsteps (); return; }

        int counter = 0;

        for (int i = 0; i < path.corners.Length; i++)
        {
            if (i >= path.corners.Length - 1) continue;

            float distance = Mathf.Floor ( Vector3.Distance ( path.corners[i], path.corners[i + 1] ) );
            Vector3 direction = (path.corners[i + 1] - path.corners[i]).normalized;

            for (int x = 0; x < distance; x++)
            {
                GameObject go;

                if (counter >= footsteps.Count)
                {
                    go = Instantiate ( prefab );
                    footsteps.Add ( go );
                }
                else
                {
                    go = footsteps[counter];
                    go.SetActive ( true );
                }

                go.transform.SetParent ( transform );
                go.transform.position = (path.corners[i] + (direction * x)) + (Vector3.up * 0.2f);

                Quaternion lookRot = Quaternion.LookRotation ( direction );
                go.transform.rotation = lookRot;

                counter++;
            }
        }
    }

    private void OnReachedPath(Vector3 destination)
    {
        ClearFootsteps ();
    }

    private void ClearFootsteps ()
    {
        for (int i = 0; i < footsteps.Count; i++)
        {
            footsteps[i].SetActive ( false );
        }

        //footsteps.Clear ();
    }
}
