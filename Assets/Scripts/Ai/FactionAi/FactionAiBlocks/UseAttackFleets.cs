using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseAttackFleets : FactionBlockWithSkips, ICallHelp {

    public int MIN_FLEETS;
    public int MAX_FLEETS;
    public float PLANET_ATTACK_PERCENTAGE;
    public float MINE_ATTACK_PERCENTAGE;
    public float ATTACK_ENEMY_VULNERABLE_MINE_PERCENTAGE;
    public float COMMAND_CENTER_ATTACK_PERCENTAGE;
    public float REQUIRED_ENEMY_AMOUNT_TO_DEFEND;
    public int SAFE_MINERAL_OVERFLOW_AMOUNT;
    public int SAFE_MONEY_OVERFLOW_AMOUNT;

    private FactionAiFleetManager FleetManager;
    private FactionAiBase Ai;
    private FactionAiBuildingManager AiBuildingManager;
    private WarManagerBlock WarManager;
    private FactionScoresManager FactionScores;
    private float CalculatedPlanetAttackPercentage;
    private float CalculatedMineAttackPercentage;
    private int EnemyFactionScores;

    public override void Initialize() {
        FleetManager = GetComponent<FactionAiFleetManager>();
        Ai = GetComponent<FactionAiBase>();
        AiBuildingManager = GetComponent<FactionAiBuildingManager>();
        WarManager = GetComponent<WarManagerBlock>();
        FactionScores = FindObjectOfType<FactionScoresManager>();
        CalculatedMineAttackPercentage = MINE_ATTACK_PERCENTAGE;
        CalculatedPlanetAttackPercentage = MINE_ATTACK_PERCENTAGE + PLANET_ATTACK_PERCENTAGE;
        EnemyFactionScores = 0;

        BuildBuildingsBlock buildingsBlock = GetComponent<BuildBuildingsBlock>();
        if (SAFE_MINERAL_OVERFLOW_AMOUNT == 0) SAFE_MINERAL_OVERFLOW_AMOUNT = buildingsBlock != null ? buildingsBlock.desiredMineralOverflowAmount / 2 : 16;
        if (SAFE_MONEY_OVERFLOW_AMOUNT == 0) SAFE_MONEY_OVERFLOW_AMOUNT = buildingsBlock != null ? buildingsBlock.desiredMoneyOverflowAmount / 2 : 50;
    }

    public override void Block() {
        int currentMinFleets = FactionScores.GetFactionMilitaryScoreShare(tag) < 0.5f ? MIN_FLEETS : MIN_FLEETS * 2;

        int enemyFactionScores = 0;
        foreach (Faction faction in FactionManager.Factions.Values) {
            if (RelationShipManager.AreFactionsInWar(tag, faction.factionTag)) enemyFactionScores += FactionManager.FactionScoresManager.FactionMilitaryScores[faction.factionTag];
        }

        EnemyFactionScores = enemyFactionScores;

        if (RelationShipManager.IsFactionInWar(tag) && (BuildingManager.BuildingAmountsByFactionAndType[tag][Selectable.Types.commandCenter.ToString()].Count == 0 || EnemyFactionScores < FactionManager.FactionScoresManager.FactionMilitaryScores[tag] * REQUIRED_ENEMY_AMOUNT_TO_DEFEND * BuildingManager.BuildingsMineralStorageFillPercentage(tag) * FactionManager.Factions[tag].FactionMoneyStorageFillPercentage() || WarManager != null && WarManager.TotalWarTriggered)) {
            string targetFaction = RelationShipManager.GetRandomFactionWeAreAttacking(tag);
            if (targetFaction == null) targetFaction = RelationShipManager.GetRandomFactionWeAreFighting(tag);
            float randomNumber = Random.value;
            if (randomNumber < CalculatedMineAttackPercentage && FactionManager.Factions[tag].GetFactionMineralResourcesAmount() < SAFE_MINERAL_OVERFLOW_AMOUNT && BuildingManager.BuildingAmountsByFactionAndType[targetFaction][Selectable.Types.mine.ToString()].Count > 0 && !IsThereSafeAvailableAsteroidField()) {
                AsteroidField closestEnemyAsteroidField = GetEnemyAsteroidFieldToAttack(targetFaction);
                if (closestEnemyAsteroidField != null) {
                    AttackLocation(closestEnemyAsteroidField.transform, Random.Range(currentMinFleets, MAX_FLEETS), targetFaction);
                    return;
                }
            } else if (randomNumber < CalculatedPlanetAttackPercentage && FactionManager.Factions[tag].GetFactionMoneyResourcesAmount() < SAFE_MONEY_OVERFLOW_AMOUNT && PlanetManager.FactionPlanets[targetFaction].Count > 0) {
                List<PlanetCaptureLogic> enemyPlanetsSortedByDistance = PlanetManager.GetPlanetsFromFactionSortedByDistanceToLocation(targetFaction, Ai.FactionCenterPoint);
                if (enemyPlanetsSortedByDistance.Count == 1) {
                    PlanetCaptureLogic closestEnemyPlanet = enemyPlanetsSortedByDistance[0];
                    AttackLocation(closestEnemyPlanet.transform, Random.Range(currentMinFleets, MAX_FLEETS), targetFaction);
                    return;
                } else if (enemyPlanetsSortedByDistance.Count >= 2) {
                    PlanetCaptureLogic closestEnemyPlanet = enemyPlanetsSortedByDistance[Random.Range(0, (enemyPlanetsSortedByDistance.Count / 2) + 1)];
                    AttackLocation(closestEnemyPlanet.transform, Random.Range(currentMinFleets, MAX_FLEETS), targetFaction);
                    return;
                }
            } else {
                AttackFactionBuilding(targetFaction);
            }
        } else {
            Patrol();
        }
    }

    private void AttackFactionBuilding(string targetFaction) {
        if (BuildingManager.BuildingAmountsByFactionAndType[targetFaction][Selectable.Types.commandCenter.ToString()].Count > 0 && Random.Range(0.0f, 1.0f) <= COMMAND_CENTER_ATTACK_PERCENTAGE) {
            Selectable targetCommandCenter = BuildingManager.GetFactionRandomBuildingByType(targetFaction, Selectable.Types.commandCenter);
            if (targetCommandCenter != null) {
                AttackLocation(targetCommandCenter.transform, Random.Range(MIN_FLEETS, MAX_FLEETS), targetFaction);
                return;
            }
        }

        Hitpoints targetLocation = BuildingManager.GetFactionRandomBuilding(targetFaction);
        if (targetLocation != null) AttackLocation(targetLocation.transform, Random.Range(MIN_FLEETS, MAX_FLEETS), targetFaction);
    }

    public void AttackLocation(Transform location, int attackFleetAmount, string targetFaction = null) {
        int currentSentFleets = 0;

        foreach (Fleet fleet in FleetManager.AttackFleets) {
            if (fleet != null && location != null && fleet.fleetSize >= FleetManager.attackFleetSize - 1 && fleet.MoveOrder(location.position, targetFaction)) {
                currentSentFleets++;
                if (currentSentFleets >= attackFleetAmount) break;
            }
        }
    }

    private AsteroidField GetEnemyAsteroidFieldToAttack(string enemyFaction) {
        if (Random.Range(0.0f, 1.0f) < ATTACK_ENEMY_VULNERABLE_MINE_PERCENTAGE) {   // Attack asteroid field farthest away from enemy faction
            List<AsteroidField> sortedAsteroidFields = AsteroidFieldManager.GetAsteroidFieldsSortedByDistanceToLocation(FactionManager.Factions[enemyFaction].ai.FactionCenterPoint);
            List<AsteroidField> sortedEnemyAsteroidFields = new List<AsteroidField>();
            foreach (AsteroidField field in sortedAsteroidFields) {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(field.transform.position, field.FIELD_RADIUS * 1.25f);
                foreach (Collider2D collider in colliders) {
                    if (collider.gameObject.layer == LayerMask.NameToLayer("Building") && collider.gameObject.CompareTag(enemyFaction)) {
                        sortedEnemyAsteroidFields.Add(field);
                    }
                }
            }

            return sortedEnemyAsteroidFields[sortedEnemyAsteroidFields.Count - 1];
        } else {    // Attack asteroid field closest to our faction
            List<AsteroidField> sortedAsteroidFields = AsteroidFieldManager.GetAsteroidFieldsSortedByDistanceToLocation(Ai.FactionCenterPoint);
            List<AsteroidField> sortedEnemyAsteroidFields = new List<AsteroidField>();
            foreach (AsteroidField field in sortedAsteroidFields) {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(field.transform.position, field.FIELD_RADIUS * 1.25f);
                foreach (Collider2D collider in colliders) {
                    if (collider.gameObject.layer == LayerMask.NameToLayer("Building") && collider.gameObject.CompareTag(enemyFaction)) {
                        sortedEnemyAsteroidFields.Add(field);
                    }
                }
            }

            if (sortedEnemyAsteroidFields.Count == 1) return sortedEnemyAsteroidFields[0];
            if (sortedEnemyAsteroidFields.Count > 1) return sortedEnemyAsteroidFields[Random.Range(0, (sortedEnemyAsteroidFields.Count / 2) + 1)];
        }

        return null;
    }

    private bool IsThereSafeAvailableAsteroidField() {
        List<AsteroidField> fields = AsteroidFieldManager.GetAsteroidFieldsWithRoomForMines();
        foreach (AsteroidField field in fields) {
            if (AiBuildingManager.HostileBuildingsBetweenTwoPoints(Ai.FactionCenterPoint, field.transform.position, 3).Count == 0) {
                return true;
            }
        }

        return false;
    }

    private void Patrol() {
        Fleet randomFleet = FleetManager.GetRandomAttackFleet();
        if (randomFleet != null) {
            if (PlanetManager.FactionPlanets[tag].Count > 0 && Random.Range(0, 1.0f) > 0.33f) {
                PlanetCaptureLogic randomPlanet = PlanetManager.GetFactionRandomPlanet(tag);
                randomFleet.MoveOrder(randomPlanet.transform.position, tag);
            } else if (BuildingManager.Buildings[tag].Count > 0) {
                Hitpoints randomBuilding = BuildingManager.GetFactionRandomBuilding(tag);
                randomFleet.MoveOrder(randomBuilding.transform.position, tag);
            }
        }
    }

    public void CallForHelp(Vector2 location, int enemyAmount, bool important) {
        if (enemyAmount >= FleetManager.attackFleetSize && EnemyFactionScores > FactionManager.FactionScoresManager.FactionMilitaryScores[tag] * REQUIRED_ENEMY_AMOUNT_TO_DEFEND) {
            int shipAmount = 0;
            foreach (Fleet fleet in FleetManager.AttackFleets) {
                if (fleet.MoveOrder(location, tag, important)) {
                    shipAmount += fleet.fleetSize;
                    if (shipAmount >= enemyAmount) break;
                }
            }
        }
    }
}
