using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.Events;

public class BackStabInteractPoint : NetworkBehaviour
{
    [SerializeField]
    private FarmerController _farmerController;
    
    private UnityAction _backStabPointEnterCallback;
    private UnityAction _backStabPointExitCallback;
    
    [SyncVar]
    private bool _playerWithinRange;
    [SyncVar]
    private Transform _nearbyPlayer;

    private void Update()
    {
        if (_playerWithinRange && Input.GetKey(KeyCode.E) && _nearbyPlayer != null)
        {
            Debug.Log("backstabbing");
            OnBackStabInitiated();
        }
    }
    
    private void OnBackStabInitiated()
    {
        _nearbyPlayer.GetComponent<ChickPlayerController>().BackStabFarmer(transform);
        DownFarmer();
    }
    
    private void DownFarmer()
    {
        _farmerController.GetDowned();
    }

    public void SetEnterCallback(UnityAction callback)
    {
        _backStabPointEnterCallback = callback;
    }
    
    public void SetExitCallback(UnityAction callback)
    {
        _backStabPointExitCallback = callback;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        var playerController = other.GetComponent<ChickPlayerController>();
        if (playerController != null && playerController.GetEquippedPickUp() == EPickUpID.Knife)
        {
            _nearbyPlayer = other.transform;
            OnPlayerTriggered(_nearbyPlayer);
            
        }
    }

    [ServerRpc(RunLocally = true)]
    private void OnPlayerTriggered(Transform playerTransform)
    {
        _nearbyPlayer = playerTransform;
        _playerWithinRange = true;
        _backStabPointEnterCallback?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        var playerController = other.GetComponent<ChickPlayerController>();
        if (playerController != null && playerController.GetEquippedPickUp() == EPickUpID.Knife)
        {
            _nearbyPlayer = null;
            _playerWithinRange = false;
            _backStabPointExitCallback?.Invoke();
        }
    }
}
