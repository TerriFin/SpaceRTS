using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

public class Sensors : MonoBehaviour {

    public struct SensorUpdateJob : IJob {
        [ReadOnly]
        public NativeList<Sensors.Data> InputObjects;
        [NativeDisableParallelForRestriction]
        public NativeArray<Sensors.Data> OutputObjects;

        public void Execute() {
            if (InputObjects.Length > 1) {
                bool enemy = false;
                bool enemyArmed = false;
                bool enemyMilitary = false;
                bool asteroid = false;
                for (int index = 1; index < InputObjects.Length; index++) {
                    Sensors.Data data = InputObjects[index];
                    float distance = Vector2.Distance(InputObjects[0].Position, data.Position);
                    if (OutputObjects[data.Type].Distance == 0 || distance < OutputObjects[data.Type].Distance) {
                        data.Distance = distance;
                        OutputObjects[data.Type] = data;

                        if (data.Type == 0) {
                            enemy = true;
                        } else if (data.Type == 1) {
                            enemyArmed = true;
                        } else if (data.Type == 2) {
                            enemyMilitary = true;
                        } else if (data.Type == 3) {
                            asteroid = true;
                        }
                    }
                }

                if (!enemy) {
                    OutputObjects[0] = new Sensors.Data(Vector2.zero, 0, -1, 0);
                }

                if (!enemyArmed) {
                    OutputObjects[1] = new Sensors.Data(Vector2.zero, 1, -1, 0);
                }

                if (!enemyMilitary) {
                    OutputObjects[2] = new Sensors.Data(Vector2.zero, 2, -1, 0);
                }

                if (!asteroid) {
                    OutputObjects[3] = new Sensors.Data(Vector2.zero, 3, -1, 0);
                }
            }
        }
    }

    public struct Data {
        public Vector2 Position { get; private set; }
        public int Type { get; private set; }
        public int Index { get; private set; }
        public float Distance { get; set; }

        public Data(Vector2 position, int type, int index, float distance) {
            Position = position;
            Type = type;
            Index = index;
            Distance = distance;
        }
    }

    public float refreshTime;
    public float sensorAreaRadius;

    public List<Collider2D> Allies { get; private set; }
    public List<Collider2D> ArmedAllies { get; private set; }
    public List<Collider2D> ArmedAlliesMilitary { get; private set; }
    public List<Collider2D> Enemies { get; private set; }
    public List<Collider2D> ArmedEnemies { get; private set; }
    public List<Collider2D> ArmedEnemiesMilitary { get; private set; }
    public List<Collider2D> Asteroids { get; private set; }

    private NativeList<Sensors.Data> EnemiesAndAsteroids { get; set; }
    private NativeArray<Sensors.Data> ClosestEnemiesAndAsteroid { get; set; }
    private JobHandle CurrentJob { get; set; }

    private void Start() {
        Allies = new List<Collider2D>();
        ArmedAllies = new List<Collider2D>();
        ArmedAlliesMilitary = new List<Collider2D>();
        Enemies = new List<Collider2D>();
        ArmedEnemies = new List<Collider2D>();
        ArmedEnemiesMilitary = new List<Collider2D>();
        Asteroids = new List<Collider2D>();

        EnemiesAndAsteroids = new NativeList<Sensors.Data>(16, Allocator.Persistent);
        ClosestEnemiesAndAsteroid = new NativeArray<Sensors.Data>(4, Allocator.Persistent);
        CurrentJob = new JobHandle();

        StartCoroutine(ScanArea());
    }

    private void OnDestroy() {
        CurrentJob.Complete();

        EnemiesAndAsteroids.Dispose();
        ClosestEnemiesAndAsteroid.Dispose();
    }

