using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickIndicator : MonoBehaviour {

    public float ACTIVE_TIME;

    private void Start() {
        StartCoroutine(DeleteThisGameobjectAfterActiveTime());
    }

    private void Update() {
        transform.Rotate(0, 0, 1.8f);
    }

    IEnumerator DeleteThisGameobjectAfterActiveTime() {
        yield return new WaitForSeconds(ACTIVE_TIME);
        Destroy(gameObject);
    }
}
