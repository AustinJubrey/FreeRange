using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet.Object;
using UnityEngine;

public class EscapePoint : NetworkBehaviour
{
    private int _numPlayersPresent = 0;
    private int _maxPlayers = 0;

    private void Start()
    {
        PiersEvent.Listen<Transform>(PiersEventKey.EventKey.ClientJoined, OnClientJoined);
    }

    private void OnClientJoined(Transform t)
    {
        _maxPlayers = NetworkManager.ClientManager.Clients.Count;
        
        if(IsServer)
            _maxPlayers++;
    }

    private void Update()
    {
        if (!IsServer)
            return;
        
        if (_numPlayersPresent > 0)
        {
            if (_maxPlayers == NetworkManager.ClientManager.Clients.Count)
            {
                Debug.Log("all players present for exit");
                if (CheckWinConditions())
                {
                    Debug.Log("Players Win!");
                }
            }
        }
    }

    private bool CheckWinConditions()
    {
        //check if one of the players has the truck key
        Debug.Log(NetworkManager.ClientManager.Clients.Count);
        Debug.Log(NetworkManager.ClientManager.Clients.First().Value.FirstObject.name);
        var firstPlayer = NetworkManager.ClientManager.Clients.First().Value.FirstObject.GetComponent<ChickPlayerController>();
        if (firstPlayer)
            return true;
        
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            _numPlayersPresent++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsServer)
        {
            _numPlayersPresent--;
        }
    }
}
