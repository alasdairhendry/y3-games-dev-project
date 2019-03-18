using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourGroupController : MonoBehaviour {

    public static ColourGroupController Instance;

    [SerializeField] private ScriptableColourGroup group;
    public ScriptableColourGroup Group { get { return group; } }

    private void Awake ()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy ( this.gameObject );
    }

    public Color GetColour(string name)
    {
        Color c = Color.white;

        for (int i = 0; i < group.group.Count; i++)
        {
            if (group.group[i].name == name)
            {
                c = group.group[i].colour;
                break;
            }
        }

        return c;
    }
}
