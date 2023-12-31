using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource audioSource;

    [SerializeField]
    AudioClip MenuSound;

    [SerializeField]
    AudioClip ButtonSound;

    [SerializeField]
    AudioClip CompleteSound;

    [SerializeField]
    AudioClip CountdownSound;
    [SerializeField]
    AudioClip StartSound;
    [SerializeField]
    AudioClip HitPlayerSound;
    [SerializeField]
    AudioClip DeathSound;
    [SerializeField]
    AudioClip AddHitPointsSound;
    [SerializeField]
    AudioClip ClearAllSound;
    [SerializeField]
    AudioClip BadInputSound;
    [SerializeField]
    AudioClip[] HitEnemySounds = new AudioClip[4];

    [SerializeField]
    AudioClip GameMusic;
    [SerializeField]
    AudioClip IntroMusic;
    [SerializeField]
    AudioClip AmbientSounds;
    [SerializeField]
    AudioClip[] ClickSounds = new AudioClip[4];

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);

        audioSource = this.GetComponent<AudioSource>();
    }

    public void StartMusic()
    {
        audioSource.clip = GameMusic;
        audioSource.Play();
    }
    public void StartIntroMusic()
    {
        audioSource.clip = IntroMusic;
        audioSource.Play();
    }
    public void StartAmbient()
    {
        audioSource.clip = AmbientSounds;
        audioSource.Play();
    }
    public void StopMusic()
    {
        audioSource.Stop();
    }

    public void PlayMenuSound()
    {
        if (Globals.AudioOn)
            audioSource.PlayOneShot(MenuSound, 1f);
    }

    public void PlayButtonSound()
    {
        if (Globals.AudioOn)
            audioSource.PlayOneShot(ButtonSound, 1f);
    }

    public void PlayCompleteSound()
    {
        if (Globals.AudioOn)
            audioSource.PlayOneShot(CompleteSound, 1f);   
    }

    public void PlayCountdownSound()
    {
        if (Globals.AudioOn)
            audioSource.PlayOneShot(CountdownSound, 1f);   
    }

    public void PlayStartSound()
    {
        if (Globals.AudioOn)
            audioSource.PlayOneShot(StartSound, 1f);   
    }

    public void PlayHitPlayerSound()
    {
        if (Globals.AudioOn)
            audioSource.PlayOneShot(HitPlayerSound, 1f);   
    }

    public void PlayBadInputSound()
    {
        if (Globals.AudioOn)
            audioSource.PlayOneShot(BadInputSound, 1f);   
    }

    public void PlayDeathSound()
    {
        if (Globals.AudioOn)
            audioSource.PlayOneShot(DeathSound, .7f);   
    }

    public void PlayAddHitPointSound()
    {
        if (Globals.AudioOn)
            audioSource.PlayOneShot(AddHitPointsSound, 1f);   
    }

    public void PlayClearAllSound()
    {
        if (Globals.AudioOn)
            audioSource.PlayOneShot(ClearAllSound, 1f);   
    }
    
    public void PlayClickSound()
    {
        int num = Random.Range(0, ClickSounds.Length);
        audioSource.PlayOneShot(ClickSounds[num], .5f);
    }

    public void PlayHitEnemySound()
    {
        int num = Random.Range(0, HitEnemySounds.Length);
        audioSource.PlayOneShot(HitEnemySounds[num], 1f);
    }
}
