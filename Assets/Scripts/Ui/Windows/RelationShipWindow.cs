using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RelationShipWindow : MonoBehaviour {

    public Faction Faction { get; set; }

    public float UPDATE_TIMER;
    public TMP_Text factionName;
    public Image factionLogo;
    public Button declareWarButton;
    public Button declarePeaceButton;
    public Button startBlockadeButton;
    public Button endBlockadeButton;
    public Image otherFactionBlockadeStatus;
    public Image otherFactionWarStatus;

    private Image DeclareWarButtonImage;
    private Image DeclarePeaceButtonImage;
    private Image StartBlockadeButtonImage;
    private Image EndBlockadeButtonImage;

    private FactionScoresManager ScoresManager;

    private void Start() {
        DeclareWarButtonImage = declareWarButton.GetComponentsInChildren<Image>()[1];
        DeclarePeaceButtonImage = declarePeaceButton.GetComponentsInChildren<Image>()[1];
        StartBlockadeButtonImage = startBlockadeButton.GetComponentsInChildren<Image>()[1];
        EndBlockadeButtonImage = endBlockadeButton.GetComponentsInChildren<Image>()[1];

        factionName.text = Faction.factionTag;
        Color factionColor = Faction.factionColor;
        factionColor.a = 1;
        factionName.color = factionColor;
        factionLogo.sprite = Faction.factionLogo;

        ScoresManager = FindObjectOfType<FactionScoresManager>();

        StartCoroutine(UpdateRelationshipStatus());
    }

    private void OnEnable() {
        if (Faction != null) StartCoroutine(UpdateRelationshipStatus());
    }

    private IEnumerator UpdateRelationshipStatus() {
        while (true) {
            if (!ScoresManager.IsFactionInGame(Faction.factionTag)) {
                Destroy(this.gameObject);
            } else {
                // Other faction status
                if (RelationShipManager.IsFactionBlockadingFaction((string)Faction.factionTag, (string)FactionManager.PlayerFaction.factionTag)) {
                    otherFactionBlockadeStatus.enabled = true;
                } else {
                    otherFactionBlockadeStatus.enabled = false;
                }
                if (RelationShipManager.IsFactionAttackingFaction((string)Faction.factionTag, (string)FactionManager.PlayerFaction.factionTag)) {
                    otherFactionWarStatus.enabled = true;
                } else {
                    otherFactionWarStatus.enabled = false;
                }

                if (!RelationShipManager.Locked) {
                    // Our status
                    if (RelationShipManager.IsFactionAttackingFaction((string)FactionManager.PlayerFaction.factionTag, (string)Faction.factionTag)) {
                        declareWarButton.enabled = false;
                        DeclareWarButtonImage.color = new Color(0.25f, 0.25f, 0.25f, 0.75f);
                    } else {
                        declareWarButton.enabled = true;
                        DeclareWarButtonImage.color = new Color(1, 1, 1, 1);
                    }

                    if (!RelationShipManager.IsFactionAttackingFaction((string)FactionManager.PlayerFaction.factionTag, (string)Faction.factionTag)) {
                        declarePeaceButton.enabled = false;
                        DeclarePeaceButtonImage.color = new Color(0.25f, 0.25f, 0.25f, 0.75f);
                    } else {
                        declarePeaceButton.enabled = true;
                        DeclarePeaceButtonImage.color = new Color(1, 1, 1, 1);
                    }

                    if (RelationShipManager.IsFactionBlockadingFaction((string)FactionManager.PlayerFaction.factionTag, (string)Faction.factionTag)) {
                        startBlockadeButton.enabled = false;
                        StartBlockadeButtonImage.color = new Color(0.25f, 0.25f, 0.25f, 0.75f);
                    } else {
                        startBlockadeButton.enabled = true;
                        StartBlockadeButtonImage.color = new Color(1, 1, 1, 1);
                    }

                    if (!RelationShipManager.IsFactionBlockadingFaction((string)FactionManager.PlayerFaction.factionTag, (string)Faction.factionTag)) {
                        endBlockadeButton.enabled = false;
                        EndBlockadeButtonImage.color = new Color(0.25f, 0.25f, 0.25f, 0.75f);
                    } else {
                        endBlockadeButton.enabled = true;
                        EndBlockadeButtonImage.color = new Color(1, 1, 1, 1);
                    }
                } else {
                    declareWarButton.enabled = false;
                    DeclareWarButtonImage.color = new Color(0.25f, 0.25f, 0.25f, 0.75f);

                    declarePeaceButton.enabled = false;
                    DeclarePeaceButtonImage.color = new Color(0.25f, 0.25f, 0.25f, 0.75f);

                    startBlockadeButton.enabled = false;
                    StartBlockadeButtonImage.color = new Color(0.25f, 0.25f, 0.25f, 0.75f);

                    endBlockadeButton.enabled = false;
                    EndBlockadeButtonImage.color = new Color(0.25f, 0.25f, 0.25f, 0.75f);
                }
            }

            yield return new WaitForSeconds(UPDATE_TIMER);
        }
    }

    public void StartWar() {
        RelationShipManager.StartWar((string)FactionManager.PlayerFaction.factionTag, (string)Faction.factionTag);
    }

    public void EndWar() {
        RelationShipManager.EndWar((string)FactionManager.PlayerFaction.factionTag, (string)Faction.factionTag);
    }

    public void StartBlockade() {
        RelationShipManager.StartBlockade((string)FactionManager.PlayerFaction.factionTag, (string)Faction.factionTag);
    }

    public void EndBlockade() {
        RelationShipManager.EndBlockade((string)FactionManager.PlayerFaction.factionTag, (string)Faction.factionTag);
    }
}
