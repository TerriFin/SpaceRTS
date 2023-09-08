using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FactionBuilding", menuName = "ScriptableObjects/FactionBuilding")]
public class FactionBuilding : ScriptableObject {

    public enum Types {
        command,
        mine,
        production,
        defense,
    }

    public string faction;

    public GameObject building;
    public Types type;
    public float buildingRadius;
    public float buildTime;
    public int mineralCost;
    public int moneyCost;
    public string requiredPrefString;

}
