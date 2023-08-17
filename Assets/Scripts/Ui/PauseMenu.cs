using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    public static bool IS_PAUSED = false;
    public static bool CAN_PAUSE = true;

    public GameObject PAUSE_MENU_UI;
    private RectTransform PauseMenuRect;

    private void Start() {
        PauseMenuRect = PAUSE_MENU_UI.GetComponent<RectTransform>();
        Resume();
    }

    private void Update() {
        if (Input.GetKeyUp(KeyCode.Escape) && CAN_PAUSE) {
            if (IS_PAUSED) Resume();
            else Pause();
            PauseMenuRect.SetAsLastSibling();
        }
    }

    private void Pause() {
        IS_PAUSED = true;
        PAUSE_MENU_UI.SetActive(true);
        MusicManager.SetVolume("sfxVolume", 0.0f);
        MusicManager.SetVolume("musicVolume", 0.75f);
        Time.timeScale = 0f;
    }

    public void Resume() {
        IS_PAUSED = false;
        PAUSE_MENU_UI.SetActive(false);
        if (!CommunicationMenu.IS_PAUSED) {
            MusicManager.SetVolume("sfxVolume", 1.0f);
            MusicManager.SetVolume("musicVolume", 1.0f);
            Time.timeScale = 1f;
        }
    }

    public void Quit() {
        MusicManager.SetVolume("sfxVolume", 1.0f);
        MusicManager.SetVolume("musicVolume", 1.0f);
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
