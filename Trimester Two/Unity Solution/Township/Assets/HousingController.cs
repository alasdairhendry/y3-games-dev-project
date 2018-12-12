using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HousingController {

    public static System.Action<Prop_House> onCitizenLeavesHouse;
    public static System.Action<Prop_House> onHouseBecomesFull;
    public static System.Action<Prop_House> onHouseBecomesEmpty;
    public static System.Action<Prop_House> onHouseGetsNewFamily;

    public static void OnCitizenLeavesHouse (Prop_House house)
    {
        if (onCitizenLeavesHouse != null) onCitizenLeavesHouse ( house );
    }

    public static void OnHouseBecomesFull (Prop_House house)
    {
        if (onHouseBecomesFull != null) onHouseBecomesFull ( house );
    }

    public static void OnHouseBecomesEmpty (Prop_House house)
    {
        if (onHouseBecomesEmpty != null) onHouseBecomesEmpty ( house );
    }

    public static void OnHouseGetsNewFamily (Prop_House house)
    {
        if (onHouseGetsNewFamily != null) onHouseGetsNewFamily ( house );
    }

}
