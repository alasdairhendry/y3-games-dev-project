using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoSingleton<TutorialController> {

    [SerializeField] private bool tutorialIsActive = true;
    public bool TutorialIsActive { get { return tutorialIsActive; } }

    private List<Tutorial> tutorials = new List<Tutorial> ();

    public void Start_NewGame ()
    {
        tutorials.Add ( new Tutorial_PlaceInitialProps () );
        tutorials.Add ( new Tutorial_GatheringResources () );
        tutorials.Add ( new Tutorial_Marketplace () );
        tutorials.Add ( new Tutorial_Trading () );

        Begin ( 0 );
    }

    public void Load ()
    {

    }

    public void Begin (int index)
    {
        if (tutorials[index].isComplete) return;
        tutorials[index].Begin ();
    }

    public void SkipAll ()
    {
        for (int i = 0; i < tutorials.Count; i++)
        { 
            tutorials[i].Complete ();
        }
    }
}