    private IEnumerator ScanArea() {
        while (true) {
            Allies.Clear();
            ArmedAllies.Clear();
            ArmedAlliesMilitary.Clear();
            Enemies.Clear();
            ArmedEnemies.Clear();
            ArmedEnemiesMilitary.Clear();
            Asteroids.Clear();

            CurrentJob.Complete();
            EnemiesAndAsteroids.Clear();
            EnemiesAndAsteroids.Add(new Sensors.Data(transform.position, -1, -1, 0));

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, sensorAreaRadius);
            foreach(Collider2D collider in colliders) {
                if (collider.gameObject != gameObject && (collider.gameObject.layer == LayerMask.NameToLayer("Ship") || collider.gameObject.layer == LayerMask.NameToLayer("Building"))) {
                    if (collider.CompareTag(tag)) {
                        Allies.Add(collider);

                        if (collider.GetComponent<Hitpoints>().armed) {
                            ArmedAllies.Add(collider);

                            Selectable colliderData = collider.GetComponent<Selectable>();
                            if (colliderData.selectableType != Selectable.Types.police && colliderData.selectableType != Selectable.Types.raider && colliderData.selectableType != Selectable.Types.fighter) {
                                ArmedAlliesMilitary.Add(collider);
                            }
                        }
                    } else {
                        if (RelationShipManager.AreFactionsInWar(tag, collider.tag)) {
                            Enemies.Add(collider);

                            if (collider.GetComponent<Hitpoints>().armed) {
                                ArmedEnemies.Add(collider);

                                Selectable colliderData = collider.GetComponent<Selectable>();
                                if (colliderData.selectableType != Selectable.Types.police && colliderData.selectableType != Selectable.Types.raider && colliderData.selectableType != Selectable.Types.fighter) {
                                    ArmedEnemiesMilitary.Add(collider);
                                    EnemiesAndAsteroids.Add(new Sensors.Data(collider.transform.position, 2, ArmedEnemiesMilitary.Count - 1, float.MaxValue));
                                    EnemiesAndAsteroids.Add(new Sensors.Data(collider.transform.position, 1, ArmedEnemies.Count - 1, float.MaxValue));
                                    EnemiesAndAsteroids.Add(new Sensors.Data(collider.transform.position, 0, Enemies.Count - 1, float.MaxValue));
                                } else {
                                    EnemiesAndAsteroids.Add(new Sensors.Data(collider.transform.position, 1, ArmedEnemies.Count - 1, float.MaxValue));
                                    EnemiesAndAsteroids.Add(new Sensors.Data(collider.transform.position, 0, Enemies.Count - 1, float.MaxValue));
                                }
                            } else {
                                EnemiesAndAsteroids.Add(new Sensors.Data(collider.transform.position, 0, Enemies.Count - 1, float.MaxValue));
                            }
                        }
                    }
                } else if (collider.CompareTag("Asteroid")) {
                    Asteroids.Add(collider);
                    EnemiesAndAsteroids.Add(new Sensors.Data(collider.transform.position, 3, Asteroids.Count - 1, float.MaxValue));
                }
            }

            SensorUpdateJob job = new SensorUpdateJob { InputObjects = EnemiesAndAsteroids, OutputObjects = ClosestEnemiesAndAsteroid };
            CurrentJob = job.Schedule();

            yield return new WaitForSeconds(refreshTime);
        }
    }

    public Collider2D GetClosestEnemy() {
        CurrentJob.Complete();
        if (ClosestEnemiesAndAsteroid[0].Index != -1 && ClosestEnemiesAndAsteroid[0].Index < Enemies.Count) {
            return Enemies[ClosestEnemiesAndAsteroid[0].Index];
        }
        return null;
    }

    public Collider2D GetClosestArmedEnemy() {
        CurrentJob.Complete();
        if (ClosestEnemiesAndAsteroid[1].Index != -1 && ClosestEnemiesAndAsteroid[1].Index < ArmedEnemies.Count) {
            return ArmedEnemies[ClosestEnemiesAndAsteroid[1].Index];
        }
        return null;
    }

    public Collider2D GetClosestMilitaryEnemy() {
        CurrentJob.Complete();
        if (ClosestEnemiesAndAsteroid[2].Index != -1 && ClosestEnemiesAndAsteroid[2].Index < ArmedEnemiesMilitary.Count) {
            return ArmedEnemiesMilitary[ClosestEnemiesAndAsteroid[2].Index];
        }
        return null;
    }

    public Collider2D GetClosestAsteroid() {
        CurrentJob.Complete();
        if (ClosestEnemiesAndAsteroid[3].Index != -1 && ClosestEnemiesAndAsteroid[3].Index < Asteroids.Count) {
            return Asteroids[ClosestEnemiesAndAsteroid[3].Index];
        }
        return null;
    }

    public Collider2D GetRandomEnemy() {
        try {
            return Enemies[Random.Range(0, Enemies.Count)];
        } catch {
            return null;
        }
    }

    public Collider2D GetRandomArmedEnemy() {
        try {
            return ArmedEnemies[Random.Range(0, ArmedEnemies.Count)];
        } catch {
            return null;
        }
    }

    public Collider2D GetRandomMilitaryEnemy() {
        try {
            return ArmedEnemiesMilitary[Random.Range(0, ArmedEnemiesMilitary.Count)];
        } catch {
            return null;
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, sensorAreaRadius);
    }
}
