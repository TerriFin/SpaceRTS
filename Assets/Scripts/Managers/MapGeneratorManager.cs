using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneratorManager : MonoBehaviour {
    [System.Serializable]
    public struct FactionData {
        public int statusIndex;
        public int startingResourcesIndex;

        public FactionData(int statusIndex, int startingResourcesIndex) {
            this.statusIndex = statusIndex;
            this.startingResourcesIndex = startingResourcesIndex;
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
    public GameObject EmpireCommandCenter;
    public GameObject PirateCommandCenter;

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
    public int STARTING_SHIPS_AMOUNT_INDEX;
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
                FactionDatas.Add("Federation", new FactionData(3, 1));
                FactionDatas.Add("Empire", new FactionData(3, 1));
                FactionDatas.Add("Pirate", new FactionData(3, 1));

                MAP_SIZE_LAST_INDEX = 2;
                MAP_ASTEROIDS_LAST_INDEX = 2;
                MAP_PLANETS_LAST_INDEX = 2;
                TOTAL_WAR_TIMER_LAST_INDEX = 3;
                STARTING_SHIPS_AMOUNT_INDEX = 2;
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

    public void InitializeWithTestData(List<string> factionTags, List<int> factionDifficulties, List<int> factionStartingResources) {
        FactionDatas = new Dictionary<string, FactionData>();
        for (int i = 0; i < factionTags.Count; i++) {
            FactionDatas.Add(factionTags[i], new FactionData(factionDifficulties[i], factionStartingResources[i]));
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
                SpawnFactionStartingShips(STARTING_SHIPS_AMOUNT_INDEX, factionStartingPosition, FederationPolice, FederationRaider, FederationFrigate, FederationTrader);
            } else if (currentFactions[i] == "Empire") {
                Instantiate(EmpireCommandCenter, factionStartingPosition, Quaternion.identity);
                SpawnFactionStartingShips(STARTING_SHIPS_AMOUNT_INDEX, factionStartingPosition, EmpirePolice, EmpireRaider, EmpireFrigate, EmpireTrader);
            } else if (currentFactions[i] == "Pirate") {
                Instantiate(PirateCommandCenter, factionStartingPosition, Quaternion.identity);
                SpawnFactionStartingShips(STARTING_SHIPS_AMOUNT_INDEX, factionStartingPosition, PiratePolice, PirateRaider, PirateFrigate, PirateTrader);
            }
        }

        return currentFactions;
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
        return new Vector2(Mathf.Sin(randomNumber), Mathf.Cos(randomNumber)) * Random.Range(MAP_SIZE * CENTER_DEAD_ZONE, MAP_SIZE * 0.9f);
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
}
