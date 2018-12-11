using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeasonController : MonoBehaviour {

    public enum Season { Spring, Summer, Autumn, Winter }

    private static Season currentSeason = Season.Winter;
    public static Season CurrentSeason { get { return currentSeason; } }

    private static List<List<int>> seasons = new List<List<int>> ();

    private void Awake ()
    {
        GameTime.onMonthChanged += UpdateSeason;

        UpdateSeason ( ((GameTime.currentMonth - 1) + 12) % 12, GameTime.currentMonth );

        seasons = new List<List<int>> ();
        seasons.Add ( new List<int> () { 3, 4, 5 } );
        seasons.Add ( new List<int> () { 6, 7, 8 } );
        seasons.Add ( new List<int> () { 9, 10, 11 } );
        seasons.Add ( new List<int> () { 12, 1, 2 } );
    }

    private void Start () {
        
    }

    public static void UpdateSeason(int previousMonth, int currentMonth)
    {
        foreach (List<int> item in seasons)
        {
            if (item.Contains ( currentMonth ))
            {
                currentSeason = (Season)seasons.IndexOf ( item );
                return;
            }
        }
    }
}
