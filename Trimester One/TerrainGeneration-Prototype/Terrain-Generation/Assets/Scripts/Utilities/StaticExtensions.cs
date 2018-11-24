using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticExtensions {

    public static bool IsNull(this Job job)
    {
        Debug.Log ( "Check " + job.ID );

        if (job == null || job.ID <= 0)
        {
            return true;
        }
        else return false;
    }
	
}
