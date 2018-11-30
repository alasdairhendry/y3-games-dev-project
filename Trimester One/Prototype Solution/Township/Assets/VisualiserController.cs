using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualiserController : MonoBehaviour {

    public static VisualiserController Instance;

    [SerializeField] private List<PropVisualiser> propVisualisers = new List<PropVisualiser> ();
    [SerializeField] private bool isOn = false;
    public bool IsOn { get { return isOn; } }

    [SerializeField] private Material[] materials;
    [SerializeField] private int matIndex;

    private void Awake ()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy ( this.gameObject );
    }

    private void Update ()
    {
        if (Input.GetKeyDown ( KeyCode.P ))
        {
            if (isOn) TurnOff ();
            else Visualise ( materials[matIndex],
                (p, m) =>
                {
                    Material _m = new Material ( m );
                    _m.color = Color.Lerp ( Color.black, Color.white, p.transform.position.x / 100.0f );
                    return _m;
                } );
        }
    }

    public void Visualise(Material material, System.Func<PropVisualiser, Material, Material> MaterialCheck)
    {
        isOn = true;

        for (int i = 0; i < propVisualisers.Count; i++)
        {
            Material m;

            if(MaterialCheck == null)
            {
                m = material;
            }
            else m = MaterialCheck ( propVisualisers[i], material );

            propVisualisers[i].Visualise ( m );
        }
    }

    public void TurnOff ()
    {
        isOn = false;

        for (int i = 0; i < propVisualisers.Count; i++)
        {
            propVisualisers[i].TurnOff ();
        }
    }

	public void AddVisualiser (PropVisualiser visualiser)
    {
        if (!propVisualisers.Contains ( visualiser )) propVisualisers.Add ( visualiser );
    }

    public void RemoveVisualiser(PropVisualiser visualiser)
    {
        if (propVisualisers.Contains ( visualiser )) propVisualisers.Remove ( visualiser );
    }
}
