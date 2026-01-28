using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour
{
    [Header("Interface")]
    public CanvasGroup blackScreen;
    public float fadeSpeed = 2.0f;

    [Header("Audio Settings")]
    public AudioSource backgroundMusic; // For background music
    public AudioSource soundEffect; // For UI sounds
    public AudioClip hoverSound;
    public AudioClip clickSound;

    private float defaultVolume = 0.8f; // Use 0.8 as max volume to match the GameplaySystem and avoid clipping
    private bool isTransitioning = false; // Flag to prevent double actions during transitions

    void Start()
    {
        // Force loop just in case
        if(backgroundMusic != null) backgroundMusic.loop = true;

        if(blackScreen != null)
        {
            blackScreen.alpha = 1; // Set screen to black initially
            blackScreen.blocksRaycasts = false; // Allow clicks to pass through
            StartCoroutine(FadeInSequence()); // Start fading in
        }
        // Ensure game runs at normal speed
        Time.timeScale = 1f;
    }

    //Play Button
    public void Play()
    {
        // Check if we are already changing scenes
        if(!isTransitioning) StartCoroutine(ChangeSceneSequence("MapTest"));
    }


    //Exit Button
    public void Exit()
    {
        // Start the exit sequence with sound
        StartCoroutine(ExitSequence());
    }

    //Hover
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

            // Fade in music smoothly respecting the 0.8 limit
            if(backgroundMusic != null)
            {
                // Lerp from 0 to 0.8 based on the timer
                backgroundMusic.volume = Mathf.Lerp(defaultVolume, 0, timer);
            }

            yield return null;
        }

        blackScreen.alpha = 0; // Ensure alpha is exactly 0
        blackScreen.blocksRaycasts = false;

        if(backgroundMusic != null) backgroundMusic.volume = defaultVolume; // Max volume
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

            // Fade out music smoothly
            if(backgroundMusic != null)
            {
                // Lerp from 0.8 to 0
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

    IEnumerator ExitSequence()
    {
        isTransitioning = true;
        // Ensure time runs (consistency with PauseSystem)
        Time.timeScale = 1f;

        // Play the click sound immediately
        if(clickSound != null && soundEffect != null)
        {
            soundEffect.PlayOneShot(clickSound);
        }
        // This ensures the screen turns black and music fades out smoothly before quitting.
        yield return StartCoroutine(FadeOutSequence()); // Wait for FadeOut to finish

        Debug.Log("Exiting game...");
        Application.Quit();
    }
}

