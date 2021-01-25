using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManagerController : MonoBehaviour
{
    // Hold the UI?


    // Manually set in the editor, it is the level's number
    public int level;

    // Save in Playerprefs that player has completed this level
    public void CompleteLevel()
    {
        int levelsCompleted = PlayerPrefs.GetInt("LevelsCompleted");
        if(levelsCompleted < level)
        {
            PlayerPrefs.SetInt("LevelsCompleted", level);
            PlayerPrefs.Save();
            Debug.Log("Player progress saved!");
        }
    }
}
