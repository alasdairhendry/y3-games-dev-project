using System.Collections;
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

    public static float[] ToFloat3(this Vector3 value)
    {
        return new float[] { value.x, value.y, value.z };
    }

    public static Vector3 ToVector3(this float[] value)
    {
        if (value.Length != 3) { Debug.LogError ( "Array doesnt contain 3 elements" ); return new Vector3 (); }
        return new Vector3 ( value[0], value[1], value[2] );
    }

    public static int Loop(int value, int min, int max)
    {
        if (value > max) return min;
        else if (value < min) return max;
        else return value;
    }

    public static void ShuffleList<E> (List<E> list)
    {
        System.Random random = new System.Random ();

        if (list.Count > 1)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                E tmp = list[i];
                int randomIndex = random.Next ( i + 1 );

                //Swap elements
                list[i] = list[randomIndex];
                list[randomIndex] = tmp;
            }
        }
    }
}

public static class RichText
{
    public static string RTBold (this string str)
    {
        return "<b>" + str + "</b>";
    }

    public static string RTBlack (this string str)
    {
        return "<color=black>" + str + "</color>";
    }
}
