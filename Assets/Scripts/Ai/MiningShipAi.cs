using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningShipAi : MonoBehaviour, IAi {

    public float MINING_DISTANCE;

    private ShipMovement Controls;
    private MineralStorage Storage;
    private Sensors Sensors;
    private Collider2D CurrentAsteroid;

    public void InitializeAi() {
        Controls = GetComponent<ShipMovement>();
        Storage = GetComponent<MineralStorage>();
        Sensors = GetComponent<Sensors>();
        CurrentAsteroid = null;
    }

    public void ExecuteStep() {
        if (Controls.ORIGIN == null) {
            Destroy(gameObject);
        }

        if (Storage.FreeStorage() == 0 || Sensors.Asteroids.Count == 0) {
            if (Controls.ORIGIN != null) {
                Controls.SetPrimaryTargetPos(Controls.ORIGIN.transform.position);
                if (Controls.AreWeThereYet()) {
                    int overFlowMinerals = Controls.ORIGIN.GetComponent<MineralStorage>().GiveMinerals(Storage.currentMineralStorage);
                    Storage.TakeMinerals(Storage.currentMineralStorage);
                    Storage.GiveMinerals(overFlowMinerals);
                }
            }
        } else {
            if (CurrentAsteroid == null) {
                CurrentAsteroid = GetNewTarget(Sensors.Asteroids);
            }

            if (CurrentAsteroid != null) {
                float randomNumber = Random.Range(0, Mathf.PI * 2);
                Vector2 randomCircleSpot = new Vector2(Mathf.Sin(randomNumber), Mathf.Cos(randomNumber)) * MINING_DISTANCE;
                Controls.SetPrimaryTargetPos((Vector2) CurrentAsteroid.transform.position + randomCircleSpot);
            }
        }
    }

    private Collider2D GetNewTarget(List<Collider2D> targets) {
        if (targets.Count != 0) {
            return targets[Random.Range(0, targets.Count)];
        }

        return null;
    }
}
