﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Richness { Sparse, Abundant, Plentiful }
public enum PropCategory { Housing, Paths, Food, Entertainment, Production, Storage, Misc, Gathering }
public enum PlacementType { Plopable, Draggable }
public enum PlacementArea { Ground, Waterside, Water }
public enum ProfessionType { None, Student, Worker, Lumberjack, Quarryman, Stonemason, Fisherman, Winemaker, Vintner, Charcoal_Burner, Miner, Blacksmith }

public static class EnumCollection {

    public static Dictionary<string, Richness> richness = new Dictionary<string, Richness> ()
    {
        { Richness.Sparse.ToString(), Richness.Sparse },
        { Richness.Abundant.ToString(), Richness.Abundant },
        { Richness.Plentiful.ToString(), Richness.Plentiful }
    };

    public static Dictionary<string, PropCategory> propCategory = new Dictionary<string, PropCategory> ()
    {
        { PropCategory.Housing.ToString(), PropCategory.Housing },
        { PropCategory.Paths.ToString(), PropCategory.Paths },
        { PropCategory.Food.ToString(), PropCategory.Food },
        { PropCategory.Entertainment.ToString(), PropCategory.Entertainment },
        { PropCategory.Production.ToString(), PropCategory.Production },
        { PropCategory.Storage.ToString(), PropCategory.Storage }
    };
}
