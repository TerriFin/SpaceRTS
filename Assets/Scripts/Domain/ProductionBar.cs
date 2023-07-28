using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionBar : MonoBehaviour {

    public float productionTime;
    public Transform origin;
    public GameObject bar;

    private float StartTime;

    private void Start() {
        transform.position = origin.transform.position + -transform.up;
        StartTime = Time.time;
    }

    private void Update() {
        if (origin != null) {
            bar.transform.localScale = new Vector3((Time.time - StartTime) / productionTime, 1, 1);
            transform.position = origin.transform.position + -transform.up;

            if (Time.time - StartTime >= productionTime || origin == null) {
                Destroy(gameObject);
            }
        } else {
            Destroy(gameObject);
        }
    }
}
