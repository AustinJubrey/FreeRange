using UnityEngine;

public class AudioUtilityManager : MonoBehaviour
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
}
