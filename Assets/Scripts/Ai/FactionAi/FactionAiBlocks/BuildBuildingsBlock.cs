using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildBuildingsBlock : FactionBlockWithSkips {
    [System.Serializable]
    public struct BuildingData {
        public FactionBuilding building;
        public bool commandCenter;
        public bool mine;
        public bool defense;
        public int desiredAmount;
        public float maxPercentageOfAllBuildings;
    }

    // Desired amount of resources to production (from -30 to 30)
    public int desiredMineralOverflowAmount;
    public int desiredMoneyOverflowAmount;
    public float CHANCE_TO_BUILD_PRODUCTION_WHEN_NOT_ENOUGH_RESOURCE_FLOW;
    // How much more ahead have to be to relax building (from -500 to 500)
    public int winningThreshold;
    // Percent chance to build while winning
    public float chanceToBuildWhileWinning;
    // Area we check for defensive buildings before building ourselves
    public float defenseScanRadius;
    // Amount of defences orbiting planets
    public int planetDefenseAmount;
    // Amount of defences near mines
    public int asteroidFieldDefenseAmount;
    // How dispersed base should be
    public float chanceToBuildProductionNearMines;
    // How often mines should be build in row
    public int howManySkipsBetweenMines;
    // How often command centers should be build in row
    public int howManySkipsBetweenCommandCenters;
    // How much randomness there is in selecting asteroid field to mine
    public float asteroidFieldSelectionRandomness;
    // Amount of time before preset buildings disabled
    public float presetBuildingTime;
    public float enemyBuildingCheckRadiusInPath;
    public bool START_WITH_MINE;
    public int HOW_MANY_PLANETS_TO_CLAIM_AT_START;
    // List of buildings
    public BuildingData[] buildings;
    

    private FactionAiBase FactionAi;
    private FactionAiFleetManager FactionAiFleetManager;
    private FactionAiBuildingManager AiBuildingManager;
    private LevelBorderManager LevelBorder;
    private int MinesToSkip;
    private int CommandCentersToSkip;

    private BuildingData FirstDefenseStructure;
    private BuildingData Mine;
    private BuildingData CommandCenter;
    private List<BuildingData> Defenses;
    private List<BuildingData> Production;

    private BuildingData Nothing;
    private bool PresetBuildingsActive;
    private bool firstLoop;

    public override void Initialize() {
        FactionAi = GetComponent<FactionAiBase>();
        FactionAiFleetManager = GetComponent<FactionAiFleetManager>();
        AiBuildingManager = GetComponent<FactionAiBuildingManager>();
        LevelBorder = FindObjectOfType<LevelBorderManager>();
        
        MinesToSkip = howManySkipsBetweenMines;
        CommandCentersToSkip = howManySkipsBetweenCommandCenters;
        Defenses = new List<BuildingData>();
        Production = new List<BuildingData>();

        foreach (BuildingData buildingData in buildings) {
            if (buildingData.commandCenter) {
                CommandCenter = buildingData;
            } else if (buildingData.mine) {
                Mine = buildingData;
            } else if (buildingData.defense) {
                Defenses.Add(buildingData);
                if (FirstDefenseStructure.building == null) FirstDefenseStructure = buildingData;
            } else {
                Production.Add(buildingData);
            }
        }

        Nothing = new BuildingData();
        PresetBuildingsActive = true;
        firstLoop = true;

        StartCoroutine(StartPresetBuildingTimer());
    }

    private IEnumerator StartPresetBuildingTimer() {
        yield return new WaitForSeconds(presetBuildingTime);
        PresetBuildingsActive = false;
    }

    public override void Block() {
        BuildingData buildingToBuild = GetDesiredBuilding();

        // First building is mine if not player and variable set to it.
        if (firstLoop && (FactionManager.PlayerFaction == null || !CompareTag(FactionManager.PlayerFaction.factionTag))) {
            if (START_WITH_MINE) TryToBuildMine();
            if (HOW_MANY_PLANETS_TO_CLAIM_AT_START > 0) {
                List<PlanetCaptureLogic> planetsInOrder = PlanetManager.GetPlanetsFromFactionSortedByDistanceToLocation("Untagged", FactionAi.FactionCenterPoint);
                int claimablePlanets = planetsInOrder.Count / FactionManager.Factions.Count;

                for (int i = 0; i < HOW_MANY_PLANETS_TO_CLAIM_AT_START && i < claimablePlanets; i++) {
                    AiBuildingManager.BuildBuilding(FirstDefenseStructure.building, (Vector2)planetsInOrder[i].transform.position + GetRandomCirclePos() * Random.Range(0f, 2f), enemyBuildingCheckRadiusInPath);
                }
            }
        }

        if (firstLoop) {
            firstLoop = false;
            return;
        }

        if (buildingToBuild.building == null) {
            buildingToBuild = GetBuildingToBuild();
        }
        if (buildingToBuild.building != null) {
            if (buildingToBuild.maxPercentageOfAllBuildings == 0 ||  (float) BuildingManager.BuildingAmountsByFactionAndType[tag][buildingToBuild.building.building.GetComponent<Selectable>().selectableType.ToString()].Count / (float) BuildingManager.Buildings[tag].Count < buildingToBuild.maxPercentageOfAllBuildings) {
                if (buildingToBuild.building.type == FactionBuilding.Types.mine) {
                    TryToBuildMine();
                } else if (buildingToBuild.building.type == FactionBuilding.Types.defense) {
                    TryToBuildDefences(buildingToBuild);
                } else if (buildingToBuild.building.type == FactionBuilding.Types.production) {
                    TryToBuildProduction(buildingToBuild);
                } else if (buildingToBuild.building.type == FactionBuilding.Types.command) {
                    TryToBuildCommandCenter();
                }
            }
        }
    }

    private BuildingData GetDesiredBuilding() {
        if (PresetBuildingsActive) {
            buildings.Shuffle();
            foreach (BuildingData building in buildings) {
                if (FactionManager.Factions[tag].money > building.building.moneyCost &&
                    BuildingManager.BuildingAmountsByFactionAndType[tag][building.building.building.GetComponent<Selectable>().selectableType.ToString()].Count < building.desiredAmount) {
                    return building;
                }
            }
        }

        return Nothing;
    }

    private BuildingData GetBuildingToBuild() {
        int factionScoreComparedToOthers = FactionManager.FactionScoresManager.GetActiveFactionAssetScoreComparedToOthers(tag);
        int factionMineralResources = FactionManager.Factions[tag].GetFactionMineralResourcesAmount();
        int factionMoneyResources = FactionManager.Factions[tag].GetFactionMoneyResourcesAmount();
        int freeMineSpots = AsteroidFieldManager.HowManyFreeMineSpots();

        // print(tag + " MINERAL SCORE: " + factionMineralResources + ", MONEY SCORE: " + factionMoneyResources);

        // If we are not winning..
        if (factionScoreComparedToOthers - winningThreshold < 0) {
            return DecideRequiredBuildingType(factionMineralResources, factionMoneyResources, freeMineSpots);
        // If we are winning, do nothing or build stuff
        } else {
            if (Random.value < chanceToBuildWhileWinning) {
                if (Random.value < 0.5f) {
                    return Defenses[Random.Range(0, Defenses.Count)];
                } else {
                    return DecideRequiredBuildingType(factionMineralResources, factionMoneyResources, freeMineSpots);
                }
            }
        }

        return Nothing;
    }

    private BuildingData DecideRequiredBuildingType(int factionMineralResources, int factionMoneyResources, int freeMineSpots) {
        if (factionMineralResources < desiredMineralOverflowAmount) {
            if (freeMineSpots > 0) {
                if (MinesToSkip <= 0) {
                    MinesToSkip = howManySkipsBetweenMines;
                    return Mine;
                } else {
                    MinesToSkip--;
                }
            }
        }

        if (factionMoneyResources < desiredMoneyOverflowAmount) {
            if (CommandCentersToSkip <= 0) {
                if (BuildingManager.GetBuildingTotalAmountByType(Selectable.Types.commandCenter) < LevelBorder.CurrentSize * 0.33f) {
                    CommandCentersToSkip = howManySkipsBetweenCommandCenters;
                    return CommandCenter;
                }
            } else {
                CommandCentersToSkip--;
            }
        }
        
        if ((factionMineralResources >= desiredMineralOverflowAmount && factionMoneyResources >= desiredMoneyOverflowAmount) || Random.Range(0.0f, 1.0f) < CHANCE_TO_BUILD_PRODUCTION_WHEN_NOT_ENOUGH_RESOURCE_FLOW * BuildingManager.BuildingsMineralStorageFillPercentage(tag) * FactionManager.Factions[tag].FactionMoneyStorageFillPercentage()) {
            return Production[Random.Range(0, Production.Count)];
        } else {
            return Defenses[Random.Range(0, Defenses.Count)];
        }
    }

    private void TryToBuildMine() {
        AsteroidField currentAsteroidField = null;
        if (AsteroidFieldManager.AsteroidFields.Count > 0) {
            float currentDistance = float.MaxValue;

            foreach (AsteroidField field in AsteroidFieldManager.AsteroidFields) {
                if (field.RoomForMine() && !EnemyInArea(field.transform.position)) {
                    float distanceToField = Vector2.Distance(FactionAi.FactionCenterPoint, field.transform.position) + Random.Range(0.0f, asteroidFieldSelectionRandomness);
                    if (currentDistance > distanceToField) {
                        currentAsteroidField = field;
                        currentDistance = distanceToField;
                    }
                }
            }
        }

        if (currentAsteroidField != null) {
            if (AiBuildingManager.BuildBuilding(Mine.building, (Vector2)currentAsteroidField.transform.position + GetRandomCirclePos() * Random.Range(0f, currentAsteroidField.FIELD_RADIUS), enemyBuildingCheckRadiusInPath, currentAsteroidField)) {
                currentAsteroidField.MinesOnTheWay++;
                if (currentAsteroidField.NoBuildingsInField()) {
                    AiBuildingManager.BuildBuilding(FirstDefenseStructure.building, (Vector2)currentAsteroidField.transform.position + GetRandomCirclePos() * Random.Range(0f, currentAsteroidField.FIELD_RADIUS), enemyBuildingCheckRadiusInPath);
                    if (FactionAiFleetManager != null) {
                        Fleet randomDefenceFleet = FactionAiFleetManager.GetRandomDefenceFleet();
                        if (randomDefenceFleet != null) randomDefenceFleet.MoveOrder(currentAsteroidField.transform.position, tag, true);
                    }
                }
            }
        }
    }

    private void TryToBuildDefences(BuildingData buildingData) {
        float randomNumber = Random.Range(0f, 1f);
        List<PlanetCaptureLogic> unclaimedPlanets = PlanetManager.GetPlanetsFromFactionSortedByDistanceToLocation("Untagged", FactionAi.FactionCenterPoint);
        if (unclaimedPlanets.Count > 0) {
            AiBuildingManager.BuildBuilding(buildingData.building, unclaimedPlanets[0].transform.position, enemyBuildingCheckRadiusInPath);
        } else {
            if (randomNumber <= 0.40f && PlanetManager.FactionPlanets[tag].Count > 0) {
                // Planet
                PlanetCaptureLogic planet = PlanetManager.GetFactionRandomPlanet(tag);
                if (planet != null && !EnoughDefencesInArea(planet.transform.position, planetDefenseAmount)) {
                    AiBuildingManager.BuildBuilding(buildingData.building, planet.transform.position, enemyBuildingCheckRadiusInPath);
                }
            } else if (randomNumber <= 0.60f && BuildingManager.BuildingAmountsByFactionAndType[tag][Selectable.Types.mine.ToString()].Count > 0) {
                // Mine
                Selectable mine = BuildingManager.GetFactionRandomBuildingByType(tag, Selectable.Types.mine);
                if (mine != null && !EnoughDefencesInArea(mine.transform.position, asteroidFieldDefenseAmount)) {
                    AiBuildingManager.BuildBuilding(buildingData.building, mine.transform.position, enemyBuildingCheckRadiusInPath);
                }
            } else {
                // Other
                Vector2 alertLocation = FactionAi.GetLocationThatCalledHelpAndRemoveIt();
                if (alertLocation != Vector2.zero && !EnoughDefencesInArea(alertLocation, 3)) {
                    AiBuildingManager.BuildBuilding(buildingData.building, alertLocation, enemyBuildingCheckRadiusInPath);
                }
            }
        }
    }

    private void TryToBuildProduction(BuildingData buildingData) {
        if (chanceToBuildProductionNearMines > Random.Range(0.0f, 1.0f)) {
            Selectable mine = BuildingManager.GetFactionRandomBuildingByType(tag, Selectable.Types.mine);
            if (mine != null) {
                AiBuildingManager.BuildBuilding(buildingData.building, (Vector2) mine.transform.position + GetRandomCirclePos() * 3, enemyBuildingCheckRadiusInPath);
            }
        } else {
            Selectable commandCenter = BuildingManager.GetFactionRandomBuildingByType(tag, Selectable.Types.commandCenter);
            if (commandCenter != null) {
                AiBuildingManager.BuildBuilding(buildingData.building, (Vector2) commandCenter.transform.position + GetRandomCirclePos() * 3, enemyBuildingCheckRadiusInPath);
            }
        }
    }

    private void TryToBuildCommandCenter() {
        PlanetCaptureLogic planet = PlanetManager.GetFactionRandomPlanet(tag);
        if (planet != null && !EnoughDefencesInArea(planet.transform.position, planetDefenseAmount)) {
            AiBuildingManager.BuildBuilding(CommandCenter.building, (Vector2)planet.transform.position + (Vector2)(-planet.transform.position).normalized * (Random.value * 4) + GetRandomCirclePos() * 3, enemyBuildingCheckRadiusInPath);
        } else {
            float distanceFromFaction = Random.Range(0.5f, 3.0f);
            float startingDirection = Random.Range(0, Mathf.PI * 2);
            int sphereCheckPoints = 16;
            float sphereCheckPointDistance = (Mathf.PI * 2) / sphereCheckPoints;
            while (distanceFromFaction < LevelBorder.CurrentSize) {
                for (int i = 1; i <= sphereCheckPoints; i++) {
                    bool foundBuildings = false;
                    float currentPoint = startingDirection + sphereCheckPointDistance * i;
                    Vector2 currentPointInSpace = FactionAi.FactionCenterPoint + new Vector2(Mathf.Sin(currentPoint), Mathf.Cos(currentPoint)) * distanceFromFaction;
                    if (LevelBorder.LocationInsideBuildArea(currentPointInSpace)) {
                        Collider2D[] colliders = Physics2D.OverlapCircleAll(currentPointInSpace, CommandCenter.building.buildingRadius * 1.2f);
                        foreach (Collider2D collider in colliders) {
                            if (collider.gameObject.layer == LayerMask.NameToLayer("Building")) {
                                foundBuildings = true;
                                break;
                            }
                        }

                        if (!foundBuildings) {
                            if (AiBuildingManager.BuildBuilding(CommandCenter.building, currentPointInSpace, enemyBuildingCheckRadiusInPath)) return;
                        }
                    }
                }

                distanceFromFaction += Random.Range(0.5f, 3.0f);
            }
        }
    }

    private bool EnoughDefencesInArea(Vector2 location, int amount) {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(location, defenseScanRadius);
        int foundAmount = 0;
        foreach (Collider2D collider in colliders) {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Building") && collider.GetComponent<Hitpoints>().armed) {
                foundAmount++;
                if (foundAmount >= amount) return true;
            }
        }

        return false;
    }

    private bool EnemyInArea(Vector2 location) {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(location, defenseScanRadius);
        foreach (Collider2D collider in colliders) {
            if (RelationShipManager.AreFactionsInWar(tag, collider.tag) && 
                (collider.gameObject.layer == LayerMask.NameToLayer("Building") || (collider.gameObject.layer == LayerMask.NameToLayer("Ship") && collider.GetComponent<Hitpoints>().armed))) {
                return true;
            }
        }

        return false;
    }

    private Vector2 GetRandomCirclePos() {
        float randomNumber = Random.Range(0, Mathf.PI * 2);
        return new Vector2(Mathf.Sin(randomNumber), Mathf.Cos(randomNumber)) * Random.Range(0.1f, 1f);
    }
}
