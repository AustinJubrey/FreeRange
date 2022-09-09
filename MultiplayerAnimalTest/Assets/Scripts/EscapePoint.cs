using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class EscapePoint : NetworkBehaviour
{
    private int _maxPlayers = 1;
    private bool _gameOverTriggered;

    private HashSet<int> _presentPlayerIds;

    private void Start()
    {
        _presentPlayerIds = new HashSet<int>();
        
        if (IsServer)
        {
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
        if (!IsServer || _gameOverTriggered)
            return;
        
        if (_presentPlayerIds.Count > 0)
        {
            if (_presentPlayerIds.Count == _maxPlayers)
            {
                if (CheckWinConditions())
                {
                    PiersEvent.Post(PiersEventKey.EventKey.PlayersWin);
                    _gameOverTriggered = true;
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
