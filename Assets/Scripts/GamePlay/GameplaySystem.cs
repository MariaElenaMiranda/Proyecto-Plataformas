using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplaySystem : BaseSceneManager
{
    [Header("Gameplay Audio Settings")]
    public AudioSource bossThemeMusic; // The second source used for crossfading
    public AudioLowPassFilter mainFilter; // Filtro del Bosque
    public AudioLowPassFilter bossFilter;  // Filtro del Boss

    [Header("Filter Settings")]
    public float pausedFrequency = 500f;// Sounds like being underwater
    private float defaultFrequency = 22000f; // Fully open, clean sound
    public float pausedVolume = 0.3f;

    void Awake()
    {
        BaseAwake();
        // Apply silence immediately to both sources
        if (bossThemeMusic != null) bossThemeMusic.mute = !isMusicOn;
    }

    void Start()
    {
        BaseStart();
        // Always make sure the game starts with clean audio
        if(mainFilter != null) mainFilter.cutoffFrequency = defaultFrequency;
        if(bossFilter != null) bossFilter.cutoffFrequency = defaultFrequency;
    }

    public void ToggleGameplayMusic()
    {
        ToggleMusic();
        // Apply to both channels
        if(bossThemeMusic != null) bossThemeMusic.mute = !isMusicOn;
    }

    //to switch the background music (Boss Fight)
    public void SwitchMusic(AudioClip newMusicClip)
    {
        // Only start the transition if the song is actually different
        if(backgroundMusic.clip != newMusicClip)
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

    public void ExitScene(string sceneName)
    {
        Time.timeScale = 1f;
        ChangeScene(sceneName);
    }

//-----------------------------------------------------------------------------------
//COROUTINES:
    // Smooth transition for the "Underwater" filter effect
    IEnumerator FadeAudioEffect(float endVolume, float endFrequency)
    {
        if(backgroundMusic == null) yield break;
        float startVolume = backgroundMusic.volume;

        // Get current frequency safely to avoid null errors
        float startFrequency = (mainFilter != null) ? mainFilter.cutoffFrequency : defaultFrequency;
        float time = 0;

        while(time < 1)
        {
            // Using unscaledDeltaTime because TimeScale is 0 during Pause
            time += Time.unscaledDeltaTime * fadeSpeed;
            
            // Interpolate Volume
            backgroundMusic.volume = Mathf.Lerp(startVolume, endVolume, time);

            // Interpolate Filter frequency
            float newFrequency = Mathf.Lerp(startFrequency, endFrequency, time);
            if(mainFilter != null) mainFilter.cutoffFrequency = newFrequency;
            if(bossFilter != null) bossFilter.cutoffFrequency = newFrequency;

            yield return null;
        }
        // Set exact final values
        backgroundMusic.volume = endVolume;
        if(mainFilter != null) mainFilter.cutoffFrequency = endFrequency;
        if(bossFilter != null) bossFilter.cutoffFrequency = endFrequency;
    }

    // Crossfade between two songs
    IEnumerator SwapMusic(AudioClip newMusicClip)
    {
        // Local references to swap them easily later
        AudioSource activeSource = backgroundMusic;
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
        backgroundMusic = newSource;
        bossThemeMusic = activeSource;

        mainFilter = newFilter;
        bossFilter = activeFilter;
    }
}
