using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collideable : MonoBehaviour {

    private Rigidbody2D Body;

    private void Start() {
        Body = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ship")) {
            Vector2 rotationVector = (transform.position - collision.transform.position).normalized;
            Body.AddForce(rotationVector);
        }
    }
}
