using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntEnemyShips : MonoBehaviour, IScenarioEffect {
    public string ORIGIN_FACTION;
    public string TARGET_FACTION;

    public void Effect() {
        foreach (Hitpoints ship in ShipsManager.MilShips[ORIGIN_FACTION]) {
            IShipMovement shipMovement = ship.GetComponent<IShipMovement>();
            if (shipMovement.AreWeThereYet()) shipMovement.SetPrimaryTargetPos(ShipsManager.GetFactionRandomMilShip(TARGET_FACTION).transform.position);
        }
    }
}
