using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_LoadingOverlay_Panel : UIPanel {

    private World world;
    [SerializeField] private Text text;

	protected override void Start () {
        base.Start ();

        world = GameObject.FindObjectOfType<World>();
        world.OnTerrainBeginGenerate += OnBegin;
        world.OnTerrainGenerateStateChange += OnChange;
        world.OnTerrainEndGenerate += OnEnd;
	}

    private void OnBegin()
    {
        Show ();
        text.text = "Loading...";
    }

    private void OnChange(int curr, int total)
    {
        text.text = "Loading " + (((float)curr / (float)total) * 100.0f).ToString("0.00") + "%...";
    }

    private void OnEnd()
    {
        Hide ();
    }
}
