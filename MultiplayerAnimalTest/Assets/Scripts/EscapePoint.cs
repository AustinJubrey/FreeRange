using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet;
using FishNet.Object;
using UnityEngine;

public class EscapePoint : NetworkBehaviour
{
    private int _maxPlayers = 1;

    private HashSet<int> _presentPlayerIds;

    private void Start()
    {
        if (IsServer)
        {
            _presentPlayerIds = new HashSet<int>();
            PiersEvent.Listen<List<Transform>>(PiersEventKey.EventKey.PlayerCacheUpdated, OnPlayerCacheUpdated);
        }
    }
    
    private void OnPlayerCacheUpdated(List<Transform> transforms)
    {
        if (!IsServer)
            return;
        
        _maxPlayers = transforms.Count;
    }

    private void Update()
    {
        if (!IsServer)
            return;
        
        if (_presentPlayerIds.Count > 0)
        {
            if (_presentPlayerIds.Count == _maxPlayers)
            {
                if (CheckWinConditions())
                {
                    Debug.Log("Players Win!");
                }
            }
        }
    }

    private bool CheckIfAnyPlayerHasTruckKey()
    {
        foreach (var playerTransform in FreeRangePlayerManager.Instance.GetPlayerTransforms())
        {
            var chickController = playerTransform.GetComponent<ChickPlayerController>();
            if (chickController != null && chickController.GetEquippedPickUp() == EPickUpID.TruckKeys)
                return true;
        }

        return false;
    }

    private bool CheckWinConditions()
    {
        var HaveTruckKeys = CheckIfAnyPlayerHasTruckKey();
        if (HaveTruckKeys)
            return true;
        
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            var networkObject = other.GetComponent<NetworkObject>();

            if (!networkObject)
                return;
            
            _presentPlayerIds.Add(networkObject.Owner.ClientId);
                
            Debug.Log(_presentPlayerIds.Count + " / " + _maxPlayers);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsServer)
        {
            var networkObject = other.GetComponent<NetworkObject>();

            if (!networkObject)
                return;
            
            _presentPlayerIds.Remove(networkObject.Owner.ClientId);
        }
    }
}