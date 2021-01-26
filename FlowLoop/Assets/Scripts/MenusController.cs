using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenusController : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject levelsMenu;
    public GameObject howToPlayMenu;

    public Button continueButton;

    void Start()
    {
        levelsMenu.SetActive(false);
        howToPlayMenu.SetActive(false);

        // Check if player has completed at least 1 level to enable or disable Continue button
        if (PlayerPrefs.HasKey("LevelsCompleted"))
        {
            int levelsCompleted = PlayerPrefs.GetInt("LevelsCompleted");
            if (levelsCompleted >= 1)
            {
                Debug.Log("Continue button enabled: player has completed " + levelsCompleted + " levels!");
                continueButton.interactable = true;
            }
            else
            {
                continueButton.interactable = false;
            }
        }
        else
            continueButton.interactable = false;
    }


    public void OnContinueClick()
    {
        if (PlayerPrefs.HasKey("LevelsCompleted"))
        {
            // if player has completed level N, then levels up to and including N+1 are unlocked
            // so we load N+1 level
            int levelsCompleted = PlayerPrefs.GetInt("LevelsCompleted");
            if (levelsCompleted >= 1 && levelsCompleted < 4)
            {
                SceneManager.LoadSceneAsync(levelsCompleted + 1);
            }
            else if (levelsCompleted >= 4)
            {
                SceneManager.LoadSceneAsync(4);
            }
        }
    }

    public void OnSelectLevelsClick()
    {
        levelsMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void OnHowToPlayClick()
    {
        howToPlayMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void OnBackToMainMenu()
    {
        mainMenu.SetActive(true);
        levelsMenu.SetActive(false);
        howToPlayMenu.SetActive(false);
    }
}
