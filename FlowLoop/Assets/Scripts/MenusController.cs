using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
    This class handles all of the UI behaviour in the Main Menu.
 */

public class MenusController : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject levelsMenu;
    public GameObject howToPlayMenu;

    public Button continueButton;
    public Button[] levelButtons;

    void Start()
    {
        levelsMenu.SetActive(false);
        howToPlayMenu.SetActive(false);

        // Check if player has completed at least 1 level to enable or disable Continue button
        if (PlayerPrefs.HasKey("LevelsCompleted"))
        {
            int levelsCompleted = PlayerPrefs.GetInt("LevelsCompleted");
            if (levelsCompleted > 0)
            {
                continueButton.interactable = true;
            }
            else
            {
                continueButton.interactable = false;
            }
        }
        else
        {
            PlayerPrefs.SetInt("LevelsCompleted", 0);
            continueButton.interactable = false;
        }
            
    }


    public void OnContinueClick()
    {
        if (PlayerPrefs.HasKey("LevelsCompleted"))
        {
            // if player has completed level N, then levels up to and including N+1 are unlocked
            // so we load N+1 level
            int levelsCompleted = PlayerPrefs.GetInt("LevelsCompleted");
            if (levelsCompleted > 0 && levelsCompleted < 4)
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

        // set buttons of levels that are unlocked as interactable
        if (PlayerPrefs.HasKey("LevelsCompleted"))
        {
            int levelsCompleted = PlayerPrefs.GetInt("LevelsCompleted");
            if(levelsCompleted > 0)
            {
                // clamp value to avoid OutOfIndex error
                if (levelsCompleted >= levelButtons.Length)
                    levelsCompleted = levelButtons.Length-1;

                // reverse loop to enable levels before the highest unlocked level
                for (int i = levelsCompleted; i > 0; i--)
                {
                    levelButtons[i].interactable = true;
                }
            }
        }
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

    public void OnLevelClick(int idx)
    {
        SceneManager.LoadSceneAsync(idx);
    }
}
