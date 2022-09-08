using FishNet.Object;
using UnityEngine;

public class DynamicAudioSourceMB : MonoBehaviour {

    private AudioTrack _audioTrack;
    private AudioSource _audioSource;
    public float _playTime;
    private float _lengthOfAudioPlayed;
    private bool _isInitialized;

    private void Update()
    {
        if (_isInitialized)
        {
            _lengthOfAudioPlayed += Time.deltaTime;
            _audioSource.volume = GetScaledVolume();

            if (_audioSource == null || _audioTrack == null || _audioTrack.GetAudioClip() == null ||
                FinishedPlaying())
            {
                Destroy(gameObject);
            }
        }
    }

    private void InitializeDynamicAudioSource()
    {
        transform.SetParent(null);
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _audioTrack.GetAudioClip();
        _audioSource.volume = GetScaledVolume();
        _audioSource.spatialBlend = _audioTrack.GetSpatialBlend();
        _audioSource.loop = _audioTrack.GetLoop();

        _isInitialized = true;
        Play();
    }

    public void SetAudioTrack(AudioTrack track)
    {
        _audioTrack = track;
        InitializeDynamicAudioSource();
    }

    public void Mute()
    {
        _audioSource.mute = true;
    }

    public void UnMute()
    {
        _audioSource.mute = false;
    }

    private float GetScaledVolume()
    {
        if(_audioTrack == null)
        {
            return 0.0f;
        }

        return _audioTrack.GetVolume();
    }

    public void Play()
    {
        if(CanPlay())
        {
            _audioSource.Play();
            _lengthOfAudioPlayed = 0.0f;
        }
    }
    
    private bool CanPlay()
    {
        return _audioSource != null && !_audioSource.isPlaying && _audioTrack != null && _audioTrack.GetAudioClip() != null;
    }

    private bool FinishedPlaying()
    {
        return (!_audioSource.loop && _lengthOfAudioPlayed >= _audioTrack.GetAudioClip().length) || (_playTime != 0.0f && _lengthOfAudioPlayed >= _playTime);
    }

    private void OnDestroy()
    {
        //PiersEvent.Post<AudioSource>(PiersEventKey.EventKey.AudioSourceReleased, _audioSource);
    }

    private void OnDisable()
    {
        Destroy(gameObject);
    }

}
