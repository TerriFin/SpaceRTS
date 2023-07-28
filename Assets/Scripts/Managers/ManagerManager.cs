using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerManager : MonoBehaviour {
    private void Awake() {
        SkirmishMapManager skirmishManager = FindObjectOfType<SkirmishMapManager>();
        if (skirmishManager != null) skirmishManager.InitializePlayers();

        FactionManager.Reset();
        AsteroidFieldManager.Reset();
        BuildingManager.Reset();
        BuildingPlacementManager.Reset();
        CargoShipManager.Reset();
        MarketManager.Reset();
        PlanetManager.Reset();
        SelectionManager.Reset();
        ShipsManager.Reset();
        FactionOpinionManager.Reset();
        GlobalMessageManager.Reset();

        if (skirmishManager != null) skirmishManager.InitializeSkirmishMap();
    }
}
