using UnityEngine;

public class TrackLibrary : MonoBehaviour
{
    [Header("Farmer Sounds")]
    [SerializeField] private AudioTrack _farmerNoticedPlayer_01;
    [Header("Farmer FootStep Sounds")]
    [SerializeField] private AudioTrack _farmerFootStepsDirt_01;
    [SerializeField] private AudioTrack _farmerFootStepsDirt_02;
    [Header("Player FootStep Sounds")]
    [SerializeField] private AudioTrack _playerFootStepGrass_01;
    [SerializeField] private AudioTrack _playerFootStepGrass_02;
    [SerializeField] private AudioTrack _playerFootStepGrass_03;

    public AudioTrack GetTrackByType(AudioTrackTypes type)
    {
        switch (type)
        {
            case AudioTrackTypes.FarmerNoticedPlayerSound: // Farmer Sounds
                return _farmerNoticedPlayer_01;
            case AudioTrackTypes.FarmerFootStepsDirt01:
                return _farmerFootStepsDirt_01;
            case AudioTrackTypes.FarmerFootStepsDirt02:
                return _farmerFootStepsDirt_02;
            
            case AudioTrackTypes.PlayerFootStepsGrass01: // Player Sounds
                return _playerFootStepGrass_01;
            case AudioTrackTypes.PlayerFootStepsGrass02:
                return _playerFootStepGrass_02;
            case AudioTrackTypes.PlayerFootStepsGrass03:
                return _playerFootStepGrass_03;
        }

        return null;
    }
}
