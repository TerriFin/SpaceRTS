using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntEnemyBuildingsAll : MonoBehaviour, IScenarioEffect {
    public string ORIGIN_FACTION;

    public void Effect() {
        foreach (Hitpoints ship in ShipsManager.MilShips[ORIGIN_FACTION]) {
            IShipMovement shipMovement = ship.GetComponent<IShipMovement>();
            if (shipMovement.AreWeThereYet()) shipMovement.SetPrimaryTargetPos(BuildingManager.GetFactionRandomBuilding(RelationShipManager.GetRandomOrGoodTargetFaction(ORIGIN_FACTION)).transform.position);
        }
    }
}
