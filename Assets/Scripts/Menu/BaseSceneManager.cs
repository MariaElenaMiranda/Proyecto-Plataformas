using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseSceneManager : MonoBehaviour
{
    [Header("Visual Settings")]
    public CanvasGroup blackScreen;
    public float fadeSpeed = 2.0f;
    public TextMeshProUGUI musicButtonText; // Reference to the music button text

    [Header("Audio Settings")]
    public AudioSource backgroundMusic; // The music that plays when losing
    public AudioSource soundEffect; //For UI sounds
    public AudioClip hoverSound;
    public AudioClip clickSound;

    [Header("Settings")]
    protected float defaultVolume = 0.8f; // Safety limit for volume (0.8 matches the rest of the game)
    protected bool isTransitioning = false; // Flag to prevent double actions during transitions
    protected bool isMusicOn; // We store the state here so both Awake and Start can use it.

    protected void BaseAwake()
    {
        // Load saved data: (1 = Music On / 0 = Music Off)
        isMusicOn = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;

        // Apply silence immediately
        // If Music is ON (true) -> Mute must be OFF (false)
        if (backgroundMusic != null) backgroundMusic.mute = !isMusicOn;
    }

    protected void BaseStart()
    {
        // Ensure game runs at normal speed
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
        UpdateMusicButtonText();
    }

    public void ToggleMusic()
    {
        // Invert state
        isMusicOn = !isMusicOn;

        // Save
        PlayerPrefs.SetInt("MusicEnabled", isMusicOn ? 1 : 0);
        PlayerPrefs.Save();

        // Apply to both channels
        if(backgroundMusic != null) backgroundMusic.mute = !isMusicOn;

        // Update UI
        UpdateMusicButtonText();
    }

    protected void UpdateMusicButtonText()
    {
        // Update text based on state
        if(musicButtonText != null)
        {
            musicButtonText.text = isMusicOn ? "MUSIC: ON" : "MUSIC: OFF";
        }
    }

    protected void PlayHoverSound()
    {
        // Plays the UI hover sound effect
        if(hoverSound != null && soundEffect != null)
        {
            soundEffect.PlayOneShot(hoverSound);
        }
    }

    protected void PlayClickSound()
    {
        // Plays the UI click sound effect
        if(clickSound != null && soundEffect != null)
        {
            soundEffect.PlayOneShot(clickSound);
        }
    }
    
    protected void ChangeScene(string sceneName)
    {
        // Logic for changing scenes (Win/Lose screens)
        if(!isTransitioning) StartCoroutine(ChangeSceneSequence(sceneName));
    }

    //-----------------------------------------------------------------------------------
    //COROUTINES:

    protected IEnumerator FadeInSequence()
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

    protected IEnumerator FadeOutSequence()
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

    protected IEnumerator ChangeSceneSequence(string sceneName)
    {
        isTransitioning = true;
        Time.timeScale = 1f; // Important: Unpause time for animations/fades

        //Click sound:
        PlayClickSound();

        yield return StartCoroutine(FadeOutSequence()); // Wait for FadeOut to finish

        SceneManager.LoadScene(sceneName);
    }
}

