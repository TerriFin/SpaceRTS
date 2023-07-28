using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CargoShipButton : MonoBehaviour {

    public bool plus;
    public bool minus;
    public bool smallCargo;
    public bool mediumCargo;
    public bool bigCargo;

    private void Start() {
        GetComponent<Button>().onClick.AddListener(HandleClick);
    }

    private void HandleClick() {
        if (plus) {
            if (smallCargo) {
                if (FactionManager.PlayerFaction.desiredSmallCargoes < 99) {
                    FactionManager.PlayerFaction.desiredSmallCargoes++;
                }
            } else if (mediumCargo) {
                if (FactionManager.PlayerFaction.desiredMediumCargoes < 99) {
                    FactionManager.PlayerFaction.desiredMediumCargoes++;
                }
            } else if (bigCargo) {
                if (FactionManager.PlayerFaction.desiredBigCargoes < 99) {
                    FactionManager.PlayerFaction.desiredBigCargoes++;
                }
            }
        } else if (minus) {
            if (smallCargo) {
                if (FactionManager.PlayerFaction.desiredSmallCargoes > 0) {
                    FactionManager.PlayerFaction.desiredSmallCargoes--;
                }
            } else if (mediumCargo) {
                if (FactionManager.PlayerFaction.desiredMediumCargoes > 0) {
                    FactionManager.PlayerFaction.desiredMediumCargoes--;
                }
            } else if (bigCargo) {
                if (FactionManager.PlayerFaction.desiredBigCargoes > 0) {
                    FactionManager.PlayerFaction.desiredBigCargoes--;
                }
            }
        }
    }
}
