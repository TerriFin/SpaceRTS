using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelationShipWindowManager : MonoBehaviour {

    public GameObject relationShipWindowPrefab;

    private void Start() {
        List<Faction> factionsWithoutPlayer = new List<Faction>();
        foreach (Faction faction in FactionManager.Factions.Values) {
            print(faction.factionTag);
            if (FactionManager.PlayerFaction.factionTag != faction.factionTag) {
                factionsWithoutPlayer.Add(faction);
            }
        }

        int howMany = 0;
        foreach (Faction faction in factionsWithoutPlayer) {
            GameObject relationShip = Instantiate(relationShipWindowPrefab, transform);
            relationShip.GetComponent<RelationShipWindow>().Faction = faction;
            relationShip.GetComponent<RectTransform>().anchoredPosition = new Vector2(-150 + (-250 * howMany), 0);
            print(relationShip.transform.position); // Without this the relationship view breaks on mobile... no idea why.
            howMany++;
        }
    }
}
