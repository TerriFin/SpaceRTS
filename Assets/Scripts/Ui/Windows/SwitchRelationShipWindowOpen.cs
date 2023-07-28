using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchRelationShipWindowOpen : MonoBehaviour {

    public GameObject relationShipHolder;
    public GameObject otherButton;

    private void Start() {
        GetComponent<Button>().onClick.AddListener(SwitchRelationShipHolder);
    }

    private void SwitchRelationShipHolder() {
        gameObject.SetActive(false);
        otherButton.SetActive(true);
        relationShipHolder.SetActive(!relationShipHolder.activeSelf);
    }
}
