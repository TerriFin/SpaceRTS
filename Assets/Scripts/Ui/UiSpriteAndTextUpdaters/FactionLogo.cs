using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactionLogo : MonoBehaviour {
    private void Start() {
        gameObject.GetComponent<Image>().sprite = FactionManager.PlayerFaction.factionLogo;
    }
}
