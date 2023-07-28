using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CommunicationMenu : MonoBehaviour {

    public static bool IS_PAUSED = false;

    public GameObject COMMUNICATION_MENU;
    public TMP_Text COMMUNICATION_MENU_TEXT;
    public Image COMMUNICATION_MENU_IMAGE;

    private string[] MESSAGES;
    private string[] MESSAGE_FACTIONS;
    private int index;

    private void Start() {
        MESSAGES = new string[0];
        MESSAGE_FACTIONS = new string[0];
        index = 0;
    }

    public void StartCommunicationMenu(List<string> messages, List<string> messageFactions) {
        MESSAGES = messages.ToArray();
        MESSAGE_FACTIONS = messageFactions.ToArray();
        index = 0;

        COMMUNICATION_MENU.SetActive(true);
        UpdateCommunicationMenu();
        AudioListener.volume = 0f;
        Time.timeScale = 0f;
        IS_PAUSED = true;
    }

    public void Continue() {
        index++;
        if (index >= MESSAGES.Length) {
            COMMUNICATION_MENU.SetActive(false);
            AudioListener.volume = 1f;
            Time.timeScale = 1f;
            IS_PAUSED = false;
        } else UpdateCommunicationMenu();
    }

    private void UpdateCommunicationMenu() {
        COMMUNICATION_MENU_TEXT.text = MESSAGES[index];
        Color textColor = FactionManager.Factions[MESSAGE_FACTIONS[index]].factionColor;
        textColor.a = 1.0f;
        COMMUNICATION_MENU_TEXT.color = textColor;
        COMMUNICATION_MENU_IMAGE.sprite = FactionManager.Factions[MESSAGE_FACTIONS[index]].factionLeader;
    }
}
