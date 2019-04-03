using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Tutorial_Panel : UIPanel
{
    public static HUD_Tutorial_Panel Instance;

    [SerializeField] private TMP_Text headerText;
    [SerializeField] private TMP_Text stageText;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private Button okayButton;
    [SerializeField] private Button skipCurrent;

    private void Awake ()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy ( this.gameObject );
    }

    public override void Show ()
    {
        base.Show ();
    }

    public override void Hide ()
    {
        base.Hide ();
    }

    public void ShowTutorial (Tutorial tutorial, int displayStage, string body, bool incrementStage, System.Action onClick_Okay = null)
    {
        Show ();
        GetComponentInChildren<ScrollRect> ().verticalNormalizedPosition = 1.0f;

        headerText.text = tutorial.header;
        stageText.text = displayStage.ToString ( "00" ) + " / " + tutorial.displayStages.ToString ( "00" );
        bodyText.text = body;

        okayButton.onClick.RemoveAllListeners ();

        okayButton.onClick.AddListener ( () => { Hide (); } );

        if (onClick_Okay != null)
            okayButton.onClick.AddListener ( () => { onClick_Okay (); } );

        if (incrementStage)
            okayButton.onClick.AddListener ( () => { tutorial.IncrementStage (); } );

        skipCurrent.onClick.RemoveAllListeners ();
        skipCurrent.onClick.AddListener ( () => { Hide (); tutorial.Complete (); } );
    }

    public void OnClick_SkipAll ()
    {
        TutorialController.Instance.SkipAll ();
        Hide ();
    }

   
}
