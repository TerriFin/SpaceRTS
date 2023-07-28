using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChangeRelationshipStatus : MonoBehaviour, IScenarioEffect {
    public string FACTION1;
    public string FACTION2;
    public bool DECLARE_WAR;
    public bool DECLARE_PEACE;
    public bool DECLARE_BLOCKADE;
    public bool END_BLOCKADE;
    public void Effect() {
        if (DECLARE_WAR) RelationShipManager.StartWar(FACTION1, FACTION2, true);
        if (DECLARE_PEACE) RelationShipManager.EndWar(FACTION1, FACTION2, true);
        if (DECLARE_BLOCKADE) RelationShipManager.StartBlockade(FACTION1, FACTION2, true);
        if (END_BLOCKADE) RelationShipManager.EndBlockade(FACTION1, FACTION2, true);
    }
}
