using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    public bool ACTIVE;
    public float WAIT_TIME;
    public GameObject TEST_OBJECT;

    private void Start() {
        if (ACTIVE) StartCoroutine(Timer());
    }

    private IEnumerator Timer() {
        while (true) {
            yield return new WaitForSeconds(WAIT_TIME);
            foreach (Faction faction in FactionManager.Factions.Values) {
                print(faction.factionTag + ": " + FactionManager.FactionScoresManager.FactionAssetScores[faction.factionTag]);
            }
            //float test = Mathf.Deg2Rad * Vector2.SignedAngle(TEST_OBJECT.transform.position, transform.position);
            //print(new Vector2(Mathf.Cos(test), Mathf.Sin(test)));
        }
    }
}
