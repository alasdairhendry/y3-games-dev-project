using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadState {

	public bool isComplete { get; protected set; }

    public System.Action onStart;
    public System.Action onComplete;

    private Dictionary<string, bool> loadStages = new Dictionary<string, bool>();

    public void AddStage(string b, bool s)
    {
        isComplete = false;
        
        if(loadStages.Count <= 0)
        {
            if (onStart != null) onStart();
        }

        loadStages.Add(b, s);
    }

    public void UpdateStage(string b, bool s)
    {
        loadStages[b] = s;
        CheckStages();
    }

    private void CheckStages()
    {
        foreach (KeyValuePair<string, bool> item in loadStages)
        {
            if(item.Value == false)
            {
                return;           
            }
        }

        isComplete = true;
        loadStages.Clear();
        if (onComplete != null)
            onComplete();
    }
}
