using System.Collections;
using UnityEngine;

public class MenuSystem : BaseSceneManager
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

    public void Play()
    {
        //Play Button
        ChangeScene("Map"); // Load the game scene
    }

    public void Exit()
    {
        //Exit Button
        // Only start if not already exiting
        if(!isTransitioning) StartCoroutine(ExitSequence());
    }

//---------------------------------------------------------------------------------------------
//COROUTINES

    // Custom sequence to quit the game smoothly
    IEnumerator ExitSequence()
    {
        isTransitioning = true;
        // Ensure time runs (consistency with PauseSystem)
        Time.timeScale = 1f;

        // Play the click sound immediately
        PlayClickSound();

        // This ensures the screen turns black and music fades out smoothly before quitting.
        yield return StartCoroutine(FadeOutSequence()); // Wait for FadeOut to finish

        Debug.Log("Exiting game...");
        Application.Quit(); // Close application
    }
}