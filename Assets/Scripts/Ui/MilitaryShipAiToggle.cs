using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MilitaryShipAiToggle : MonoBehaviour {

    public bool AI_TOGGLE;
    public bool COMBAT_MODULE_TOGGLE;

    public Image IMAGE_TO_UPDATE;
    public Sprite TurnOnSprite;
    public Sprite TurnOffSprite;

    private void Start() {
        GetComponent<Button>().onClick.AddListener(ToggleAi);
        UpdateButtonPicture();
    }

    private void ToggleAi() {
        // To avoid costly operations, we assume the ai is wanted to be kept off during fights
        bool targetStatus = false;
        if (AI_TOGGLE) {
            // This is so that if the first ship is destroyed, this will not fail
            if (SelectionManager.selected[0] != null) {
                targetStatus = !SelectionManager.selected[0].GetComponent<AiBase>().aiActive;
            }

            foreach (Selectable ship in SelectionManager.selected) {
                if (ship != null) {
                    ship.GetComponent<AiBase>().SetAiActive(targetStatus);
                }
            }
        } else if (COMBAT_MODULE_TOGGLE) {
            // This is so that if the first ship is destroyed, this will not fail
            if (SelectionManager.selected[0] != null) {
                targetStatus = !SelectionManager.selected[0].GetComponent<AiBase>().combatModuleActive;
            }

            foreach (Selectable ship in SelectionManager.selected) {
                if (ship != null) {
                    ship.GetComponent<AiBase>().SetCombatModuleActive(targetStatus);
                }
            }
        }

        UpdateButtonPicture();
    }

    public void UpdateButtonPicture() {
        if (AI_TOGGLE) {
            IMAGE_TO_UPDATE.sprite = SelectionManager.selected[0].GetComponent<AiBase>().aiActive ? TurnOffSprite : TurnOnSprite;
        } else if (COMBAT_MODULE_TOGGLE) {
            IMAGE_TO_UPDATE.sprite = SelectionManager.selected[0].GetComponent<AiBase>().combatModuleActive ? TurnOffSprite : TurnOnSprite;
        }
    }
}
