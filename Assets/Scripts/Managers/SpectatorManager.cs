using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorManager : MonoBehaviour {
    public List<GameObject> TO_DESTROY_IF_SPECTATOR;
    public List<GameObject> TO_DESTROY_IF_NOT_SPECTATOR;

    private void Awake() {
        if (FactionManager.PlayerFaction == null) {
            foreach (GameObject gameobject in TO_DESTROY_IF_SPECTATOR) Destroy(gameobject);
        } else {
            foreach (GameObject gameobject in TO_DESTROY_IF_NOT_SPECTATOR) Destroy(gameobject);
        }
    }
}
