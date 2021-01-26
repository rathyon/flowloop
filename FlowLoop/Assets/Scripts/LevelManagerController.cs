using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManagerController : MonoBehaviour
{
    public Button prevLevelButton;
    public Button nextLevelButton;

    // Manually set in the editor, it is the level's number
    public int currentLevel;

    void Start()
    {
        if(currentLevel > 1)
        {
            prevLevelButton.interactable = true;
        }

        if(currentLevel < 4)
        {
            if (PlayerPrefs.HasKey("LevelsCompleted"))
            {
                int levelsCompleted = PlayerPrefs.GetInt("LevelsCompleted");
                if (levelsCompleted >= currentLevel)
                {
                    nextLevelButton.interactable = true;
                }
                    
            }   
        }
    }

    // Save in Playerprefs that player has completed this level and unlock next level button
    public void CompleteLevel()
    {
        Debug.Log("Level complete!");

        if (PlayerPrefs.HasKey("LevelsCompleted"))
        {
            int levelsCompleted = PlayerPrefs.GetInt("LevelsCompleted");
            if (levelsCompleted < currentLevel)
            {
                PlayerPrefs.SetInt("LevelsCompleted", currentLevel);
                PlayerPrefs.Save();
                Debug.Log("Player progress saved!");

                // if its not last level(4), unlock next lvl button
                if (!nextLevelButton.interactable && currentLevel < 4)
                    nextLevelButton.interactable = true;
            }
        }
    }

    public void OnNextClick()
    {
        SceneManager.LoadSceneAsync(currentLevel + 1);
    }

    public void OnPrevClick()
    {
        SceneManager.LoadSceneAsync(currentLevel - 1);
    }

    public void OnMainMenuClick()
    {
        SceneManager.LoadSceneAsync(0);
    }
}
