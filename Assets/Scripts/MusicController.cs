using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MusicController : MonoBehaviour
{
    public static MusicController Instance;

    [SerializeField] private AudioClip[] _audioClips;
    [SerializeField] private AudioSource _audioSource;

    private float _musicVolume = 1f;

    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        DiaryDialog.OnVolumeChange += SetVolume;
    }

    private void OnDisable()
    {
        DiaryDialog.OnVolumeChange -= SetVolume;
    }

    public void SwitchLocationMusic(LocationName locationName)
    {
        // TODO: переделать получение аудиоклипа
        _audioSource.clip = _audioClips[(int)locationName];

        if (_audioSource.isPlaying)
        {
            _audioSource.DOFade(0, 1).OnComplete(() => 
            {
                _audioSource.Stop();
                PlayMusic();
            });
        }
        else
        {
            PlayMusic();
        }
    }

    private void PlayMusic()
    {
        _audioSource.Play();
        _audioSource.DOFade(_musicVolume, 1);
    }

    public void SetVolume(float value)
    {
        _musicVolume = value;
        _audioSource.volume = value;
    }
}
