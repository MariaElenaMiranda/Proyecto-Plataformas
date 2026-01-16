using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour
{
    public AudioSource soundEffect;
    public AudioClip hoverSound;
    public AudioClip clickSound;

    public CanvasGroup blackScreen;
    public float fadeSpeed = 2.0f;

    //Play Button
    public void Play()
    {
        StartCoroutine(FadingSequence());
    }

    IEnumerator FadingSequence()
    {
        if(clickSound != null)
        {
            soundEffect.PlayOneShot(clickSound);
        }

        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime * fadeSpeed;
            blackScreen.alpha = timer;
            yield return null;
        }

        blackScreen.alpha = 1;
        SceneManager.LoadScene("map");
    }

    //Exit Button
    public void Exit()
    {
        StartCoroutine(ExitSequence());
    }

    IEnumerator ExitSequence()
    {
        if(clickSound != null)
        {
            soundEffect.PlayOneShot(clickSound);
            yield return new WaitForSeconds(clickSound.length);
        }else
        {
            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }

    //Hover
    public void PlayHoverSound()
    {
        if(hoverSound != null)
        {
        soundEffect.PlayOneShot(hoverSound);
        }
    }
}

