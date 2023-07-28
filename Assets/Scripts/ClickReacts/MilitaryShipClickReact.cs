using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilitaryShipClickReact : MonoBehaviour, IReactToClick {

    public float fleetRandomness;
    public Fleet attachedFleet;

    private IShipMovement ShipMovement;
    private CombatModule CombatModule;
    private AiBase AiBase;

    private void Start() {
        ShipMovement = GetComponent<IShipMovement>();
        CombatModule = GetComponent<CombatModule>();
        AiBase = GetComponent<AiBase>();
        attachedFleet = null;
    }

    private void OnDestroy() {
        if (attachedFleet != null) {
            attachedFleet.RemoveShipFromFleet(this);
        }
    }

    public void ReactToClick(Vector2 targetPos) {
        if ((FactionManager.PlayerFaction != null && CompareTag(FactionManager.PlayerFaction.factionTag)) || !CombatModule.active) {
            AiBase.SetAiActive(false);
            // EXPENSIVE HACK TO GET THIS WORKING
            if (FactionManager.PlayerFaction != null && CompareTag(FactionManager.PlayerFaction.factionTag)) {
                foreach (MilitaryShipAiToggle toggle in FindObjectsOfType<MilitaryShipAiToggle>()) toggle.UpdateButtonPicture();
            }
            
            float randomValue = Random.Range(0, Mathf.PI * 2);
            Vector2 randomCircle = new Vector2(Mathf.Sin(randomValue), Mathf.Cos(randomValue)) * Random.Range(0, fleetRandomness);
            ShipMovement.SetPrimaryTargetPos(targetPos + randomCircle);
            ShipMovement.ClearSecondaryTargetPos();
        }
    }

    public void SetSecondaryTargetPos(Vector2 target) {
        if (!CombatModule.active) {
            ShipMovement.SetSecondaryTargetPos(target);
        }
    }
}
