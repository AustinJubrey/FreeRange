using FishNet.Object;
using UnityEngine;

public class AudioUtilityManager : NetworkBehaviour
{
    [SerializeField] private GameObject _audioSourcePrefab;
    [SerializeField] private TrackLibrary _trackLibrary;
    
    //Singleton things
    private static AudioUtilityManager _instance;
    public static AudioUtilityManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public GameObject GetAudioSourcePrefab()
    {
        return _audioSourcePrefab;
    }

    private AudioTrack GetAudioTrackByType(AudioTrackTypes type)
    {
        return _trackLibrary.GetTrackByType(type);
    }
    
    public void PlaySound(Transform soundParent, Vector3 soundPosition, string TrackType)
    {
        if (IsServer)
            PlaySoundOnClient(soundParent, soundPosition, TrackType);
        else
            PlaySoundRpc(soundParent, soundPosition, TrackType);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void PlaySoundRpc(Transform soundParent, Vector3 soundPosition, string TrackType)
    {
        PlaySoundOnClient(soundParent, soundPosition, TrackType);
    }
    
    [ObserversRpc]
    private void PlaySoundOnClient(Transform soundParent, Vector3 soundPosition, string TrackType)
    {
        GameObject dynamicSourceGameObject = Instantiate(_audioSourcePrefab);
        dynamicSourceGameObject.transform.SetParent(soundParent);
        dynamicSourceGameObject.transform.position = soundPosition;
        dynamicSourceGameObject.transform.localPosition = Vector3.zero;
        DynamicAudioSourceMB dynamicSource = dynamicSourceGameObject.GetComponent<DynamicAudioSourceMB>();
        dynamicSource.SetAudioTrack(GetAudioTrackByType(GetTrackTypeFromString(TrackType)));
    }

    private static AudioTrackTypes GetTrackTypeFromString(string str)
    {
        return (AudioTrackTypes) System.Enum.Parse(typeof(AudioTrackTypes), str);
    }
}
