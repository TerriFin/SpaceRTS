using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionAiFleetManager : MonoBehaviour {

    public int attackFleetSize;
    public int defenceFleetSize;
    public int attackFleetSizeRandomness;
    public int defenceFleetSizeRandomness;
    public float shipRequiredHpPercentage;
    public float fleetAtDestinationDistance;
    public float fleetRegroupDistance;
    public int orderLengthSeconds;
    public int orderRepeatSeconds;
    public int howManyMoreDefenceFleets;
    public float updateTime;
    public bool ignoreCruisers;

    public List<Fleet> AttackFleets { get; private set; }
    public List<Fleet> DefenceFleets { get; private set; }

    private FactionAiBase factionAiBase;

    private void Start() {
        attackFleetSize += Random.Range(-attackFleetSizeRandomness, attackFleetSizeRandomness + 1);
        defenceFleetSize += Random.Range(-defenceFleetSizeRandomness, defenceFleetSizeRandomness + 1);

        factionAiBase = GetComponent<FactionAiBase>();

        DefenceFleets = new List<Fleet>();
        if (factionAiBase.ON) CreateNewFleet(false);

        AttackFleets = new List<Fleet>();
        if (factionAiBase.ON) CreateNewFleet(true);

        StartCoroutine(ManageFleets());
    }

    public IEnumerator ManageFleets() {
        while (true) {
            yield return new WaitForSeconds(updateTime);

            if (factionAiBase.ON) {
                List<Hitpoints> militaryShips = ignoreCruisers ? ShipsManager.MilShips[tag].FindAll(hitpoints => {
                    Selectable data = hitpoints.GetComponent<Selectable>();
                    return data.selectableType != Selectable.Types.cruiser && data.selectableType != Selectable.Types.specialShip;
                }) : ShipsManager.MilShips[tag].FindAll(hitpoints => hitpoints.GetComponent<Selectable>().selectableType != Selectable.Types.specialShip);
                int totalShips = 0;
                foreach (Hitpoints ship in militaryShips) {
                    if (ship.CurrentHp > ship.maxHp * shipRequiredHpPercentage) totalShips++;
                }

                ManageFleetAmounts(totalShips, AttackFleets.Count, DefenceFleets.Count);

                foreach (Fleet fleet in DefenceFleets) {
                    fleet.RemoveInjuredShips();

                    if (!fleet.FleetFull()) {
                        fleet.FillFleet();
                    }

                    fleet.CheckAndResetFleetTargetIfNotInConflict();
                }

                foreach (Fleet fleet in AttackFleets) {
                    fleet.RemoveInjuredShips();

                    // If fleet is not big enough, take it back
                    if (fleet.targetFaction != null && !CompareTag(fleet.targetFaction) && fleet.Ships.Count <= (float)attackFleetSize / 2) {
                        fleet.MoveOrder(factionAiBase.GetLocationThatCalledHelp(), tag, true);
                    }

                    if (!fleet.FleetFull()) {
                        fleet.FillFleet();
                    }

                    fleet.CheckAndResetFleetTargetIfNotInConflict();
                }
            }
        }
    }

    public Fleet GetRandomAttackFleet() {
        try {
            return AttackFleets[Random.Range(0, AttackFleets.Count)];
        } catch {
            return null;
        }
    }

    public Fleet GetRandomDefenceFleet() {
        try {
            return DefenceFleets[Random.Range(0, DefenceFleets.Count)];
        } catch {
            return null;
        }
    }

    private void ManageFleetAmounts(int totalShips, int attackFleetsAmount, int defenceFleetsAmount) {
        int currentMaxFleetsSize = attackFleetsAmount * attackFleetSize + defenceFleetsAmount * defenceFleetSize;

        // If there are not enough ships to fill all fleets
        if (totalShips < currentMaxFleetsSize) {
            // If there are too many attack fleets
            if (attackFleetsAmount + howManyMoreDefenceFleets > defenceFleetsAmount && AttackFleets.Count > 1) {
                RemoveAndDestroyFleet(GetSmallestFleet(true), true);
            } else if (DefenceFleets.Count > 1) {
                RemoveAndDestroyFleet(GetSmallestFleet(false), false);
            }
        }
        // If there are more ships than can be fit to current fleets
        if (totalShips > currentMaxFleetsSize + attackFleetSize) {
            if (attackFleetsAmount + howManyMoreDefenceFleets < defenceFleetsAmount) {
                CreateNewFleet(true);
            } else {
                CreateNewFleet(false);
            }
        }
    }

    private void CreateNewFleet(bool attackFleet) {
        if (attackFleet) {
            Fleet newAttackFleet = gameObject.AddComponent<Fleet>();
            newAttackFleet.InitializeFleet(attackFleetSize, shipRequiredHpPercentage, fleetAtDestinationDistance, fleetRegroupDistance, orderLengthSeconds, orderRepeatSeconds, ignoreCruisers, tag, factionAiBase);
            AttackFleets.Add(newAttackFleet);
        } else {
            Fleet newDefenceFleet = gameObject.AddComponent<Fleet>();
            newDefenceFleet.InitializeFleet(defenceFleetSize, shipRequiredHpPercentage, fleetAtDestinationDistance, fleetRegroupDistance, orderLengthSeconds, orderRepeatSeconds, ignoreCruisers, tag, factionAiBase);
            DefenceFleets.Add(newDefenceFleet);
        }
    }

    private Fleet GetSmallestFleet(bool attackFleet) {
        int smallestSize = int.MaxValue;
        Fleet smallestFleet = null;

        if (attackFleet) {
           foreach (Fleet fleet in AttackFleets) {
                if (fleet.fleetSize < smallestSize) {
                    smallestSize = fleet.fleetSize;
                    smallestFleet = fleet;
                }
            }

            return smallestFleet;
        } else {
            foreach (Fleet fleet in DefenceFleets) {
                if (fleet.fleetSize < smallestSize) {
                    smallestSize = fleet.fleetSize;
                    smallestFleet = fleet;
                }
            }

            return smallestFleet;
        }
    }

    private void RemoveAndDestroyFleet(Fleet fleet, bool attackFleet) {
        if (fleet != null) {
            if (attackFleet) {
                AttackFleets.Remove(fleet);
                fleet.DestroyThisFleet();
            } else {
                DefenceFleets.Remove(fleet);
                fleet.DestroyThisFleet();
            }
        }
    }
}
