using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Colour Group/New")]
public class ScriptableColourGroup : ScriptableObject {

    public List<ColourGroup> group = new List<ColourGroup> ();

    [System.Serializable]
    public class ColourGroup
    {
        public string name = "";
        public Color colour = Color.white;

        public ColourGroup (string name, Color colour)
        {
            this.name = name;
            this.colour = colour;
        }
    }
}
