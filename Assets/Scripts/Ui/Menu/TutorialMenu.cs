using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TutorialMenu : MonoBehaviour {
    public List<Sprite> TUTORIAL_IMAGES;
    public List<string> TUTORIAL_TEXTS;

    public TMP_Text TUTORIAL_TEXT_HOLDER;
    public Image TUTORIAL_IMAGE_HOLDER;

    public Button FORWARD_BUTTON;
    public Button BACKWARD_BUTTON;

    private int CurrentIndex;

    private void Start() {
        ResetTutorialMenu();
    }

    public void ResetTutorialMenu() {
        TUTORIAL_TEXT_HOLDER.text = TUTORIAL_TEXTS[0];
        TUTORIAL_IMAGE_HOLDER.sprite = TUTORIAL_IMAGES[0];
        CurrentIndex = 0;
        HandleMovementButtonActive();
    }

    public void MoveForward() {
        if (CurrentIndex + 1 < TUTORIAL_TEXTS.Count) {
            CurrentIndex += 1;
            TUTORIAL_TEXT_HOLDER.text = TUTORIAL_TEXTS[CurrentIndex];
            TUTORIAL_IMAGE_HOLDER.sprite = TUTORIAL_IMAGES[CurrentIndex];
            HandleMovementButtonActive();
        }
    }

    public void MoveBackwards() {
        if (CurrentIndex > 0) {
            CurrentIndex -= 1;
            TUTORIAL_TEXT_HOLDER.text = TUTORIAL_TEXTS[CurrentIndex];
            TUTORIAL_IMAGE_HOLDER.sprite = TUTORIAL_IMAGES[CurrentIndex];
            HandleMovementButtonActive();
        }
    }

    private void HandleMovementButtonActive() {
        if (CurrentIndex == 0) {
            BACKWARD_BUTTON.gameObject.SetActive(false);
            FORWARD_BUTTON.gameObject.SetActive(true);
        } else if (CurrentIndex == TUTORIAL_TEXTS.Count - 1) {
            BACKWARD_BUTTON.gameObject.SetActive(true);
            FORWARD_BUTTON.gameObject.SetActive(false);
        } else {
            BACKWARD_BUTTON.gameObject.SetActive(true);
            FORWARD_BUTTON.gameObject.SetActive(true);
        }
    }
}
