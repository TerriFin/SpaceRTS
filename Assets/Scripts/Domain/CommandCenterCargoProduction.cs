using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandCenterCargoProduction : MonoBehaviour {

    public ProductionBar productionBar;

    private bool producing;
    private MineralStorage storage;

    private void Start() {
        storage = GetComponent<MineralStorage>();
    }

    private void Update() {
        if (!producing) {
            if (CargoShipManager.SmallCargoes[tag] + CargoShipManager.SmallCargoesBeingBuilt[tag].Count < FactionManager.Factions[tag].desiredSmallCargoes) {
                if (storage.currentMineralStorage >= FactionManager.Factions[tag].smallCargoMineralPrice && FactionManager.Factions[tag].money >= FactionManager.Factions[tag].smallCargoMoneyPrice) {
                    CargoShipManager.SmallCargoesBeingBuilt[tag].Add(this);
                    storage.currentMineralStorage -= FactionManager.Factions[tag].smallCargoMineralPrice;
                    storage.ShowOrExtendStorageBar();
                    FactionManager.Factions[tag].money -= FactionManager.Factions[tag].smallCargoMoneyPrice;
                    StartCoroutine(ProduceCargoShip(FactionManager.Factions[tag].smallCargo, FactionManager.Factions[tag].smallCargoProductionTime));
                }

            } else if (CargoShipManager.MediumCargoes[tag] + CargoShipManager.MediumCargoesBeingBuilt[tag].Count < FactionManager.Factions[tag].desiredMediumCargoes) {
                if (storage.currentMineralStorage >= FactionManager.Factions[tag].mediumCargoMineralPrice && FactionManager.Factions[tag].money >= FactionManager.Factions[tag].mediumCargoMoneyPrice) {
                    CargoShipManager.MediumCargoesBeingBuilt[tag].Add(this);
                    storage.currentMineralStorage -= FactionManager.Factions[tag].mediumCargoMineralPrice;
                    storage.ShowOrExtendStorageBar();
                    FactionManager.Factions[tag].money -= FactionManager.Factions[tag].mediumCargoMoneyPrice;
                    StartCoroutine(ProduceCargoShip(FactionManager.Factions[tag].mediumCargo, FactionManager.Factions[tag].mediumCargoProductionTime));
                }

            } else if (CargoShipManager.BigCargoes[tag] + CargoShipManager.BigCargoesBeingBuilt[tag].Count < FactionManager.Factions[tag].desiredBigCargoes) {
                if (storage.currentMineralStorage >= FactionManager.Factions[tag].bigCargoMineralPrice && FactionManager.Factions[tag].money >= FactionManager.Factions[tag].bigCargoMoneyPrice) {
                    CargoShipManager.BigCargoesBeingBuilt[tag].Add(this);
                    storage.currentMineralStorage -= FactionManager.Factions[tag].bigCargoMineralPrice;
                    storage.ShowOrExtendStorageBar();
                    FactionManager.Factions[tag].money -= FactionManager.Factions[tag].bigCargoMoneyPrice;
                    StartCoroutine(ProduceCargoShip(FactionManager.Factions[tag].bigCargo, FactionManager.Factions[tag].bigCargoProductionTime));
                }
            }
        }
    }

    private IEnumerator ProduceCargoShip(GameObject producedShip, float productionTime) {
        producing = true;
        ProductionBar bar = Instantiate(productionBar).GetComponent<ProductionBar>();
        bar.productionTime = productionTime;
        bar.origin = transform;

        yield return new WaitForSeconds(productionTime);

        CargoShipManager.SmallCargoesBeingBuilt[tag].Remove(this);
        CargoShipManager.MediumCargoesBeingBuilt[tag].Remove(this);
        CargoShipManager.BigCargoesBeingBuilt[tag].Remove(this);
        GameObject ship = Instantiate(producedShip);
        ship.tag = tag;
        ship.transform.position = transform.position;
        ship.GetComponent<ShipMovement>().ORIGIN = gameObject;
        producing = false;
    }


    private void OnDestroy() {
        CargoShipManager.SmallCargoesBeingBuilt[tag].Remove(this);
        CargoShipManager.MediumCargoesBeingBuilt[tag].Remove(this);
        CargoShipManager.BigCargoesBeingBuilt[tag].Remove(this);
    }
}
