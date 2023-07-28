using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyBar : MonoBehaviour {
    public float MONEY_TIME;
    public MoneyGenerator MONEY_GENERATOR;
    public GameObject bar;

    private float StartTime;

    private void Start() {
        StartTime = Time.time;
    }

    private void Update() {
        if (MONEY_GENERATOR != null) {
            bar.transform.localScale = new Vector3((Time.time - StartTime) / MONEY_TIME, 1, 1);
            transform.position = MONEY_GENERATOR.transform.position + -transform.up / 2;

            if (Time.time - StartTime >= MONEY_TIME || MONEY_GENERATOR == null) {
                Destroy(gameObject);
            }
        } else {
            Destroy(gameObject);
        }
    }
}
