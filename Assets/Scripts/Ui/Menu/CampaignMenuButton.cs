using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CampaignMenuButton : MonoBehaviour {
    public List<string> REQUIRED_COMPLETED_LEVELS;
    public int LEVEL_INDEX;

    private void Start() {
        bool allRequiredLevelsComplete = true;
        foreach (string requiredLevel in REQUIRED_COMPLETED_LEVELS) {
            if (!PlayerPrefs.HasKey(requiredLevel)) {
                allRequiredLevelsComplete = false;
                break;
            }
        }

        Button button = GetComponent<Button>();
        button.interactable = allRequiredLevelsComplete;
        button.onClick.AddListener(PlayLevel);
    }

    public void PlayLevel() {
        print("STARTED LEVEL " + LEVEL_INDEX);
        FindObjectOfType<MapGeneratorManager>().WHERE_TO_RETURN_IN_MENU = "CampaignMenu";
        SceneManager.LoadScene(LEVEL_INDEX);
    }
}
