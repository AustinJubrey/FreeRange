using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioTrack
{
    [SerializeField]
	private AudioClip m_clip = null;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float m_volume = 0.5f;

    // Set to 0.0f for 2D sounds, 1.0f for 3D sounds.
    // Possible to make it something in between, but should ideally be completely 2D or completely 3D
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float m_spatialBlend = 0.0f;

    [SerializeField]
	private bool m_loop = false;

    [SerializeField]
    private bool m_isMusic = false;

    private bool m_isMuted = false;

    private float m_originalVolume = 0.0f;

    private float m_PlayTime;
    public float PlayTime { get { return m_PlayTime; } set { m_PlayTime = value; } }

    private AudioTrackTypes m_trackType;
    public AudioTrackTypes TrackType { get { return m_trackType; } set { m_trackType = value; } }

    public AudioClip GetAudioClip()
    {
        return m_clip;
    }

    public bool IsMuted()
    {
        return m_isMuted;
    }

    public void Mute()
    {
        if (m_volume != 0.0f)
        {
            SetVolume(0.0f);
            m_isMuted = true;
        }
    }

    public void UnMute()
    {
        SetVolume(m_originalVolume);
        m_isMuted = false;
    }

    public bool IsMusic()
    {
        return m_isMusic;
    }

    public float GetVolume()
    {
        return m_volume;
    }

    public float GetOriginalVolume()
    {
        return m_originalVolume;
    }

    public float GetSpatialBlend()
    {
        return m_spatialBlend;
    }

    public bool GetLoop()
    {
        return m_loop;
    }

    public void SetVolume(float vol)
    {
        m_originalVolume = m_volume;
        m_volume = Mathf.Clamp(vol, 0.0f, 1.0f);
    }

    public void Set3D()
    {
        m_spatialBlend = 1.0f;
    }

    public void Set2D()
    {
        m_spatialBlend = 0.0f;
    }

    public bool Is2DTrack()
    {
        return m_spatialBlend == 0.0f;
    }

    public bool Is3DTrack()
    {
        return m_spatialBlend > 0.0f;
    }
}