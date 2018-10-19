using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour {

    public static GameState Instance;
    public LoadState worldLoadState = new LoadState();

    private void Awake()
    {
        Debug.Log("GameState");
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        worldLoadState.onComplete += OnWorldLoaded;
    }

    private void OnWorldLoaded()
    {
        Debug.Log("World Loaded");
    }

    private void OnDestroy()
    {
        worldLoadState.onComplete -= OnWorldLoaded;
    }
}
