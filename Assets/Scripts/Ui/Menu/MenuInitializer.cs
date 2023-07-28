using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInitializer : MonoBehaviour {
    public List<GameObject> VIEWS;

    private void Start() {
        MapGeneratorManager mapGenerator = FindObjectOfType<MapGeneratorManager>();
        foreach (GameObject listedView in VIEWS) {
            listedView.SetActive(listedView.name == mapGenerator.WHERE_TO_RETURN_IN_MENU);
        }
    }
}
