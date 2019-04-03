using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartnerController : MonoSingleton<PartnerController> {

    private List<CitizenBase> eligibleMales = new List<CitizenBase> ();
    private List<CitizenBase> eligibleFemales = new List<CitizenBase> ();
    
    public void SetEligible (CitizenBase cBase)
    {
        if (eligibleMales.Contains ( cBase )) return;
        if (eligibleFemales.Contains ( cBase )) return;

        if (cBase.CitizenFamily.gender == CitizenFamily.Gender.Male)
        {
            eligibleMales.Add ( cBase );
            cBase.OnCitizenDied += (_cBase) => { SetIneligible ( _cBase ); };
        }
        else if (cBase.CitizenFamily.gender == CitizenFamily.Gender.Female)
        {
            eligibleFemales.Add ( cBase );
            cBase.OnCitizenDied += (_cBase) => { SetIneligible ( _cBase ); };
        }

        Check ();
    }

    public void SetIneligible(CitizenBase cBase)
    {
        if (!eligibleMales.Contains ( cBase ) && !eligibleFemales.Contains ( cBase )) return;

        if (cBase.CitizenFamily.gender == CitizenFamily.Gender.Male)
        {
            eligibleMales.Remove ( cBase );
            cBase.OnCitizenDied -= (_cBase) => { SetIneligible ( _cBase ); };
        }
        else if (cBase.CitizenFamily.gender == CitizenFamily.Gender.Female)
        {
            eligibleFemales.Remove ( cBase );
            cBase.OnCitizenDied -= (_cBase) => { SetIneligible ( _cBase ); };
        }
    }

    private void Check ()
    {
        if(eligibleMales.Count > 0 && eligibleFemales.Count > 0)
        {
            eligibleMales[0].CitizenFamily.SetPartner ( eligibleFemales[0].CitizenFamily.thisMember );
            eligibleFemales[0].CitizenFamily.SetPartner ( eligibleMales[0].CitizenFamily.thisMember );

            CitizenController.Instance.SetAsNewFamily ( eligibleMales[0], eligibleFemales[0] );

            eligibleMales[0].CitizenHousing.LeaveHouse ( true );
            eligibleFemales[0].CitizenHousing.LeaveHouse ( true );

            SetIneligible ( eligibleMales[0] );
            SetIneligible ( eligibleFemales[0] );
        }
    }
}
