using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnShipsRandom : MonoBehaviour, IScenarioEffect {
    public GameObject SHIP;
    public int AMOUNT;

    private LevelBorderManager borders;

    private void Start() {
        borders = FindObjectOfType<LevelBorderManager>();
    }

    public void Effect() {
        float randomNumber = Random.Range(0, Mathf.PI * 2);
        Vector2 randomCircleSpot = new Vector2(Mathf.Sin(randomNumber), Mathf.Cos(randomNumber)) * borders.CurrentSize * 1.75f;
        for (int i = 0; i < AMOUNT; i++) {
            GameObject newShip = Instantiate(SHIP, (Vector3) randomCircleSpot + new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0), Quaternion.identity);
            MineralStorage storage = newShip.GetComponent<MineralStorage>();
            if (storage != null) storage.GiveMinerals(storage.maxMineralStorage);
            newShip.GetComponent<Hitpoints>().IGNORES_STAGE_BORDERS = true;
            if (newShip.GetComponent<MilitaryShipClickReact>() != null) {
                newShip.GetComponent<AiBase>().SetAiActive(false);
                newShip.GetComponent<IShipMovement>().SetPrimaryTargetPos(newShip.transform.position.normalized * borders.CurrentSize * 0.825f);
            }
        }
    }
}
