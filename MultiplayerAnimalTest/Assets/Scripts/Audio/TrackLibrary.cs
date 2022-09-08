using UnityEngine;

public class TrackLibrary : MonoBehaviour
{
    [SerializeField] private AudioTrack _farmerNoticedPlayer_01;
    [SerializeField] private AudioTrack _farmerFootStepsDirt_01;
    [SerializeField] private AudioTrack _playerFootStepGrass_01;

    public AudioTrack GetTrackByType(AudioTrackTypes type)
    {
        switch (type)
        {
            case AudioTrackTypes.FarmerNoticedPlayerSound:
                return _farmerNoticedPlayer_01;
            case AudioTrackTypes.FarmerFootStepsDirt:
                return _farmerFootStepsDirt_01;
            case AudioTrackTypes.PlayerFootStepsGrass:
                return _playerFootStepGrass_01;
        }

        return null;
    }
}
