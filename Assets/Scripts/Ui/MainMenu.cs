using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

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
