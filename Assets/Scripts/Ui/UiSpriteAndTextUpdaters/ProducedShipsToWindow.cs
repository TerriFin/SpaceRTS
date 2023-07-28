using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProducedShipsToWindow : MonoBehaviour {

    public Text producedShips;
    public Text maxProducedShips;

    private Production Production;

    private void Start() {
        Production = SelectionManager.selected[0].GetComponent<Production>();
        maxProducedShips.text = Production.productionLimit + "";
    }

    private void Update() {
        producedShips.text = Production.CurrentShips + "";
    }
}
