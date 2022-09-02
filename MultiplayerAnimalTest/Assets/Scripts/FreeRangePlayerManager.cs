using System.Collections.Generic;
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
    
    [SerializeField]
    private GameObject _playerPrefab;
    
    [SerializeField]
    private Transform _playerSpawn;
    
    private List<NetworkConnection> _playerConnections;
    
    private Dictionary<NetworkConnection, string> _playerConnectionNameDict =
        new Dictionary<NetworkConnection, string>();

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
        if (!IsServer)
        {
            SpawnPlayer();
        }
    }

    public Dictionary<NetworkConnection, string> GetConnectionNameDictionary()
    {
        return _playerConnectionNameDict;
    }

    public void SetPlayerDataFromLobby(Dictionary<int, string> nameDict)
    {
        if (!IsServer)
            return;
        
        foreach (var entry in InstanceFinder.ServerManager.Clients)
        {
            _playerConnectionNameDict.Add(entry.Value, nameDict[entry.Value.ClientId]);
            Debug.Log(entry.Value);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayer(NetworkConnection conn = null)
    {
        NetworkObject nob = Instantiate(_playerPrefab, _playerSpawn.position, _playerSpawn.rotation).GetComponent<NetworkObject>();
        InstanceFinder.ServerManager.Spawn(nob, conn);
        nob.GetComponent<ChickPlayerController>().SetNameLabel(_playerConnectionNameDict[conn]);
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
