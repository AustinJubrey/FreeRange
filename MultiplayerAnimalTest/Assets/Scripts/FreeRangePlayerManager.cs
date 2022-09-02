using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class FreeRangePlayerManager : NetworkBehaviour
{
    [SyncVar]
    [SerializeField]
    private Transform _capturedChickSpawn;
    
    private List<NetworkConnection> _playerConnections;
    
    //Singleton things
    private static FreeRangePlayerManager _instance;
    public static FreeRangePlayerManager Instance { get { return _instance; } }

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
        
        _playerConnections = new List<NetworkConnection>();
        RefreshPlayerConnections();

        Listen();
    }

    private void Listen()
    {
        PiersEvent.Listen(PiersEventKey.EventKey.ClientJoined, RefreshPlayerConnectionsAndBroadcast);
        PiersEvent.Listen(PiersEventKey.EventKey.ClientExited, RefreshPlayerConnectionsAndBroadcast);
    }

    private void Start()
    {
    }

    public Transform GetCapturedChickSpawn()
    {
        return _capturedChickSpawn;
    }

    private void RefreshPlayerConnections()
    {
        if (!IsServer)
            return;
        
        _playerConnections = new List<NetworkConnection>();
        
        foreach (var conn in InstanceFinder.ServerManager.Clients)
        {
            _playerConnections.Add(conn.Value);
        }
    }
    
    private void RefreshPlayerConnectionsAndBroadcast()
    {
        RefreshPlayerConnections();
        PiersEvent.Post(PiersEventKey.EventKey.PlayerCacheUpdated, GetPlayerTransforms());
    }

    public List<Transform> GetPlayerTransforms()
    {
        List<Transform> playerTransforms = new List<Transform>();

        foreach (var conn in _playerConnections)
        {
            if (conn.Objects == null || conn.Objects.Count == 0)
            {
                //Debug.Log("connection had nothing in its objects list");
            }

            var firstObject = conn.FirstObject;
            if(firstObject != null)
                playerTransforms.Add(firstObject.transform);
            //else
                //Debug.Log("player connection had a null first object");
        }

        return playerTransforms;
    }

    public int GetAmountOfPlayers()
    {
        return _playerConnections.Count;
    }
}
