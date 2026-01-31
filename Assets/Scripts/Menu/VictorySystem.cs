using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictorySystem : BaseSceneManager
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
    public void NewGame()
    {
        // Reloads the game scene to start over
        ChangeScene("Map"); // Play Again Button
    }

    public void MainMenu()
    {
        // Calls the protected method from BaseSceneManager to handle the fade out
        ChangeScene("MainMenu"); // Return to Main Menu Button
    }
}