using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseSceneManager : MonoBehaviour
{
    [Header("Visual Settings")]
    public CanvasGroup blackScreen; // The black panel used for fading in/out
    public float fadeSpeed = 2.0f; // Speed of the fade transition
    public TextMeshProUGUI musicButtonText; // Reference to the UI text that shows ON/OFF

    [Header("Audio Settings")]
    public AudioSource backgroundMusic; // Chanel for plays the level loop.
    public AudioSource soundEffect; // Channel for short sounds (UI clicks)
    public AudioClip hoverSound; // The sound file for hovering over a button
    public AudioClip clickSound; // The sound file for clicking a button

    [Header("Settings")]
    protected float defaultVolume = 0.8f; // Safety limit for volume
    protected bool isTransitioning = false; // Flag to block input during scene changes
    protected static bool isMusicOn; // Remembers if music is on across all scenes

//-----------------------------------------------------------------------------------------
//UNITY EVENTS
    void Awake()
    {
        BaseAwake(); // Executes the setup logic defined below
    }

    void Start()
    {
        BaseStart(); // Executes the startup logic defined below
    }

//-----------------------------------------------------------------------------------------
// PUBLIC METHODS

// Called by the Music Button
    public void ToggleMusic()
    {
       // Invert the boolean (True -> False / False -> True)
        isMusicOn = !isMusicOn;

        // Save to memory so it remembers next time
        PlayerPrefs.SetInt("MusicEnabled", isMusicOn ? 1 : 0);
        PlayerPrefs.Save();

        // Apply changes immediately
        ApplyMusicSettings();
        UpdateMusicButtonText();
    }

    // Called by Event Trigger (Pointer Enter)
    public void PlayHoverSound()
    {
        // Plays the UI hover sound effect
        if(hoverSound != null && soundEffect != null)
        {
            soundEffect.PlayOneShot(hoverSound); // PlayOneShot allows overlapping sounds
        }
    }

    public void PlayClickSound()
    {
        // Plays the UI click sound effect
        if(clickSound != null && soundEffect != null)
        {
            soundEffect.PlayOneShot(clickSound); // PlayOneShot allows overlapping sounds
        }
    }

//-----------------------------------------------------------------------------------------
// PROTECTED METHODS: These methods are protected so "Children" scripts can use them.
    protected void BaseAwake()
    {
        // Load saved data: (1 = Music On / 0 = Music Off)
        isMusicOn = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;

        // Configure the AudioSource Mute state
        if (backgroundMusic != null) {
            // If music is ON, Mute is OFF (and vice versa)
            backgroundMusic.mute = !isMusicOn;

            // If allowed, set the volume ready
            if (isMusicOn) backgroundMusic.volume = defaultVolume;
        }
    }

    protected void BaseStart()
    {
        // Ensure game runs at normal speed (Fixes paused game bugs)
        Time.timeScale = 1f;

        // Setup Music Loop
        if(backgroundMusic != null)
        {
            backgroundMusic.loop = true;

            // Only play if the user wants music
            if(isMusicOn)
            {
                backgroundMusic.volume = 0; // Start silent for Fade In effect
                backgroundMusic.Play();
            }
        }

        // Setup Black Screen Fade In
        if(blackScreen != null)
        {
            blackScreen.alpha = 1; // Start fully black
            blackScreen.blocksRaycasts = false; // Let clicks pass through
            StartCoroutine(FadeInSequence()); // Begin animation
        }
        // Update the text on screen
        UpdateMusicButtonText();
    }
    protected void UpdateMusicButtonText()
    {
        // Updates the button text label
        if(musicButtonText != null)
        {
            musicButtonText.text = isMusicOn ? "MUSIC: ON" : "MUSIC: OFF";
        }
    }

    // Applies the music state to the AudioSource
    protected void ApplyMusicSettings()
    {
        if(backgroundMusic != null)
        {
            backgroundMusic.mute = !isMusicOn;
            // Special Case: User turned music on while playing
            if(isMusicOn)
            {
                // If it wasn't playing, start it now
                if(!backgroundMusic.isPlaying) backgroundMusic.Play();
                backgroundMusic.volume = defaultVolume;
            }
        }
    }

    protected void ChangeScene(string sceneName)
    {
        // Logic for changing scenes (Win/Lose screens)
        // Only start if not already transitioning
        if(!isTransitioning) StartCoroutine(ChangeSceneSequence(sceneName));
    }

    //-----------------------------------------------------------------------------------
    //COROUTINES

    // Fade IN: Black -> Transparent
    protected IEnumerator FadeInSequence()
    {
        float timer = 1; // 1 = Opaque (Black)
        while(timer > 0)
        {
            // Reduces alpha from 1 (black) to 0 (transparent)
            timer -= Time.deltaTime * fadeSpeed;

            // Apply transparency to black screen
            if(blackScreen != null) blackScreen.alpha = timer;

            // Fade in Music Volume (0 -> 0.8)
            if(backgroundMusic != null && isMusicOn)
            {
                // Lerp makes a smooth transition
                backgroundMusic.volume = Mathf.Lerp(defaultVolume, 0, timer);
            }

            yield return null; // Wait for next frame
        }

        // Final Cleanup: Ensure it's invisible
        if(blackScreen != null)
        {
            blackScreen.alpha = 0;
            blackScreen.blocksRaycasts = false;
        }

        // Ensure exact volume at the end
        if(backgroundMusic != null && isMusicOn) backgroundMusic.volume = defaultVolume;
    }

    // Fade OUT: Transparent -> Black
    protected IEnumerator FadeOutSequence()
    {
        // Block clicks during fade out
        if(blackScreen != null) blackScreen.blocksRaycasts = true;

        float timer = 0; // 0 = Transparent
        while(timer < 1)
        {
            // Increases alpha from 0 (transparent) to 1 (black)
            timer += Time.deltaTime * fadeSpeed;

            // Apply black screen
            if(blackScreen != null) blackScreen.alpha = timer;

            // Fade out Music Volume (0.8 -> 0)
            if(backgroundMusic != null && isMusicOn)
            {
                backgroundMusic.volume = Mathf.Lerp(defaultVolume, 0, timer);
            }

            yield return null; // Wait for next frame
        }

        // Silence music completely before switching scene
        if(backgroundMusic != null) backgroundMusic.volume = 0f;
    }

    // The full sequence to change scenes
    protected IEnumerator ChangeSceneSequence(string sceneName)
    {
        isTransitioning = true;
        Time.timeScale = 1f; // Ensure animations run

        //Click sound:
        PlayClickSound();

        yield return StartCoroutine(FadeOutSequence()); // Wait for FadeOut to finish

        SceneManager.LoadScene(sceneName); // Load the scene
    }
}

