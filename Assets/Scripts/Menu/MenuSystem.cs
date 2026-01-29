using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSystem : BaseSceneManager
{
    void Awake()
    {
        BaseAwake();
    }

    void Start()
    {
        BaseStart();
    }

    //Play Button
    public void Play()
    {
        ChangeScene("MapTest");
    }

    //Exit Button
    public void Exit()
    {
        if(!isTransitioning) StartCoroutine(ExitSequence());
    }

//-----------------------------------------------------------------------------------
//COROUTINES:
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
        Application.Quit();
    }
}