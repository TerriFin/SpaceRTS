using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayCommunication : MonoBehaviour, IScenarioEffect {

    public List<string> MESSAGES;
    public List<string> MESSAGES_FACTIONS;

    private CommunicationMenu communicationMenu;

    private void Start() {
        communicationMenu = FindObjectOfType<CommunicationMenu>();
    }

    public void Effect() {
        communicationMenu.StartCommunicationMenu(MESSAGES, MESSAGES_FACTIONS);
    }
}
