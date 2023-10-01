using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidField : MonoBehaviour {
    public int STARTING_ASTEROIDS;
    public int MAX_ASTEROIDS;
    public float FIELD_RADIUS;
    public bool ASTEROIDS_RESPAWN;
    public float MIN_RESPAWN_RATE;
    public float MAX_RESPAWN_RATE;
    public float BIG_ASTEROID_CHANCE;
    public int MAX_AI_MINES;

    public GameObject SMALL_ASTEROID;
    public GameObject MEDIUM_ASTEROID;
    public GameObject BIG_ASTEROID;

    public int MinesOnTheWay { get; set; }

    public int CurrentAsteroids;

    private void Start() {
        AsteroidFieldManager.AsteroidFields.Add(this);

        MinesOnTheWay = 0;

        CurrentAsteroids = 0;

        for (int i = 0; i < STARTING_ASTEROIDS; i++) {
            SpawnAsteroid();
        }

        if (ASTEROIDS_RESPAWN) {
            StartCoroutine(KeepAsteroidsSpawned());
        }
    }

    public void AsteroidDestroyed() {
        CurrentAsteroids--;
    }

    // See if this can be made faster by maybe storing mines as they are built and destroyed?
    public bool RoomForMine() {
        // For some reason adding LayerMask.NameToLayer as third input does not work, still gets asteroids and other stuff...
        Collider2D[] allBuildingsInField = Physics2D.OverlapCircleAll(transform.position, FIELD_RADIUS * 1.4f);

        int amount = MinesOnTheWay;
        if (amount >= MAX_AI_MINES) return false;
        foreach (Collider2D building in allBuildingsInField) {
            // ...So we check that colliders are buildings here. Bit hacky.
            if (building.gameObject.layer == LayerMask.NameToLayer("Building") && building.GetComponent<Selectable>().selectableType == Selectable.Types.mine) {
                amount++;
                if (amount >= MAX_AI_MINES) return false;
            }
        }

        return true;
    }

    public bool NoBuildingsInField() {
        Collider2D[] allBuildingsInField = Physics2D.OverlapCircleAll(transform.position, FIELD_RADIUS * 1.1f);
        foreach (Collider2D building in allBuildingsInField) {
            if (building.gameObject.layer == LayerMask.NameToLayer("Building")) {
                return false;
            }
        }

        return true;
    }

    public HashSet<string> FactionsPresentInField() {
        HashSet<string> factions = new HashSet<string>();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, FIELD_RADIUS);
        foreach (Collider2D collider in colliders) {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Building")) {
                factions.Add(collider.tag);
            }
        }

        return factions;
    }

    private IEnumerator KeepAsteroidsSpawned() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(MIN_RESPAWN_RATE, MAX_RESPAWN_RATE));
            if (CurrentAsteroids < MAX_ASTEROIDS) {
                SpawnAsteroid();
            }
        }
    }

    private void SpawnAsteroid() {
        GameObject asteroidToSpawn = SMALL_ASTEROID;
        if (Random.Range(0.0f, 1.0f) < BIG_ASTEROID_CHANCE) {
            asteroidToSpawn = BIG_ASTEROID;
        } else {
            if (Random.Range(0.0f, 1.0f) < 0.4) {
                asteroidToSpawn = MEDIUM_ASTEROID;
            }
        }

        float randomNumber = Random.Range(0, Mathf.PI * 2);
        Vector2 randomCircleSpot = new Vector2(Mathf.Sin(randomNumber), Mathf.Cos(randomNumber));
        randomCircleSpot *= Random.Range(0, FIELD_RADIUS);

        GameObject asteroid = Instantiate(asteroidToSpawn, transform);
        asteroid.transform.position =  (Vector2)transform.position + randomCircleSpot;
        asteroid.transform.Rotate(0, 0, Random.Range(0, 360));

        asteroid.GetComponent<Asteroid>().AttachedAsteroidField = this;

        CurrentAsteroids++;
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, FIELD_RADIUS);
    }
}
