using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tab_LoadGame : MonoBehaviour
{
    [SerializeField] private Transform parent;
    [SerializeField] private GameObject filePrefab;
    [SerializeField] private Button loadButton;

    List<PersistentData.SaveData> saveFiles = new List<PersistentData.SaveData> ();

    private void Start ()
    {
        Refresh ();
    }

    private void OnEnable ()
    {
        Refresh ();
    }

    private void Refresh ()
    {
        FetchSaveFiles ();
        DestroyOldFiles ();
        CreateNewFiles ();
    }

    private void FetchSaveFiles ()
    {
        saveFiles.Clear ();

        foreach (string file in PersistentData.FetchSaveFiles ())
        {
            saveFiles.Add ( PersistentData.Load ( file ) );
        }

        saveFiles = saveFiles.OrderByDescending ( x => x?.lastPlayed ).ToList ();
    }

    private void DestroyOldFiles ()
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Destroy ( parent.GetChild ( i ).gameObject );
        }
    }

    private void CreateNewFiles ()
    {
        bool selected = false;

        for (int i = 0; i < saveFiles.Count; i++)
        {
            if (!selected && saveFiles[i] != null)
            {
                CreateFile ( saveFiles[i], true );
                selected = true;
            }
            else
            {
                CreateFile ( saveFiles[i], false );
            }
        }
    }

    private void CreateFile (PersistentData.SaveData data, bool select)
    {
        //PersistentData.SaveData data = PersistentData.Load ( saveFile );

        //if (data == null) return; // Create corrupt file button thing

        GameObject go = Instantiate ( filePrefab );
        go.transform.SetParent ( parent );

        TMP_Text[] texts = go.GetComponentsInChildren<TMP_Text> ();

        texts[0].text = (data == null) ? "Save File Corrupt" : data.townName;
        texts[1].text = (data == null) ? "0" : data.citizens.Count.ToString ( "0" );

        if (data == null)
        {
            go.GetComponent<Button> ().onClick.AddListener ( () =>
            {
                loadButton.onClick.RemoveAllListeners ();
                HUD_Dialogue_Panel.Instance.ShowDialogue ( "Save File Corrupt", "This save file is corrupted and can't be loaded. \nIf you have made a backup, paste it into the save files directory.",
                               new DialogueButton ( DialogueButton.Preset.Okay, null ) );
            } );
        }
        else
        {
            go.GetComponent<Button> ().onClick.AddListener ( () =>
            {
                ShowSaveData ( data );
                loadButton.GetComponentInChildren<TMP_Text> ().text = "Enter " + data.townName;
                loadButton.onClick.RemoveAllListeners ();
                loadButton.onClick.AddListener ( () => { SaveLoad.Instance.Load ( data.townName ); loadButton.interactable = false; } );
            } );
        }

        if (select)
        {
            go.GetComponent<Button> ().onClick?.Invoke ();
        }
    }

    private void ShowSaveData (PersistentData.SaveData data)
    {
        Transform leftPanel = transform.Find ( "Box_FileInfo" ).Find ( "LeftPanel" );
        Transform rightPanel = transform.Find ( "Box_FileInfo" ).Find ( "RightPanel" );

        leftPanel.transform.Find ( "Name" ).Find ( "Value" ).GetComponent<TMP_Text> ().text = data.townName;
        leftPanel.transform.Find ( "LastPlayed" ).Find ( "Value" ).GetComponent<TMP_Text> ().text = data.lastPlayed.ToShortTimeString () + " " + ToShortDate ( data.lastPlayed );
        leftPanel.transform.Find ( "Settled" ).Find ( "Value" ).GetComponent<TMP_Text> ().text = data.created.ToShortTimeString () + " " + ToShortDate ( data.created );
        leftPanel.transform.Find ( "Region" ).Find ( "Value" ).GetComponent<TMP_Text> ().text = WorldPresets.Instance.Presets[data.worldPresetIndex].presetName;

        rightPanel.transform.Find ( "Money" ).Find ( "Value" ).GetComponent<TMP_Text> ().text = MoneyController.BeautifyMoney ( data.money );
        rightPanel.transform.Find ( "Citizens" ).Find ( "Value" ).GetComponent<TMP_Text> ().text = data.citizens.Count.ToString ( "0" );
        rightPanel.transform.Find ( "Buildings" ).Find ( "Value" ).GetComponent<TMP_Text> ().text = data.props.Count.ToString ( "0" );
        rightPanel.transform.Find ( "Date" ).Find ( "Value" ).GetComponent<TMP_Text> ().text = string.Format ( "Day {0}, Month {1}, Year {2}", data.gameTime.dayOfMonth, data.gameTime.month, data.gameTime.year );
    }

    private string ToShortDate (System.DateTime date)
    {
        date = System.DateTime.Now;
        return date.Day.ToString ( "00" ) + "/" + date.Month.ToString ( "00" ) + "/" + date.Year.ToString ().Remove ( 0, 2 );
    }

    public void QuickLoad ()
    {
        FetchSaveFiles ();

        if (saveFiles.Count == 0)
        {
            HUD_Dialogue_Panel.Instance.ShowDialogue ( "No Save Files", "No previous save files were found that could be quick-loaded.", new DialogueButton ( DialogueButton.Preset.Okay, null ) );
            return;
        }

        PersistentData.SaveData data = saveFiles[0];

        if (data == null)
        {
            HUD_Dialogue_Panel.Instance.ShowDialogue ( "Save File Corrupt", "This save file is corrupted and can't be loaded. \nIf you have made a backup, paste it into the save files directory.",
                              new DialogueButton ( DialogueButton.Preset.Okay, null ) );
        }
        else
        {
            SaveLoad.Instance.Load ( data.townName );
            loadButton.interactable = false;
        }
    }
}
