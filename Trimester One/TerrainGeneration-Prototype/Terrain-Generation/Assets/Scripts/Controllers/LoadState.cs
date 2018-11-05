using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadState {

    public string name { get; protected set; }
    public bool isRunning { get; protected set; }
	public bool isComplete { get; protected set; }

    public System.Action onStart;
    public System.Action<int,int> onStageComplete;
    public System.Action onComplete;

    public int currentStage { get; protected set; }

    private Dictionary<string, bool> loadStages = new Dictionary<string, bool>();

    public LoadState(string name)
    {
        this.name = name;
    }

    public void Begin()
    {
        if (isRunning) { Debug.LogError(name + ": Can't run a load state asynchronously"); return; }

        if (onStart != null) onStart();

        isRunning = true;
        isComplete = false;
        currentStage = 0;
        CheckStages();
    }

    public void AddStage(string b, bool s = false)
    {
        if (isRunning) { Debug.LogError(name + ": Can't reset load stages. Load stage is running."); return; }
        
        if (loadStages.ContainsKey(b)) { Debug.LogError(name + ": Load stage (" + b + ") already exists"); return; }
        loadStages.Add(b, s);
    }

    public void ResetStage()
    {
        if (isRunning) { Debug.LogError(name + ": Can't reset load stages. Load stage is running."); return; }
        loadStages.Clear();
    }

    public void UpdateStage(string b, bool s)
    {
        if (!loadStages.ContainsKey(b)) { Debug.LogError(name + ": Can't update load stage (" + b + "), as it doesnt exist"); return; }

        loadStages[b] = s;
        CheckStages();
    }

    private void CheckStages()
    {
        int x = 0;
        foreach (KeyValuePair<string, bool> item in loadStages)
        {
            if(item.Value == true)
            {
                x++;       
            }
        }

        if(x > currentStage)
        {
            if (onStageComplete != null)
                onStageComplete(x, loadStages.Count);
        }

        currentStage = x;

        if (currentStage >= loadStages.Count)
        {
            isComplete = true;
            isRunning = false;
            if (onComplete != null)
                onComplete();
        }
    }
}
