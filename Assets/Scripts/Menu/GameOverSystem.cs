using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverSystem : BaseSceneManager
{
    void Awake()
    {
        BaseAwake(); // Run the parent logic first (Load prefs, Audio setup)
    }

    void Start()
    {
        BaseStart(); // Run parent start (Fade In sequence, Music play)
    }

    //-----------------------------------------------------------------------------------------
    // PUBLIC METHODS

    public void MainMenu()
    {
        // Calls the protected method from BaseSceneManager to handle the fade out
        ChangeScene("MainMenu"); // Return to Main Menu
    }

    public void RestartGame()
    {
        ChangeScene("MapTest"); // Reload the Level
    }
}