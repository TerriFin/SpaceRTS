using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {

    public GameObject EXPLOSION;
    public bool CONTROLLED;
    public int DIRECT_DAMAGE;
    public float EXPLOSION_RADIUS;
    public int EXPLOSION_DAMAGE;
    public float MISSILE_SPEED;
    public float MISSILE_TURN_RATE;
    public float MISSILE_LIFESPAN;
    public bool DESTROYS_ASTEROIDS;
    public Vector2 ORIGIN;

    // THESE GOTTA BE SET FOR MOVEMENT TO WORK!
    public GameObject MISSILE_TARGET;

    private Rigidbody2D Body;

    private void Start() {
        ORIGIN = transform.position;

        Body = GetComponent<Rigidbody2D>();

        StartCoroutine(StartDeleteTimer());
    }

    private void Update() {
        ApplyMovement();
    }

    private void ApplyMovement() {
        Body.AddForce(transform.up * MISSILE_SPEED * Time.deltaTime);
        if (CONTROLLED) {
            if (MISSILE_TARGET != null) {
                float angleToTarget = Vector2.SignedAngle((Vector2)MISSILE_TARGET.transform.position - (Vector2)transform.position, transform.up);
                if (angleToTarget < 0) {
                    Body.rotation += MISSILE_TURN_RATE * Time.deltaTime;
                } else {
                    Body.rotation -= MISSILE_TURN_RATE * Time.deltaTime;
                }
            }
        }
    }

    private IEnumerator StartDeleteTimer() {
        yield return new WaitForSeconds(MISSILE_LIFESPAN);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Asteroid" || collision.gameObject.layer == LayerMask.NameToLayer("Ship") || collision.gameObject.layer == LayerMask.NameToLayer("Building")) {
            if (collision.tag == "Asteroid" || RelationShipManager.AreFactionsInWar(tag, collision.tag)) {
                collision.GetComponent<Hitpoints>().TakeDamage(DIRECT_DAMAGE, ORIGIN, tag);
                Explosion explosion = Instantiate(EXPLOSION, transform.position, Quaternion.identity).GetComponent<Explosion>();
                explosion.Explode(tag, EXPLOSION_RADIUS, EXPLOSION_DAMAGE, ORIGIN, 0.06f);
                Destroy(gameObject);

                if (collision.tag == "Asteroid" && DESTROYS_ASTEROIDS) Destroy(collision.gameObject);
            }
        }
    }
}
