using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hud_Professions_Panel : UIPanel {

    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform contentTransform;

    private Dictionary<ProfessionType, TextMeshProUGUI> textObjects = new Dictionary<ProfessionType, TextMeshProUGUI> ();

    ProfessionType cycleType = ProfessionType.None;
    int cycleIndex = 0;

    protected override void Start ()
    {
        base.Start ();

        List<Profession> profs = ProfessionController.Instance.professions;

        for (int i = 0; i < profs.Count; i++)
        {

            if (i == 0) continue;

            int count = i;

            GameObject go = Instantiate ( prefab );
            go.transform.SetParent ( contentTransform );

            TextMeshProUGUI[] texts = go.GetComponentsInChildren<TextMeshProUGUI> ();
            texts[0].text = profs[i].type.ToString ();
            texts[1].text = ProfessionController.Instance.professionsByCitizen[profs[i].type].Count.ToString ( "0" );

            texts[0].gameObject.AddComponent<Button> ().onClick.AddListener ( () => { Cycle ( profs[count].type ); } );

            textObjects.Add ( profs[i].type, texts[1] );

            Button[] buttons = go.GetComponentsInChildren<Button> ();

            if (i <= 2)
            {
                buttons[1].gameObject.SetActive ( false );
                buttons[2].gameObject.SetActive ( false );
                continue;
            }

            buttons[1].onClick.AddListener ( () =>
            {
                ProfessionController.Instance.DecreaseProfession ( ProfessionController.Instance.professions[count].type );
            } );

            buttons[2].onClick.AddListener ( () =>
            {
                ProfessionController.Instance.IncreaseProfession ( ProfessionController.Instance.professions[count].type );
            } );

        }

        ProfessionController.Instance.onRefreshProfessions += OnProfessionsChanged;
    }

    private void Cycle(ProfessionType type)
    {
        if(type == cycleType)
        {
            cycleIndex++;

            if (cycleIndex >= ProfessionController.Instance.professionsByCitizen[type].Count) cycleIndex = 0;
        }
        else
        {
            cycleType = type;
            cycleIndex = 0;
        }

        if (ProfessionController.Instance.professionsByCitizen[type].Count <= 0) return;
        ProfessionController.Instance.professionsByCitizen[type][cycleIndex].Inspectable.InspectAndLockCamera ();
    }

    private void OnProfessionsChanged ()
    {
        List<Profession> profs = ProfessionController.Instance.professions;

        for (int i = 0; i < profs.Count; i++)
        {
            if (textObjects.ContainsKey ( profs[i].type ))
                textObjects[profs[i].type].text = ProfessionController.Instance.professionsByCitizen[profs[i].type].Count.ToString ( "0" );
        }
    }
}
