using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Production : MonoBehaviour {

    public GameObject producedShip;
    public int moneyCost;
    public int metalCost;
    public float productionTime;
    public int productionLimit;
    public bool on;
    public GameObject productionBar;

    public int CurrentShips { get; private set; }
    public int FactionMineralValue { get; private set; }
    public int FactionMoneyValue { get; private set; }

    private MineralStorage Storage;
    private ProductionWaypoint Waypoint;
    private bool Producing;

    private void Start() {
        Storage = GetComponent<MineralStorage>();
        Waypoint = GetComponent<ProductionWaypoint>();
        CurrentShips = 0;

        // Apply bonus to difficult AI
        float aiDifficultyModifier = FactionManager.Factions[tag].aiBonusMultiplier - 1;
        if (aiDifficultyModifier != 0) {
            productionTime -= productionTime * aiDifficultyModifier;
        }

        FactionMineralValue = (int) -(metalCost / productionTime);
        FactionMoneyValue = (int) -((moneyCost / productionTime) * 0.5f);

        Producing = false;
    }

    private void Update() {
        if (on && FactionManager.Factions[tag].money >= moneyCost && Storage.currentMineralStorage >= metalCost && CurrentShips < productionLimit && !Producing) {
            FactionManager.Factions[tag].money -= moneyCost;
            Storage.currentMineralStorage -= metalCost;
            Storage.ShowOrExtendStorageBar();
            Producing = true;
            CurrentShips++;
            StartCoroutine(StartProduction());
        }
    }

    public void ShipDestroyed() {
        CurrentShips--;
    }

    public bool EnoughMineralsToProduce() {
        return Storage.currentMineralStorage >= metalCost;
    }

    private IEnumerator StartProduction() {
        ProductionBar bar = Instantiate(productionBar).GetComponent<ProductionBar>();
        bar.productionTime = productionTime;
        bar.origin = transform;

        yield return new WaitForSeconds(productionTime);

        GameObject ship = Instantiate(producedShip);
        ship.tag = tag;
        ship.transform.position = transform.position;
        ship.GetComponent<IShipMovement>().SetOrigin(gameObject);

        if (Waypoint != null && Waypoint.WaypointSet) {
            ship.GetComponent<AiBase>().SetAiActive(false);
            ship.GetComponent<IShipMovement>().SetPrimaryTargetPos(Waypoint.CurrentWaypoint);
        }

        Producing = false;
    }
}
