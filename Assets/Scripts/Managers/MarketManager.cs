using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketManager : MonoBehaviour {

    public static List<Market> buyingMarkets;
    public static List<Market> sellingMarkets;

    public static void Reset() {
        buyingMarkets = new List<Market>();
        sellingMarkets = new List<Market>();
    }
}
