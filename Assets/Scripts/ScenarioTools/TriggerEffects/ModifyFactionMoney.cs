using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyFactionMoney : MonoBehaviour, IScenarioEffect {
    public string FACTION;
    public int AMOUNT;
    public bool SET_MONEY;
    public void Effect() {
        if (FactionManager.Factions.ContainsKey(FACTION)) {
            if (SET_MONEY) FactionManager.Factions[FACTION].money = AMOUNT;
            else FactionManager.Factions[FACTION].ModifyMoney(AMOUNT);
        }
    }
}
