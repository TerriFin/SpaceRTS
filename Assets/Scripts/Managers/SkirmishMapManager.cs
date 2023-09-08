using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkirmishMapManager : MonoBehaviour {
    public GameObject FACTION_MANAGER;
    public Camera MAIN_CAMERA;
    public int DEFAULT_STARTING_MINERALS;
    public int DEFAULT_STARTING_MONEY;

    public List<string> FACTION_TEST_TAGS;
    public List<int> FACTION_TEST_DIFFICULTIES;
    public List<int> FACTION_TEST_STARTING_RESOURCES;

    private MapGeneratorManager MapGenerator;
  
    public void InitializePlayers() {
        if (MapGenerator == null) MapGenerator = FindObjectOfType<MapGeneratorManager>();
        if (MapGenerator.FactionDatas == null) MapGenerator.InitializeWithTestData(FACTION_TEST_TAGS, FACTION_TEST_DIFFICULTIES, FACTION_TEST_STARTING_RESOURCES);

        // Set player faction and Ai:s
        FactionAiBase[] foundFactionAiBases = FACTION_MANAGER.GetComponentsInChildren<FactionAiBase>();
        Faction[] foundFactions = FACTION_MANAGER.GetComponents<Faction>();
        foreach (Faction currentFoundFaction in foundFactions) {
            foreach (string currentGivenFaction in MapGenerator.FactionDatas.Keys) {
                if (currentGivenFaction == currentFoundFaction.factionTag) {
                    if (MapGenerator.FactionDatas[currentGivenFaction].statusIndex == 1) {
                        currentFoundFaction.playerFaction = true;
                        FactionManager.PlayerFaction = currentFoundFaction;
                        break;
                    } else {
                        currentFoundFaction.playerFaction = false;
                        if (MapGenerator.FactionDatas[currentGivenFaction].statusIndex != 0) {
                            foreach (FactionAiBase currentFoundFactionAiBase in foundFactionAiBases) {
                                if (currentGivenFaction == currentFoundFactionAiBase.tag) {
                                    currentFoundFactionAiBase.ON = true;
                                    CargoShipAmountManagerBlock aiCargoManager = currentFoundFactionAiBase.GetComponent<CargoShipAmountManagerBlock>();
                                    BuildBuildingsBlock aiBuildingManager = currentFoundFactionAiBase.GetComponent<BuildBuildingsBlock>();
                                    switch (MapGenerator.FactionDatas[currentGivenFaction].statusIndex) {
                                        case 2:
                                            currentFoundFactionAiBase.aiDecisionTimer = 1.2f;
                                            currentFoundFaction.aiBonusMultiplier = 0.8f;
                                            if (aiCargoManager != null) aiCargoManager.MINE_CARGO_POINTS -= 4;
                                            if (aiBuildingManager != null) aiBuildingManager.CHANCE_TO_BUILD_PRODUCTION_WHEN_NOT_ENOUGH_RESOURCE_FLOW -= 0.15f;
                                            break;
                                        case 3:
                                            currentFoundFactionAiBase.aiDecisionTimer = 1.0f;
                                            currentFoundFaction.aiBonusMultiplier = 1.0f;
                                            if (aiCargoManager != null) aiCargoManager.MINE_CARGO_POINTS += 0;
                                            if (aiBuildingManager != null) aiBuildingManager.CHANCE_TO_BUILD_PRODUCTION_WHEN_NOT_ENOUGH_RESOURCE_FLOW += 0.0f;
                                            break;
                                        case 4:
                                            currentFoundFactionAiBase.aiDecisionTimer = 0.8f;
                                            currentFoundFaction.aiBonusMultiplier = 1.25f;
                                            if (aiCargoManager != null) aiCargoManager.MINE_CARGO_POINTS += 7;
                                            if (aiBuildingManager != null) aiBuildingManager.CHANCE_TO_BUILD_PRODUCTION_WHEN_NOT_ENOUGH_RESOURCE_FLOW += 0.15f;
                                            break;
                                        case 5:
                                            currentFoundFactionAiBase.aiDecisionTimer = 0.7f;
                                            currentFoundFaction.aiBonusMultiplier = 1.5f;
                                            if (aiCargoManager != null) aiCargoManager.MINE_CARGO_POINTS += 14;
                                            if (aiBuildingManager != null) aiBuildingManager.CHANCE_TO_BUILD_PRODUCTION_WHEN_NOT_ENOUGH_RESOURCE_FLOW += 0.2f;
                                            break;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void InitializeSkirmishMap() {
        if (MapGenerator == null) MapGenerator = FindObjectOfType<MapGeneratorManager>();
        MapGenerator.GenerateMap();

        if (FactionManager.PlayerFaction != null) {
            foreach (CommandCenterCargoProduction cc in FindObjectsOfType<CommandCenterCargoProduction>()) {
                if (cc.CompareTag(FactionManager.PlayerFaction.factionTag)) {
                    Vector3 cameraPosition = cc.transform.position;
                    cameraPosition.z = -10;
                    MAIN_CAMERA.transform.position = cameraPosition;
                    break;
                }
            }
        }

        // Set starting resources and total war timer
        foreach (string currentGivenFaction in MapGenerator.FactionDatas.Keys) {
            if (MapGenerator.FactionDatas[currentGivenFaction].statusIndex != 0) {
                FactionManager.Factions[currentGivenFaction].money = DEFAULT_STARTING_MONEY * MapGenerator.FactionDatas[currentGivenFaction].startingResourcesIndex;
                Selectable[] selectables = FindObjectsOfType<Selectable>();
                foreach (Selectable selectable in selectables) {
                    if (selectable.selectableType == Selectable.Types.commandCenter && currentGivenFaction == selectable.tag) {
                        selectable.GetComponent<MineralStorage>().currentMineralStorage = (DEFAULT_STARTING_MINERALS * MapGenerator.FactionDatas[currentGivenFaction].startingResourcesIndex);
                    }
                }

                WarManagerBlock aiWarBlock = FactionManager.Factions[currentGivenFaction].ai.GetComponent<WarManagerBlock>();
                if (aiWarBlock != null) aiWarBlock.TOTAL_WAR_TIME = MapGenerator.TOTAL_WAR_TIMER;
            }
        }
    }
}
