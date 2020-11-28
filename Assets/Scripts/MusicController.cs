using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class MusicController : MonoBehaviour
{
    public static MusicController Instance;

    private bool _isMusicEnabled = true;
    public bool IsMusicEnabled 
    { 
        get 
        {
            return _isMusicEnabled;
        }

        set
        {
            _isMusicEnabled = value;

            if (_isMusicEnabled == false)
            {
                _audioSource.DOFade(0, 1).OnComplete(() =>
                {
                    _audioSource.Stop();
                });
            }
            else
            {
                PlayMusic();
            }
        } 
    }

    [SerializeField] private AudioClip[] _audioClips;
    [SerializeField] private AudioSource _audioSource;

    public float MusicVolume { get; private set; } = 1f;

    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        SettingsDialog.OnVolumeChange += SetVolume;
    }

    private void OnDisable()
    {
        SettingsDialog.OnVolumeChange -= SetVolume;
    }

    public void SwitchLocationMusic(LocationName locationName)
    {
        if (locationName == LocationName.Unknown)
        {
            _audioSource.Stop();
            return;
        }

        // TODO: переделать получение аудиоклипа
        _audioSource.clip = _audioClips[(int)locationName];

        if (_audioSource.isPlaying)
        {
            StopMusic(PlayMusic);
        }
        else
        {
            PlayMusic();
        }
    }

    private void StopMusic(Action callback)
    {
        _audioSource.DOFade(0, 1).OnComplete(() =>
        {
            _audioSource.Stop();

            callback?.Invoke();
        });
    }

    private void PlayMusic()
    {
        if (_isMusicEnabled == false)
        {
            return;
        }

        _audioSource.volume = 0;

        _audioSource.Play();
        _audioSource.DOFade(MusicVolume, 1);
    }

    public void SetVolume(float value)
    {
        MusicVolume = value;
        _audioSource.volume = value;
    }

    public void PlayOneShot(AudioClip clip, Action callback)
    {
        StopMusic(() => 
        {
            StartCoroutine(EndGameAction(clip, callback));
        });
    }

    // костыльное завершение игры, потом переделать
    private IEnumerator EndGameAction(AudioClip clip, Action callback)
    {
        _audioSource.PlayOneShot(clip);

        yield return new WaitForSeconds(1.3f);

        callback?.Invoke();
    }
}
