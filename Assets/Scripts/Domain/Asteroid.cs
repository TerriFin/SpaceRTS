using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour {

    public AsteroidField AttachedAsteroidField { private get; set; }

    private void OnDestroy() {
        if (AttachedAsteroidField != null) {
            AttachedAsteroidField.AsteroidDestroyed();
        }
    }
}
