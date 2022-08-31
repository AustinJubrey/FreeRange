using System;
using FishNet.Example.Prediction.CharacterControllers;
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

    private bool _playerWithinRange;

    private ChickPlayerController _nearbyPlayer;

    void Update()
    {
        if (_playerWithinRange && Input.GetKey(KeyCode.E) && _nearbyPlayer != null)
        {
            OnCollected();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnCollected()
    {
        _nearbyPlayer.EquipPickUp(_pickUpData._id.ToString());
        Despawn();
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
