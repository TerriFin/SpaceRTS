using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fleet : MonoBehaviour {

    // THESE MUST BE SET!!
    public int fleetSize = 0;
    public float shipRequiredHpPercentage = 1f;
    public float fleetAtDestinationDistance = 0f;
    public float fleetRegroupDistance = 0f;
    public int orderLengthSeconds = 0;
    public int orderRepeatSeconds = 0;
    public bool ignoreCruisers = false;
    public string factionTag = "";
    public string targetFaction;
    public List<MilitaryShipClickReact> Ships { get; private set; }

    private Vector2 Destination;
    private bool OldOrder;
    private FactionAiBase AiBase;

    public void InitializeFleet(int fleetSize, float shipRequiredHpPercentage, float fleetAtDestinationDistance, float fleetRegroupDistance, int orderLengthSeconds, int orderRepeatSeconds, bool ignoreCruisers, string factionTag, FactionAiBase aiBase) {
        this.fleetSize = fleetSize;
        this.shipRequiredHpPercentage = shipRequiredHpPercentage;
        this.fleetAtDestinationDistance = fleetAtDestinationDistance;
        this.fleetRegroupDistance = fleetRegroupDistance;
        this.orderLengthSeconds = orderLengthSeconds;
        this.orderRepeatSeconds = orderRepeatSeconds;
        this.ignoreCruisers = ignoreCruisers;
        this.factionTag = factionTag;

        Ships = new List<MilitaryShipClickReact>();
        Destination = Vector2.zero;
        targetFaction = null;
        OldOrder = false;
        AiBase = aiBase;

        FillFleet();
        StartCoroutine(RepeatOrder());
    }

    public bool MoveOrder(Vector2 targetPos, string targetFaction = null, bool overrideOldCommand = false) {
        if (FleetNotBusy() || overrideOldCommand) {
            if (targetPos == Vector2.zero) targetPos += new Vector2(0.01f, 0.01f);

            Destination = targetPos;
            this.targetFaction = targetFaction;
            StartCoroutine(StartOrderTimer());
            Vector2 fleetLocation = GetFleetLocation();
            foreach (MilitaryShipClickReact ship in Ships) {
                ship.ReactToClick(Destination);
                ship.SetSecondaryTargetPos(fleetLocation + ((Vector2) ship.transform.position - fleetLocation) / 2);
            }

            return true;
        }

        return false;
    }

    public void CheckAndResetFleetTargetIfNotInConflict() {
        if (targetFaction != null && !CompareTag(targetFaction) && !RelationShipManager.AreFactionsInWar(tag, targetFaction)) {
            Selectable commandCenter = BuildingManager.GetFactionRandomBuildingByType(tag, Selectable.Types.commandCenter);
            if (commandCenter != null) {
                MoveOrder(commandCenter.transform.position, tag, true);
            } else {
                MoveOrder(AiBase.FactionCenterPoint, tag, true);
            }
        }
    }

    public void FillFleet() {
        if (!FleetFull()) {
            List<Hitpoints> militaryShips = ignoreCruisers ? ShipsManager.MilShips[tag].FindAll(hitpoints => hitpoints.GetComponent<Selectable>().selectableType != Selectable.Types.cruiser) : ShipsManager.MilShips[tag];
            foreach (Hitpoints ship in militaryShips) {
                MilitaryShipClickReact current = ship.GetComponent<MilitaryShipClickReact>();
                if (current.attachedFleet == null && ship.CurrentHp > ship.maxHp * shipRequiredHpPercentage) {
                    current.attachedFleet = this;
                    current.GetComponent<AiBase>().SetAiActive(false);
                    Ships.Add(current);

                    if (Destination == Vector2.zero) Destination = current.transform.position;

                    current.ReactToClick(Destination);

                    if (FleetFull()) break;
                }
            }
        }
    }

    public void RemoveInjuredShips() {
        for (int i = Ships.Count - 1; i >= 0; i--) {
            MilitaryShipClickReact current = Ships[i];
            Hitpoints Hitpoints = current.GetComponent<Hitpoints>();
            if (Hitpoints.CurrentHp < Hitpoints.maxHp * shipRequiredHpPercentage) {
                current.attachedFleet = null;
                current.GetComponent<AiBase>().SetAiActive(true);
                IShipMovement shipMovement = current.GetComponent<IShipMovement>();
                shipMovement.SetPrimaryTargetPos(transform.position);
                shipMovement.SetSecondaryTargetPos(transform.position);
                Ships.RemoveAt(i);
            }
        }
    }

    public void DestroyThisFleet() {
        if (Ships.Count > 0) {
            /* !!!
             * IF SOME SHIPS START AFKING WITHOUT HAVING AI ON AND BEING IN A FLEET THAT IS DESTROYED, IT IS BECAUSE OF Ships.ToArray() BELOW!
             */
            foreach (MilitaryShipClickReact ship in Ships.ToArray()) {
                if (ship != null) {
                    ship.attachedFleet = null;
                    ship.GetComponent<AiBase>().SetAiActive(true);
                    Ships.Remove(ship);
                }
            }
        }

        Destroy(this);
    }

    public bool FleetFull() {
        return Ships.Count >= fleetSize;
    }

    public void RemoveShipFromFleet(MilitaryShipClickReact ship) {
        Ships.Remove(ship);
    }

    public Vector2 GetFleetLocation() {
        if (Ships.Count == 0) {
            return Vector2.zero;
        } else {
            Vector2 position = Vector2.zero;
            foreach (MilitaryShipClickReact ship in Ships) {
                position += (Vector2)ship.transform.position;
            }

            return position / Ships.Count;
        }
    }

    public bool FleetNotBusy() {
        return OldOrder || Vector2.Distance(GetFleetLocation(), Destination) < fleetAtDestinationDistance;
    }

    private IEnumerator StartOrderTimer() {
        OldOrder = false;
        yield return new WaitForSeconds(orderLengthSeconds);
        OldOrder = true;
    }

    private IEnumerator RepeatOrder() {
        while (true) {
            yield return new WaitForSeconds(orderRepeatSeconds);
            Vector2 currentFleetLocation = GetFleetLocation();
            foreach (MilitaryShipClickReact ship in Ships) {
                ship.ReactToClick(Destination);
                if (Vector2.Distance(ship.transform.position, currentFleetLocation) > fleetRegroupDistance) {
                    ship.SetSecondaryTargetPos(currentFleetLocation + ((Vector2)ship.transform.position - currentFleetLocation) / 2);
                }
            }
        }
    }

    private void OnDrawGizmos() {
        if (CompareTag("Federation")) {
            Gizmos.color = new Color(0, 0, 1f);
        } else if (CompareTag("Empire")) {
            Gizmos.color = new Color(1f, 0.5f, 0);
        } else if (CompareTag("Pirate")) {
            Gizmos.color = new Color(1f, 0, 0);
        }

        Vector2 fleetLocation = GetFleetLocation();
        if (fleetLocation != Vector2.zero) {
            if (Destination != Vector2.zero) {
                Gizmos.DrawLine(fleetLocation, Destination);
                Gizmos.DrawWireSphere(Destination, 0.75f);
            }

            if (CompareTag("Federation")) {
                Gizmos.color = new Color(0, 0, 1f, 0.33f);
            } else if (CompareTag("Empire")) {
                Gizmos.color = new Color(1f, 0.5f, 0, 0.33f);
            } else if (CompareTag("Pirate")) {
                Gizmos.color = new Color(1f, 0, 0, 0.33f);
            }

            foreach (MilitaryShipClickReact ship in Ships) {
                Gizmos.DrawLine(ship.transform.position, fleetLocation);
            }
        }
    }
}
