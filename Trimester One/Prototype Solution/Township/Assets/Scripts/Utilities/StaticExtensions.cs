﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticExtensions {

    public static bool IsNull(this Job job)
    {
        if (job == null || job.ID <= 0)
        {
            return true;
        }
        else return false;
    }
	
    public static string ToDescription(this float value, Dictionary<float, string> descriptions)
    {
        if (!descriptions.ContainsKey ( value )) return "null";
        return descriptions[value];
    }
}
