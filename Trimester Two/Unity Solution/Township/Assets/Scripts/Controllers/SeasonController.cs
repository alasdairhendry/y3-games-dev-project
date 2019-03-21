using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SeasonController {

    public enum Season { Spring, Summer, Autumn, Winter }
    public static Season CurrentSeason { get; private set; } = Season.Winter;

    //private static List<List<int>> seasons = new List<List<int>> () { new List<int> () { 3, 4, 5 }, new List<int> () { 6, 7, 8 }, new List<int> () { 9, 10, 11 }, new List<int> () { 12, 1, 2 } };

    private static Dictionary<Season, List<int>> seasons = new Dictionary<Season, List<int>> () {
    { Season.Spring, new List<int> () { 3, 4, 5 } },
    { Season.Summer, new List<int> () { 6, 7, 8 } },
    { Season.Autumn, new List<int> () { 9, 10, 11 } },
    { Season.Winter, new List<int> () { 12, 1, 2 } }
    };

    //private void Awake ()
    //{
    //    GameTime.onMonthChanged += UpdateSeason;

    //    UpdateSeason ( ((GameTime.currentMonth - 1) + 12) % 12, GameTime.currentMonth );

    //    seasons = new List<List<int>> ();
    //    seasons.Add ( new List<int> () { 3, 4, 5 } );
    //    seasons.Add ( new List<int> () { 6, 7, 8 } );
    //    seasons.Add ( new List<int> () { 9, 10, 11 } );
    //    seasons.Add ( new List<int> () { 12, 1, 2 } );
    //}

    //private void Start () {

    //}

    public static List<int> GetSeasonData (Season season)
    {
        return seasons[season];
    }


    public static void UpdateSeason(int previousMonth, int currentMonth)
    {
        foreach (KeyValuePair<Season, List<int>> season in seasons)
        {
            if (season.Value.Contains ( currentMonth ))
            {
                CurrentSeason = season.Key;
                return;
            }
        }
    }
}
