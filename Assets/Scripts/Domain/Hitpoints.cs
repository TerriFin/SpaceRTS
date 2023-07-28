using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitpoints : MonoBehaviour {

    public GameObject healthBar;
    public GameObject explosion;
    public bool shouldExplode;
    public float explosionSize;
    public bool asteroid;
    public float DESTRUCTION_OPINION_PENALTY;
    public int maxHp;
    public float repairDuration;
    public int repairAmount;
    public bool doesNotHeal;
    public bool armed;
    public bool IGNORES_STAGE_BORDERS;

    public int CurrentHp { get; private set; }

    private ShipAlert ShipAlert;
    private MineralStorage Storage;
    private Selectable SelectedData;
    private GameObject CurrentHealthBar;

    private void Start() {
        if (CurrentHp == 0) {
            CurrentHp = maxHp;
        }
        if (!doesNotHeal) {
            StartCoroutine(Heal());
        }

        ShipAlert = GetComponent<ShipAlert>();
        Storage = GetComponent<MineralStorage>();
        SelectedData = GetComponent<Selectable>();
        CurrentHealthBar = null;

        if (SelectedData != null && SelectedData.selectableType == Selectable.Types.buildingShip) {
            Selectable.Types buildingType = GetBuildingShipBuildingType();
            if (buildingType != Selectable.Types.nothing) {
                BuildingManager.Buildings[tag].Add(this);
                BuildingManager.BuildingAmountsByFactionAndType[tag][buildingType.ToString()].Add(GetComponent<Selectable>());
            }
        } else if (gameObject.layer == LayerMask.NameToLayer("Ship")) {
            if (GetComponent<MilitaryShipClickReact>() != null) {
                ShipsManager.MilShips[tag].Add(this);
            } else {
                ShipsManager.CivShips[tag].Add(this);
            }
        } else if (gameObject.layer == LayerMask.NameToLayer("Building")) {
            BuildingManager.Buildings[tag].Add(this);
            BuildingManager.BuildingAmountsByFactionAndType[tag][SelectedData.selectableType.ToString()].Add(GetComponent<Selectable>());
        }
    }

    private void OnDestroy() {
        if (SelectedData != null && SelectedData.selectableType == Selectable.Types.buildingShip) {
            Selectable.Types buildingType = GetBuildingShipBuildingType();
            if (buildingType != Selectable.Types.nothing) {
                BuildingManager.Buildings[tag].Remove(this);
                BuildingManager.BuildingAmountsByFactionAndType[tag][buildingType.ToString()].Remove(GetComponent<Selectable>());
            }
        } else if (gameObject.layer == LayerMask.NameToLayer("Ship")) {
            if (GetComponent<MilitaryShipClickReact>() != null) {
                ShipsManager.MilShips[tag].Remove(this);
            } else {
                ShipsManager.CivShips[tag].Remove(this);
            }
        } else if (gameObject.layer == LayerMask.NameToLayer("Building")) {
            BuildingManager.Buildings[tag].Remove(this);
            BuildingManager.BuildingAmountsByFactionAndType[tag][SelectedData.selectableType.ToString()].Remove(GetComponent<Selectable>());
        }

        

        if (CurrentHealthBar != null) {
            Destroy(CurrentHealthBar);
        }
    }

    private void Update() {
        if (CurrentHp <= 0) {
            if (shouldExplode) {
                Explosion currentExplosion = Instantiate(explosion, transform.position, Quaternion.identity).GetComponent<Explosion>();
                currentExplosion.Explode(tag, explosionSize, 0, transform.position, 0.03f);
            }

            SelectionManager.HandleDestruction(gameObject.GetComponent<Selectable>());
            Destroy(gameObject);
        }
    }

    public float GetCurrentHpPercentage() {
        return (float)CurrentHp / (float)maxHp;
    }

    public void SetHpToPercentage(float percentage) {
        CurrentHp = (int) (maxHp * percentage);
    }

    public bool TakeDamage(int amount, Vector2 origin, string faction) {
        ShowOrExtendHealthBar();
        if (ShipAlert != null) {
            ShipAlert.Alert(origin, true);
        }
        CurrentHp -= amount;
        if (CurrentHp <= 0) {
            if (Storage != null && faction != null) {
                FactionManager.Factions[faction].ModifyMoney(Storage.currentMineralStorage * 12);
                if (DESTRUCTION_OPINION_PENALTY > 0) FactionOpinionManager.ModifyFactionOpinion(tag, faction, -DESTRUCTION_OPINION_PENALTY);
            }
            return true;
        } else {
            return false;
        }
    }

    public void DestroyThis() {
        CurrentHp = -1000;
    }

    IEnumerator Heal() {
        while (true) {
            yield return new WaitForSeconds(repairDuration);
            if (CurrentHp + repairAmount > maxHp) {
                CurrentHp = maxHp;
            } else {
                CurrentHp += repairAmount;
            }
        }
    }

    private void ShowOrExtendHealthBar() {
        if (CurrentHealthBar == null && healthBar != null) {
            CurrentHealthBar = Instantiate(healthBar);
            HitpointsBar hitpointsBar = CurrentHealthBar.GetComponent<HitpointsBar>();
            hitpointsBar.Hitpoints = this;
            if (asteroid || gameObject.layer == LayerMask.NameToLayer("Building")) hitpointsBar.SetAsBuilding();
        } else {
            CurrentHealthBar.GetComponent<HitpointsBar>().ResetTimer();
        }
    }

    private Selectable.Types GetBuildingShipBuildingType() {
        // Building Ship
        ConstructionShipAi buildingShipAi = GetComponent<ConstructionShipAi>();
        if (buildingShipAi != null) return buildingShipAi.Building.GetComponent<Selectable>().selectableType;
        // Deployed building Ship
        BuildingShipLogic buildingShipLogic = GetComponent<BuildingShipLogic>();
        if (buildingShipAi != null) return buildingShipLogic.building.GetComponent<Selectable>().selectableType;

        return Selectable.Types.nothing;
    }
}
