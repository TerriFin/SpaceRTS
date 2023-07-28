using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactionCargoToImage : MonoBehaviour {

    private void Start() {
        GetComponent<Image>().sprite = FactionManager.PlayerFaction.mediumCargo.GetComponent<SpriteRenderer>().sprite;
    }

}
