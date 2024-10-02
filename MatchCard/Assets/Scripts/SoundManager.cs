using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip flipSound;
    [SerializeField] private AudioClip matchSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip backgroundMusic;

    private void Start()
    {
        // Play background music at the start of the game       
    }

    public void PlayFlipSound()
    {
        PlaySound(flipSound);
    }

    public void PlayMatchSound()
    {
        PlaySound(matchSound);
    }

    public void PlayWinSound()
    {
        PlaySound(winSound);
    }

    public void PlayGameOverSound()
    {
        PlaySound(gameOverSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource && clip)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void PlayBackgroundMusic()
    {
        if (audioSource && backgroundMusic)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}
