using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnShips : MonoBehaviour, IScenarioEffect {
    public GameObject SHIP;
    public int AMOUNT;
    public bool MOVE_SHIPS_TO_STAGE_AT_START;
    public bool SHIP_AI_ACTIVE;

    private LevelBorderManager borders;

    private void Start() {
        borders = FindObjectOfType<LevelBorderManager>();
    }

    public void Effect() {
        for (int i = 0; i < AMOUNT; i++) {
            GameObject newShip = Instantiate(SHIP, transform.position + new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0), Quaternion.identity);
            if (!borders.LocationInsideCameraArea(newShip.transform.position)) {
                MineralStorage storage = newShip.GetComponent<MineralStorage>();
                if (storage != null) storage.GiveMinerals(storage.maxMineralStorage);
                newShip.GetComponent<Hitpoints>().IGNORES_STAGE_BORDERS = true;
                if (!SHIP_AI_ACTIVE && newShip.GetComponent<MilitaryShipClickReact>() != null) newShip.GetComponent<AiBase>().SetAiActive(false);
                if (MOVE_SHIPS_TO_STAGE_AT_START) newShip.GetComponent<IShipMovement>().SetPrimaryTargetPos(newShip.transform.position.normalized * borders.CurrentSize * 0.825f);
            }
        }
    }
}
