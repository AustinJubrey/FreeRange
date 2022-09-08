using FishNet.Object;
using UnityEngine;

public class PickUp : NetworkBehaviour
{
    [SerializeField]
    private SphereCollider _collider;

    [SerializeField]
    private GameObject _interactPrompt;
    
    [SerializeField]
    private PickUpData _pickUpData;
    
    [SerializeField]
    private AudioTrack _pickupSound;

    private bool _playerWithinRange;

    private ChickPlayerController _nearbyPlayer;

    void Update()
    {
        if (_playerWithinRange && Input.GetKeyDown(KeyCode.E) && _nearbyPlayer != null)
        {
            OnCollected();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnCollected()
    {
        _nearbyPlayer.EquipPickUp(_pickUpData._id.ToString());
        PlayCollectedSound();
        Despawn();
    }

    [ObserversRpc]
    private void PlayCollectedSound()
    {
        GameObject dynamicSourceGameObject = Instantiate(AudioUtilityManager.Instance.GetAudioSourcePrefab());
        dynamicSourceGameObject.transform.SetParent(null);
        DynamicAudioSourceMB dynamicSource = dynamicSourceGameObject.GetComponent<DynamicAudioSourceMB>();
        dynamicSource.SetAudioTrack(_pickupSound);
            
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ChickPlayerController>() != null)
        {
            _interactPrompt.SetActive(true);
            _playerWithinRange = true;
            _nearbyPlayer = other.GetComponent<ChickPlayerController>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<ChickPlayerController>() != null)
        {
            _interactPrompt.SetActive(false);
            _playerWithinRange = false;
            _nearbyPlayer = null;
        }
    }
}
