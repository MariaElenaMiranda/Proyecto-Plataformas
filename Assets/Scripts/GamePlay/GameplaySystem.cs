using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplaySystem : MonoBehaviour
{
    [Header("Visual Settings")]
    public TextMeshProUGUI musicButtonText; // Reference to the music button text
    public CanvasGroup blackScreen;
    public float fadeSpeed = 2.0f;

    [Header("Audio Settings")]
    public AudioSource mainThemeMusic;
    public AudioSource bossThemeMusic; // The second source used for crossfading
    public AudioLowPassFilter mainFilter; // Filtro del Bosque
    public AudioLowPassFilter bossFilter;  // Filtro del Boss
    public float pausedFrequency = 500f;// Sounds like being underwater
    private float defaultFrequency = 22000f; // Fully open, clean sound
    public float pausedVolume = 0.3f;
    private float defaultVolume = 0.8f; // I set this to 0.8 to avoid audio clipping when the filter is active

    private bool isTransitioning = false; // Flag to prevent double actions during transitions
    private bool isMusicOn;

    void Awake()
    {
        // Load saved data: (1 = Music On / 0 = Music Off)
        isMusicOn = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;

        // Apply silence immediately to both sources
        if (mainThemeMusic != null) mainThemeMusic.mute = !isMusicOn;
        if (bossThemeMusic != null) bossThemeMusic.mute = !isMusicOn;
    }

    void Start()
    {
        // Update Visuals
        UpdateMusicButtonText(isMusicOn);

        // Always make sure the game starts with clean audio
        if(mainFilter != null) mainFilter.cutoffFrequency = defaultFrequency;
        if(bossFilter != null) bossFilter.cutoffFrequency = defaultFrequency;

        if(mainThemeMusic != null)
        {
            mainThemeMusic.loop = true;
            mainThemeMusic.volume = 0; // Start at 0 volume for the Fade In effect

            if(!mainThemeMusic.isPlaying) mainThemeMusic.Play();
        }

        if(blackScreen != null)
        {
            blackScreen.alpha = 1; // Start with black screen
            blackScreen.blocksRaycasts = false; // Allow clicks to pass through
            StartCoroutine(FadeInSequence()); // Start fading in
        }
    }

    public void ToggleMusic()
    {
        // Invert state
        isMusicOn = !isMusicOn;

        // Save
        PlayerPrefs.SetInt("MusicEnabled", isMusicOn ? 1 : 0);
        PlayerPrefs.Save();

        // Apply to both channels
        if(mainThemeMusic != null) mainThemeMusic.mute = !isMusicOn;
        if(bossThemeMusic != null) bossThemeMusic.mute = !isMusicOn;

        // Update UI
        UpdateMusicButtonText(isMusicOn);
    }

    void UpdateMusicButtonText(bool isOn)
    {
        // Update text based on state
        if(musicButtonText != null)
        {
            musicButtonText.text = isOn ? "MUSIC: ON" : "MUSIC: OFF";
        }
    }

    //to switch the background music (Boss Fight)
    public void SwitchMusic(AudioClip newMusicClip)
    {
        // Only start the transition if the song is actually different
        if(mainThemeMusic.clip != newMusicClip)
        {
            StartCoroutine(SwapMusic(newMusicClip));
        }
    }

    // Handles the audio changes when the game is paused
    public void NotifyPause(bool isPaused)
    {
        // Determine the target volume and frequency based on the pause state
        float targetVolume = isPaused ? pausedVolume : defaultVolume;
        float targetFrequency = isPaused ? pausedFrequency : defaultFrequency;

        // Start the coroutine to smooth the transition for both values
        StartCoroutine(FadeAudioEffect(targetVolume, targetFrequency));
    }

    // Logic for changing scenes (Win/Lose screens)
    public void ChangeScene(string sceneName)
    {
        if(!isTransitioning) StartCoroutine(ChangeSceneSequence(sceneName));
    }


//-----------------------------------------------------------------------------------
//COROUTINES:
    IEnumerator FadeInSequence()
    {
        // Ensure music starts playing
        if(mainThemeMusic != null && !mainThemeMusic.isPlaying) mainThemeMusic.Play();

        float timer = 1;
        while(timer > 0)
        {
            // Decrease alpha from 1 (black) to 0 (transparent)
            timer -= Time.deltaTime * fadeSpeed;
            blackScreen.alpha = timer;

            // Increase volume smoothly
            if(mainThemeMusic != null)
            {
                // Using Lerp to ensure we stop exactly at 0.8 (defaultVolume)
                // When timer is 1 -> volume is 0. When timer is 0 -> volume is 0.8
                mainThemeMusic.volume = Mathf.Lerp(defaultVolume, 0, timer);
            }

            yield return null; // Wait for next frame
        }

        blackScreen.alpha = 0; // Ensure alpha is exactly 0
        blackScreen.blocksRaycasts = false;

        // Ensure the final volume is set correctly
        if(mainThemeMusic != null) mainThemeMusic.volume = defaultVolume;
    }

    IEnumerator FadeOutSequence()
    {
        // Block player input during the fade out
        if(blackScreen != null) blackScreen.blocksRaycasts = true;

        float timer = 0;
        while(timer < 1)
        {
            // Increase alpha from 0 (transparent) to 1 (black)
            timer += Time.deltaTime * fadeSpeed;
            blackScreen.alpha = timer;

            // Lower the music volume
            if(mainThemeMusic != null)
            {
                mainThemeMusic.volume = Mathf.Lerp(defaultVolume, 0, timer);
            }

            yield return null;
        }
        if(mainThemeMusic != null) mainThemeMusic.volume = 0f; // Silence
    }

    IEnumerator ChangeSceneSequence(string sceneName)
    {
        isTransitioning = true;
        Time.timeScale = 1f; // IMPORTANT: Unpause time so the fade animation can run

        yield return StartCoroutine(FadeOutSequence()); // Wait until FadeOut finishes

        SceneManager.LoadScene(sceneName);
    }

    // Smooth transition for the "Underwater" filter effect
    IEnumerator FadeAudioEffect(float endVolume, float endFrequency)
    {
        if(mainThemeMusic == null) yield break;
        float startVolume = mainThemeMusic.volume;

        // Get current frequency safely to avoid null errors
        float startFrequency = (mainFilter != null) ? mainFilter.cutoffFrequency : defaultFrequency;
        float time = 0;

        while(time < 1)
        {
            // Using unscaledDeltaTime because TimeScale is 0 during Pause
            time += Time.unscaledDeltaTime * fadeSpeed;
            // Interpolate Volume
            mainThemeMusic.volume = Mathf.Lerp(startVolume, endVolume, time);


            // Interpolate Filter frequency
            float newFrequency = Mathf.Lerp(startFrequency, endFrequency, time);

            if(mainFilter != null) mainFilter.cutoffFrequency = newFrequency;
            if(bossFilter != null) bossFilter.cutoffFrequency = newFrequency;

            yield return null;
        }
        // Set exact final values
        mainThemeMusic.volume = endVolume;
        if(mainFilter != null) mainFilter.cutoffFrequency = endFrequency;
        if(bossFilter != null) bossFilter.cutoffFrequency = endFrequency;
    }

    // Crossfade between two songs
IEnumerator SwapMusic(AudioClip newMusicClip)
    {
        // Local references to swap them easily later
        AudioSource activeSource = mainThemeMusic;
        AudioSource newSource = bossThemeMusic;
        AudioLowPassFilter activeFilter = mainFilter;
        AudioLowPassFilter newFilter = bossFilter;

        // Setup the new track
        newSource.clip = newMusicClip;
        newSource.volume = 0;
        newSource.Play();

        float timer = 0;
        while(timer < 1)
        {
            timer += Time.unscaledDeltaTime * fadeSpeed;
            // One volume goes down, the other goes up
            activeSource.volume = Mathf.Lerp(defaultVolume, 0, timer);
            newSource.volume = Mathf.Lerp(0, defaultVolume, timer);

            // Keep the filter open/clean during the transition
            if(mainFilter != null) mainFilter.cutoffFrequency = defaultFrequency;
            if(bossFilter != null) bossFilter.cutoffFrequency = defaultFrequency;

            yield return null;
        }

        // Cleanup old track
        activeSource.Stop();
        activeSource.volume = 0;
        newSource.volume = defaultVolume;

        // Swap variable references
        mainThemeMusic = newSource;
        bossThemeMusic = activeSource;

        mainFilter = newFilter;
        bossFilter = activeFilter;
    }
}
