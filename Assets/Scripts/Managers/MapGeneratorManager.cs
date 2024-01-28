using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public class MapGeneratorManager : MonoBehaviour {
    [System.Serializable]
    public struct FactionData {
        public int statusIndex;
        public int startingResourcesIndex;
        public int startingShipsIndex;
        public int startingExpansionIndex;

        public FactionData(int statusIndex, int startingResourcesIndex, int startingShipsIndex, int startingExpansionIndex) {
            this.statusIndex = statusIndex;
            this.startingResourcesIndex = startingResourcesIndex;
            this.startingShipsIndex = startingShipsIndex;
            this.startingExpansionIndex = startingExpansionIndex;
        }
    }

    public GameObject SMALL_ASTEROID_FIELD;
    public GameObject MEDIUM_ASTEROID_FIELD;
    public GameObject LARGE_ASTEROID_FIELD;
    public List<GameObject> PLANETS;
    public List<GameObject> SUNS;

    public GameObject FederationPolice;
    public GameObject FederationRaider;
    public GameObject FederationFrigate;
    public GameObject FederationTrader;
    public GameObject EmpirePolice;
    public GameObject EmpireRaider;
    public GameObject EmpireFrigate;
    public GameObject EmpireTrader;
    public GameObject PiratePolice;
    public GameObject PirateRaider;
    public GameObject PirateFrigate;
    public GameObject PirateTrader;

    public GameObject FederationCommandCenter;
    public GameObject FederationMine;
    public GameObject FederationDefenceStation;
    public GameObject FederationFrigateStation;
    public GameObject FederationPatrolStation;
    public GameObject EmpireCommandCenter;
    public GameObject EmpireMine;
    public GameObject EmpireDefenceStation;
    public GameObject EmpireFrigateStation;
    public GameObject EmpirePatrolStation;
    public GameObject PirateCommandCenter;
    public GameObject PirateMine;
    public GameObject PirateDefenceStation;
    public GameObject PirateFrigateStation;
    public GameObject PiratePatrolStation;

    public float MAP_SIZE;
    public float CENTER_DEAD_ZONE;
    public float CENTER_ASTEROID_FIELD_CHANCE;
    public float CENTER_PLANET_CHANCE;
    public int DESIRED_ASTEROID_FIELDS_AMOUNT;
    public int DESIRED_PLANET_FACTION_SCORE;
    public float TOTAL_WAR_TIMER;
    public int PLANET_PLACEMENT_TRIES;
    public float SMALL_ASTEROID_FIELD_CHANCE;
    public float MEDIUM_ASTEROID_FIELD_CHANCE;
    public bool DEVELOPMENT;

    public int MAP_SIZE_LAST_INDEX;
    public int MAP_ASTEROIDS_LAST_INDEX;
    public int MAP_PLANETS_LAST_INDEX;
    public int TOTAL_WAR_TIMER_LAST_INDEX;
    public int MIRRORED_SIDES_INDEX;

    public string WHERE_TO_RETURN_IN_MENU;

    public Dictionary<string, FactionData> FactionDatas;

    private static MapGeneratorManager Instance;

    private void Awake() {
        if (!DEVELOPMENT) {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            } else {
                DontDestroyOnLoad(transform.gameObject);

                FactionDatas = new Dictionary<string, FactionData>();
                FactionDatas.Add("Federation", new FactionData(1, 1, 2, 1));
                FactionDatas.Add("Empire", new FactionData(3, 1, 2, 1));
                FactionDatas.Add("Pirate", new FactionData(3, 1, 2, 1));

                MAP_SIZE_LAST_INDEX = 2;
                MAP_ASTEROIDS_LAST_INDEX = 2;
                MAP_PLANETS_LAST_INDEX = 2;
                TOTAL_WAR_TIMER_LAST_INDEX = 3;
                MIRRORED_SIDES_INDEX = 0;

                Instance = this;
            }
        } else {
            MapGeneratorManager[] generators = FindObjectsOfType<MapGeneratorManager>();
            foreach (MapGeneratorManager generator in generators) {
                if (!generator.DEVELOPMENT) {
                    Destroy(this.gameObject);
                }
            }
        }
    }

    public void InitializeWithTestData(List<string> factionTags, List<int> factionDifficulties, List<int> factionStartingResources, List<int> factionStartingShips, List<int> factionStartingExpansion) {
        FactionDatas = new Dictionary<string, FactionData>();
        for (int i = 0; i < factionTags.Count; i++) {
            FactionDatas.Add(factionTags[i], new FactionData(factionDifficulties[i], factionStartingResources[i], factionStartingShips[i], factionStartingExpansion[i]));
        }
    }

    public void GenerateMap() {
        FindObjectOfType<LevelBorderManager>().InitializeMap(MAP_SIZE);

        List<AsteroidField> fields = new List<AsteroidField>();
        List<PlanetCaptureLogic> planets = new List<PlanetCaptureLogic>();

        int chosenSun = Random.Range(0, SUNS.Count);
        GameObject.Instantiate(SUNS[chosenSun], GameObject.FindGameObjectWithTag("MainCamera").transform);

        AsteroidField centerField = PlaceCenterAsteroidField();
        PlanetCaptureLogic centerPlanet = PlaceCenterPlanet();

        if (centerField != null) fields.Add(centerField);
        if (centerPlanet != null) planets.Add(centerPlanet);

        float anchor = Random.Range(0, Mathf.PI * 2);
            int currentAsteroidFieldTries = 0;
            while (currentAsteroidFieldTries < DESIRED_ASTEROID_FIELDS_AMOUNT) {
                float randomNumber = Random.Range(0.0f, 1.0f);
            GameObject chosenAsteroidField;
            if (randomNumber < SMALL_ASTEROID_FIELD_CHANCE) {
                    chosenAsteroidField = SMALL_ASTEROID_FIELD;
                } else if (randomNumber < MEDIUM_ASTEROID_FIELD_CHANCE) {
                    chosenAsteroidField = MEDIUM_ASTEROID_FIELD;
                } else {
                    chosenAsteroidField = LARGE_ASTEROID_FIELD;
                }

                int currentTries = 0;
                while (currentTries < 50) {
                    Vector2 randomPointInMap = GetRandomPointInMap();
                    if (CheckAsteroidFieldEnoughSpace(randomPointInMap, fields, chosenAsteroidField.GetComponent<AsteroidField>().FIELD_RADIUS * 5.0f)) {
                        for (int i = 0; i < MIRRORED_SIDES_INDEX + 1; i++) {
                            fields.Add(Instantiate(chosenAsteroidField, Quaternion.Euler(0, 0, 360 / (MIRRORED_SIDES_INDEX + 1) * i) * randomPointInMap, Quaternion.identity).GetComponent<AsteroidField>());
                        }
                        break;
                    }
                    currentTries++;
                }

                currentAsteroidFieldTries += MIRRORED_SIDES_INDEX + 1;
            }

            int currentPlanetFactionScore = 0;
            int failedTries = 0;
            while (currentPlanetFactionScore < DESIRED_PLANET_FACTION_SCORE) {
                GameObject chosenPlanet = PLANETS[Random.Range(0, PLANETS.Count)];
                Vector2 randomPointInMap = GetRandomPointInMap();
                if (CheckPlanetEnoughSpace(randomPointInMap, planets, chosenPlanet.GetComponent<PlanetCaptureLogic>().PLANET_DISTANCE_REQUIREMENT)) {
                    for (int i = 0; i < MIRRORED_SIDES_INDEX + 1; i++) {
                        planets.Add(Instantiate(chosenPlanet, Quaternion.Euler(0, 0, 360 / (MIRRORED_SIDES_INDEX + 1) * i) * randomPointInMap, Quaternion.identity).GetComponent<PlanetCaptureLogic>());
                        planets[planets.Count - 1].SetMoneyGenerator();
                        currentPlanetFactionScore += planets[planets.Count - 1].MoneyGenerator.factionMoneyValue;
                    }
                } else {
                    failedTries++;
                }

                if (failedTries >= PLANET_PLACEMENT_TRIES) break;
            }

            PlaceFactionsOnMap(anchor);
    }

    private AsteroidField PlaceCenterAsteroidField() {
        if (Random.Range(0.0f, 1.0f) < CENTER_ASTEROID_FIELD_CHANCE) {
            float randomNumber = Random.Range(0.0f, 1.0f);
            if (randomNumber < SMALL_ASTEROID_FIELD_CHANCE) {
                return Instantiate(SMALL_ASTEROID_FIELD, Vector2.zero, Quaternion.identity).GetComponent<AsteroidField>();
            } else if (randomNumber < MEDIUM_ASTEROID_FIELD_CHANCE) {
                return Instantiate(MEDIUM_ASTEROID_FIELD, Vector2.zero, Quaternion.identity).GetComponent<AsteroidField>();
            } else {
                return Instantiate(LARGE_ASTEROID_FIELD, Vector2.zero, Quaternion.identity).GetComponent<AsteroidField>();
            }
        }

        return null;
    }

    private PlanetCaptureLogic PlaceCenterPlanet() {
        if (Random.Range(0.0f, 1.0f) < CENTER_PLANET_CHANCE) {
            return Instantiate(PLANETS[Random.Range(0, PLANETS.Count)], Vector2.zero, Quaternion.identity).GetComponent<PlanetCaptureLogic>();
        }

        return null;
    }

    private List<string> PlaceFactionsOnMap(float anchor) {
        List<string> currentFactions = new List<string>();
        string playerFaction = null;
        foreach (string faction in FactionDatas.Keys) {
            FactionData factionData = FactionDatas[faction];
            if (factionData.statusIndex == 1 && playerFaction == null) {
                playerFaction = faction;
                currentFactions.Add(faction);
            } else if (factionData.statusIndex > 1) currentFactions.Add(faction);
        }

        int factionPlacementDivider = currentFactions.Count == 2 ? MIRRORED_SIDES_INDEX > 0 ? MIRRORED_SIDES_INDEX + 1 : 2 : currentFactions.Count;
        float distanceFromCenter = Random.Range(MAP_SIZE * 0.33f, MAP_SIZE * 0.75f);
        for (int i = 0; i < currentFactions.Count; i++) {
            Vector2 factionStartingPosition = new Vector2(Mathf.Sin(anchor + Mathf.PI * 2 / factionPlacementDivider * i), Mathf.Cos(anchor + Mathf.PI * 2 / factionPlacementDivider * i)) * distanceFromCenter;
            if (currentFactions[i] == "Federation") {
                Instantiate(FederationCommandCenter, factionStartingPosition, Quaternion.identity);
                SpawnFactionStartingBuildings("Federation", FactionDatas["Federation"].startingExpansionIndex, factionStartingPosition, FederationMine, FederationDefenceStation, FederationFrigateStation, FederationPatrolStation);
                SpawnFactionStartingShips(FactionDatas["Federation"].startingShipsIndex, factionStartingPosition, FederationPolice, FederationRaider, FederationFrigate, FederationTrader);
            } else if (currentFactions[i] == "Empire") {
                Instantiate(EmpireCommandCenter, factionStartingPosition, Quaternion.identity);
                SpawnFactionStartingBuildings("Empire", FactionDatas["Empire"].startingExpansionIndex, factionStartingPosition, EmpireMine, EmpireDefenceStation, EmpireFrigateStation, EmpirePatrolStation);
                SpawnFactionStartingShips(FactionDatas["Empire"].startingShipsIndex, factionStartingPosition, EmpirePolice, EmpireRaider, EmpireFrigate, EmpireTrader);
            } else if (currentFactions[i] == "Pirate") {
                Instantiate(PirateCommandCenter, factionStartingPosition, Quaternion.identity);
                SpawnFactionStartingBuildings("Pirate", FactionDatas["Pirate"].startingExpansionIndex, factionStartingPosition, PirateMine, PirateDefenceStation, PirateFrigateStation, PiratePatrolStation);
                SpawnFactionStartingShips(FactionDatas["Pirate"].startingShipsIndex, factionStartingPosition, PiratePolice, PirateRaider, PirateFrigate, PirateTrader);
            }
        }

        return currentFactions;
    }

    private void SpawnFactionStartingBuildings(string faction, int startingExpansionIndex, Vector2 startingPosition, GameObject mine, GameObject defenceStation, GameObject frigateStation, GameObject patrolDepot) {
        switch (startingExpansionIndex) {
            case 1:
                SpawnFactionPlanetDefenceStations(faction, startingPosition, 1, defenceStation);
                SpawnFactionMines(faction, startingPosition, 2, mine, defenceStation);
                SpawnFactionProduction(startingPosition, MAP_SIZE / Mathf.PI, 1, frigateStation);
                SpawnFactionProduction(startingPosition, MAP_SIZE / Mathf.PI, 1, patrolDepot);
                break;
            case 2: 
                SpawnFactionPlanetDefenceStations(faction, startingPosition, 3, defenceStation);
                SpawnFactionMines(faction, startingPosition, 5, mine, defenceStation);
                SpawnFactionProduction(startingPosition, MAP_SIZE / Mathf.PI, 2, frigateStation);
                SpawnFactionProduction(startingPosition, MAP_SIZE / Mathf.PI, 2, patrolDepot);
                break;
            case 3:
                SpawnFactionPlanetDefenceStations(faction, startingPosition, 5, defenceStation);
                SpawnFactionMines(faction, startingPosition, 9, mine, defenceStation);
                SpawnFactionProduction(startingPosition, MAP_SIZE / Mathf.PI, 4, frigateStation);
                SpawnFactionProduction(startingPosition, MAP_SIZE / Mathf.PI, 4, patrolDepot);
                break;
        }
    }

    private void SpawnFactionPlanetDefenceStations(string faction, Vector2 factionStartingPoint, int planetsToClaim, GameObject defenceStation) {
        List<PlanetCaptureLogic> planetsInOrder = GetPlanetsSortedByDistanceToPoint(factionStartingPoint);
        int claimablePlanets = (planetsInOrder.Count / 3) + 1;

        int successTries = 0;
        int failTries = 0;
        while (successTries < planetsToClaim && successTries + failTries < claimablePlanets) {
            if (CheckForOtherFactionCommandCenter(planetsInOrder[successTries + failTries].transform.position, 7.0f, faction)) failTries++; 
            else if (TryToPlaceDownBuilding(planetsInOrder[successTries + failTries].transform.position, defenceStation, 2.8f, 4.0f, 10, 0)) successTries++;
            else failTries++;
        }
    }

    private void SpawnFactionMines(string faction, Vector2 factionStartingPoint, int minesToBuild, GameObject mine, GameObject defenceStation) {
        List<AsteroidField> fieldsInOrder = GetAsteroidFieldsSortedByDistanceToPoint(factionStartingPoint);
        int claimableFields = (fieldsInOrder.Count / 3) + 1;

        int minesBuilt = 0;
        int fieldsProcessed = 0;
        while (minesBuilt < minesToBuild && fieldsProcessed < claimableFields) {
            if (CheckForOtherFactionCommandCenter(fieldsInOrder[fieldsProcessed].transform.position, fieldsInOrder[fieldsProcessed].FIELD_RADIUS + 7.0f, faction)) {
                fieldsProcessed++;
                continue;
            }
            int fieldBuiltMines = 0;
            while (fieldBuiltMines < fieldsInOrder[fieldsProcessed].MAX_AI_MINES) {
                if (TryToPlaceDownBuilding(fieldsInOrder[fieldsProcessed].transform.position, mine, fieldsInOrder[fieldsProcessed].FIELD_RADIUS, 1.5f, 50, 325)) {
                    TryToPlaceDownBuilding(fieldsInOrder[fieldsProcessed].transform.position, defenceStation, fieldsInOrder[fieldsProcessed].FIELD_RADIUS + 3.0f, 3.75f, 50, 0);
                    minesBuilt++;
                }

                fieldBuiltMines++;
            }

            fieldsProcessed++;
        }
    }

    private void SpawnFactionProduction(Vector2 factionStartingPoint, float radius, int toBuild, GameObject productionBuilding) {
        for (int i = 0; i < toBuild; i++) {
            TryToPlaceDownBuilding(factionStartingPoint, productionBuilding, radius, 3.5f, 50, 0);
        }
    }

    private void SpawnFactionStartingShips(int startingShipsIndex, Vector2 location, GameObject policeShip, GameObject raiderShip, GameObject frigateShip, GameObject traderShip) {
        switch (startingShipsIndex) {
            case 1:
                Instantiate(raiderShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                break;
            case 2:
                Instantiate(policeShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(raiderShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(raiderShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(traderShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(traderShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                break;
            case 3:
                Instantiate(policeShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(policeShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(raiderShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(raiderShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(raiderShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(frigateShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(traderShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(traderShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(traderShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                break;
            case 4:
                Instantiate(policeShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(policeShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(policeShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(policeShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(raiderShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(raiderShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(raiderShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(raiderShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(raiderShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(frigateShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(frigateShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(frigateShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(traderShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(traderShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(traderShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(traderShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(traderShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                break;
            case 5:
                Instantiate(raiderShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(raiderShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(raiderShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(raiderShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(raiderShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(frigateShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(frigateShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(frigateShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(frigateShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(frigateShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(frigateShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(frigateShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(frigateShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(frigateShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(traderShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(traderShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(traderShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(traderShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                Instantiate(traderShip, location + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
                break;

        }
    }

    private Vector2 GetRandomPointInMap() {
        float randomNumber = Random.Range(0, Mathf.PI * 2);
        return new Vector2(Mathf.Sin(randomNumber), Mathf.Cos(randomNumber)) * Random.Range(MAP_SIZE * CENTER_DEAD_ZONE * (MIRRORED_SIDES_INDEX <= 2 ? 1.0f : MIRRORED_SIDES_INDEX <= 4 ? 1.5f : 1.75f), MAP_SIZE * 0.9f);
    }

    private bool CheckAsteroidFieldEnoughSpace(Vector2 location, List<AsteroidField> fields, float distance) {
        foreach (AsteroidField field in fields) {
            if (Vector2.Distance(location, field.transform.position) < distance) return false;
        }

        return true;
    }

    private bool CheckPlanetEnoughSpace(Vector2 location, List<PlanetCaptureLogic> planets, float distance) {
        foreach (PlanetCaptureLogic planet in planets) {
            if (Vector2.Distance(location, planet.transform.position) < distance) return false;
        }

        return true;
    }

    private bool CheckForOtherFactionCommandCenter(Vector2 position, float radius, string faction) {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, radius, 1 << LayerMask.NameToLayer("Building"));
        foreach (Collider2D collider in colliders) {
            if (collider.GetComponent<CommandCenterBuildingShipSpawner>() != null && collider.gameObject.tag != faction) return true;
        }

        return false;
    }

    private bool TryToPlaceDownBuilding(Vector2 position, GameObject building, float maxRadius, float checkRadius, int tries, int startingMinerals) {
        for (int i = 0; i < tries; i++) {
            float randomNumber = Random.Range(0, Mathf.PI * 2);
            Vector2 checkSpot = position + new Vector2(Mathf.Sin(randomNumber), Mathf.Cos(randomNumber)) * Random.Range(0.0f, maxRadius);
            if (Vector2.Distance(Vector2.zero, checkSpot) > MAP_SIZE) continue;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(checkSpot, checkRadius, 1 << LayerMask.NameToLayer("Building"));
            if (colliders.Length == 0) {
                GameObject builtBuilding = Instantiate(building, checkSpot, Quaternion.identity);
                if (startingMinerals > 0) builtBuilding.GetComponent<MineralStorage>().currentMineralStorage = startingMinerals;
                return true;
            }
        }

        return false;
    }

    private List<PlanetCaptureLogic> GetPlanetsSortedByDistanceToPoint(Vector2 position) {
        List<PlanetCaptureLogic> planets = FindObjectsOfType<PlanetCaptureLogic>().ToList();
        planets.Sort(delegate(PlanetCaptureLogic x, PlanetCaptureLogic y) {
            return (int)(Vector2.Distance(position, x.transform.position) - Vector2.Distance(position, y.transform.position));
        });

        return planets;
    }

    private List<AsteroidField> GetAsteroidFieldsSortedByDistanceToPoint(Vector2 position) {
        List<AsteroidField> fields = FindObjectsOfType<AsteroidField>().ToList();
        fields.Sort(delegate(AsteroidField x, AsteroidField y) {
            return (int)(Vector2.Distance(position, x.transform.position) - Vector2.Distance(position, y.transform.position));
        });

        return fields;
    }
}
