using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmAudioObject : MonoBehaviour
{
    private AudioSource _audioSource;
    public AudioClip Clip => _audioSource.clip;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void Play(AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.loop = true;
        _audioSource.Play();
    }

    public void Stop()
    {
        _audioSource.clip = null;
        _audioSource.Stop();
        gameObject.SetActive(false);
    }

    public void Pause()
    {
        _audioSource.Pause();
    }
    
    public void UnPause()
    {
        _audioSource.UnPause();
    }
}
