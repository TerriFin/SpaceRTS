using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class RelationShipWindow : MonoBehaviour {

    public Faction Faction { get; set; }

    public float UPDATE_TIMER;
    public TMP_Text factionName;
    public TMP_Text factionMoneyAmount;
    public TMP_Text diplomacyDisabledText;
    public Image factionLogo;
    public Button warButton;
    public Image warButtonImage;
    public Button blockadeButton;
    public Image blockadeButtonImage;
    public Image otherFactionBlockadeStatus;
    public Image otherFactionWarStatus;
    public Sprite declareWarImage;
    public Sprite endWarImage;
    public Sprite declareBlockadeImage;
    public Sprite endBlockadeImage;

    private FactionScoresManager ScoresManager;

    private void Start() {
        factionName.text = Faction.factionTag;
        Color factionColor = Faction.factionColor;
        factionColor.a = 1;
        factionName.color = factionColor;
        factionMoneyAmount.color = factionColor;
        diplomacyDisabledText.color = factionColor;
        factionLogo.sprite = Faction.factionLogo;

        if (!RelationShipManager.Locked) Destroy(diplomacyDisabledText.gameObject);

        ScoresManager = FindObjectOfType<FactionScoresManager>();

        warButton.onClick.AddListener(HandleWarClick);
        blockadeButton.onClick.AddListener(HandleBlockadeClick);

        StartCoroutine(UpdateRelationshipStatus());
    }

    private void OnEnable() {
        if (Faction != null) StartCoroutine(UpdateRelationshipStatus());
    }

    private IEnumerator UpdateRelationshipStatus() {
        while (true) {
            if (!FactionManager.Factions.ContainsKey(Faction.factionTag)) {
                Destroy(this.gameObject);
                break;
            } else {
                // Other faction status
                if (RelationShipManager.IsFactionBlockadingFaction(Faction.factionTag, FactionManager.PlayerFaction.factionTag)) {
                    otherFactionBlockadeStatus.enabled = true;
                } else {
                    otherFactionBlockadeStatus.enabled = false;
                }
                if (RelationShipManager.IsFactionAttackingFaction(Faction.factionTag, FactionManager.PlayerFaction.factionTag)) {
                    otherFactionWarStatus.enabled = true;
                } else {
                    otherFactionWarStatus.enabled = false;
                }

                if (!RelationShipManager.Locked) {
                    // Our status
                    if (RelationShipManager.IsFactionAttackingFaction(FactionManager.PlayerFaction.factionTag, Faction.factionTag)) {
                        warButtonImage.sprite = endWarImage;
                    } else {
                        warButtonImage.sprite = declareWarImage;
                    }

                    if (RelationShipManager.IsFactionBlockadingFaction(FactionManager.PlayerFaction.factionTag, Faction.factionTag)) {
                        blockadeButtonImage.sprite = endBlockadeImage;
                    } else {
                        blockadeButtonImage.sprite = declareBlockadeImage;
                    }
                } else {
                    warButton.interactable = false;
                    warButtonImage.color = new Color(1, 1, 1, 0.5f);
                    blockadeButton.interactable = false;
                    blockadeButtonImage.color = new Color(1, 1, 1, 0.5f);
                }
            }

            factionMoneyAmount.SetText(string.Format("{0:C0}", FactionManager.Factions[Faction.factionTag].money));

            yield return new WaitForSeconds(UPDATE_TIMER);
        }
    }

    private void HandleWarClick() {
        if (RelationShipManager.IsFactionAttackingFaction(FactionManager.PlayerFaction.factionTag, Faction.factionTag)) {
            RelationShipManager.EndWar(FactionManager.PlayerFaction.factionTag, Faction.factionTag);
        } else {
            RelationShipManager.StartWar(FactionManager.PlayerFaction.factionTag, Faction.factionTag);
        }
    }

    private void HandleBlockadeClick() {
        if (RelationShipManager.IsFactionBlockadingFaction(FactionManager.PlayerFaction.factionTag, Faction.factionTag)) {
            RelationShipManager.EndBlockade(FactionManager.PlayerFaction.factionTag, Faction.factionTag);
        } else {
            RelationShipManager.StartBlockade(FactionManager.PlayerFaction.factionTag, Faction.factionTag);
        }
    }
}
