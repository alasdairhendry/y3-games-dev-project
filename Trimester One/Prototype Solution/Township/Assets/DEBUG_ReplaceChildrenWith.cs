using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DEBUG_ReplaceChildrenWith : MonoBehaviour {

    [SerializeField] private GameObject _PREFAB;
    [SerializeField] private string _NAME;

    private List<Vector3> positions = new List<Vector3> ();
    private List<Vector3> rotations = new List<Vector3> ();

    [ContextMenu("Replace")]
    public void Replace ()
    {
        int count = this.transform.childCount;

        for (int i = 0; i < count; i++)
        {
            positions.Add ( this.transform.GetChild ( i ).position );
            rotations.Add ( this.transform.GetChild ( i ).eulerAngles );          
        }

        for (int i = 0; i < count; i++)
        {
            DestroyImmediate ( this.transform.GetChild ( i ).gameObject );
        }

        for (int i = 0; i < count; i++)
        {
            GameObject go = Instantiate ( _PREFAB );
            go.transform.SetParent ( this.transform );
            go.name = _NAME;
            go.transform.position = positions[i];
            go.transform.eulerAngles = rotations[i];
        }
    }

}
