using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    private void Start() {
        MusicManager.SetVolume("sfxVolume", 1.0f);
        MusicManager.SetVolume("musicVolume", 1.0f);
        MusicManager.PlayTheme("Menu");
    }

    public void PlayGame() {
        print("STARTED GAME");
        FindObjectOfType<MapGeneratorManager>().WHERE_TO_RETURN_IN_MENU = "SkirmishMenu";
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame() {
        print("QUIT");
        Application.Quit();
    }
}
