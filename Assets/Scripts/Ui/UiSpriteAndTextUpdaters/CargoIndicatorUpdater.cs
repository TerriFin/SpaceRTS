using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CargoIndicatorUpdater : MonoBehaviour {

    public bool smallCargo;
    public bool mediumCargo;
    public bool bigCargo;
    public Image cargoShipImage;
    public TMP_Text mineralPrice;
    public TMP_Text moneyPrice;

    private TMP_Text AttachedText;

    private void Start() {
        AttachedText = GetComponent<TMP_Text>();
        cargoShipImage = GetComponentInChildren<Image>();

        if (smallCargo) {
            cargoShipImage.sprite = FactionManager.PlayerFaction.smallCargo.GetComponent<SpriteRenderer>().sprite;
            mineralPrice.text = "" + FactionManager.PlayerFaction.smallCargoMineralPrice;
            moneyPrice.text = "" + FactionManager.PlayerFaction.smallCargoMoneyPrice;
        } else if (mediumCargo) {
            cargoShipImage.sprite = FactionManager.PlayerFaction.mediumCargo.GetComponent<SpriteRenderer>().sprite;
            mineralPrice.text = "" + FactionManager.PlayerFaction.mediumCargoMineralPrice;
            moneyPrice.text = "" + FactionManager.PlayerFaction.mediumCargoMoneyPrice;
        } else if (bigCargo) {
            cargoShipImage.sprite = FactionManager.PlayerFaction.bigCargo.GetComponent<SpriteRenderer>().sprite;
            mineralPrice.text = "" + FactionManager.PlayerFaction.bigCargoMineralPrice;
            moneyPrice.text = "" + FactionManager.PlayerFaction.bigCargoMoneyPrice;
        }
    }

    private void Update() {
        if (smallCargo) {
            AttachedText.text = CargoShipManager.SmallCargoes[FactionManager.PlayerFaction.factionTag] + "/" + FactionManager.PlayerFaction.desiredSmallCargoes;
        } else if (mediumCargo) {
            AttachedText.text = CargoShipManager.MediumCargoes[FactionManager.PlayerFaction.factionTag] + "/" + FactionManager.PlayerFaction.desiredMediumCargoes;
        } else if (bigCargo) {
            AttachedText.text = CargoShipManager.BigCargoes[FactionManager.PlayerFaction.factionTag] + "/" + FactionManager.PlayerFaction.desiredBigCargoes;
        }
    }
}
