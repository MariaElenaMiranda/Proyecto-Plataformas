using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictorySystem : MonoBehaviour
{
    [Header("Interface")]
    public CanvasGroup blackScreen;
    public float fadeSpeed = 2.0f;

    [Header("Audio Settings")]
    public AudioSource backgroundMusic; // The music that plays when winning
    public AudioSource soundEffect; //For UI sounds
    public AudioClip hoverSound;
    public AudioClip clickSound;

    // Use 0.8 as max volume to match the GameplaySystem and avoid clipping
    private float defaultVolume = 0.8f;
    private bool isTransitioning = false; // Prevent double clicks

    void Start()
    {
        // Just in case the game finished in slow-motion, we must reset time to normal
        Time.timeScale = 1f;

        // Force loop just in case
        if(backgroundMusic != null)
        {
            backgroundMusic.loop = true;
            backgroundMusic.volume = 0; // Start silent for Fade In
        }

        if(blackScreen != null)
        {
            blackScreen.alpha = 1; // Set screen to black initially
            blackScreen.blocksRaycasts = false; // Allow clicks to pass through
            StartCoroutine(FadeInSequence()); // Start fading in
        }
    }

    public void NewGame()
    {
        // Restart the gameplay loop
        if(!isTransitioning) StartCoroutine(ChangeSceneSequence("MapTest"));
    }

    public void MainMenu()
    {
        // Go to Main Menu
        if(!isTransitioning) StartCoroutine(ChangeSceneSequence("MainMenu"));
    }

    // Hover Sound
    public void PlayHoverSound()
    {
         // Plays the UI hover sound effect
        if(hoverSound != null && soundEffect != null)
        {
            soundEffect.PlayOneShot(hoverSound);
        }
    }

//-----------------------------------------------------------------------------------
//COROUTINES:
    IEnumerator FadeInSequence()
    {
        // Ensure music starts playing
        if(backgroundMusic != null && !backgroundMusic.isPlaying) backgroundMusic.Play();

        float timer = 1;
        while(timer > 0)
        {
            // Reduces alpha from 1 (black) to 0 (transparent)
            timer -= Time.deltaTime * fadeSpeed;
            blackScreen.alpha = timer;

            // Fade in music to 0.8
            if(backgroundMusic != null)
            {
                backgroundMusic.volume = Mathf.Lerp(defaultVolume, 0, timer);
            }

            yield return null; // Wait for next frame
        }

        blackScreen.alpha = 0; // Ensure alpha is exactly 0
        blackScreen.blocksRaycasts = false;

        // Ensure the final volume is set correctly
        if(backgroundMusic != null) backgroundMusic.volume = defaultVolume;
    }

    IEnumerator FadeOutSequence()
    {
        // Block clicks during fade out
        if(blackScreen != null) blackScreen.blocksRaycasts = true;

        float timer = 0;
        while(timer < 1)
        {
            // Increases alpha from 0 (transparent) to 1 (black)
            timer += Time.deltaTime * fadeSpeed;
            blackScreen.alpha = timer;

            // Fade out music
            if(backgroundMusic != null)
            {
                backgroundMusic.volume = Mathf.Lerp(defaultVolume, 0, timer);
            }

            yield return null;
        }
        if(backgroundMusic != null) backgroundMusic.volume = 0f; // Silence
    }

    IEnumerator ChangeSceneSequence(string sceneName)
    {
        isTransitioning = true;
        Time.timeScale = 1f; // Important: Unpause time for animations/fades

        //Click sound:
        if(clickSound != null) soundEffect.PlayOneShot(clickSound);

        yield return StartCoroutine(FadeOutSequence()); // Wait for FadeOut to finish

        SceneManager.LoadScene(sceneName);
    }
}
