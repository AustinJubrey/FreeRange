using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public struct PlayerInfo
{
    public NetworkConnection Connection;
    public string Name;
    public Transform playerObject;
}

public class FreeRangePlayerManager : NetworkBehaviour
{
    [SyncVar]
    [SerializeField]
    private Transform _capturedChickSpawn;
    
    [SerializeField]
    private GameObject _playerPrefab;
    
    [SerializeField]
    private List<Transform> _playerSpawns;

    private int _spawnIndex;
    private List<PlayerInfo> _playerInfo;
    
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
        
        _playerInfo = new List<PlayerInfo>();
        UpdatePlayerInfo();

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

    public void SetPlayerDataFromLobby(Dictionary<int, string> nameDict)
    {
        if (!IsServer)
            return;
        
        foreach (var entry in InstanceFinder.ServerManager.Clients)
        {
            if(!_playerConnectionNameDict.ContainsKey(entry.Value))
                _playerConnectionNameDict.Add(entry.Value, nameDict[entry.Value.ClientId]);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayer(NetworkConnection conn = null)
    {
        if (_spawnIndex >= _playerSpawns.Count)
            _spawnIndex = 0;
        
        NetworkObject nob = Instantiate(_playerPrefab, _playerSpawns[_spawnIndex].position, _playerSpawns[_spawnIndex].rotation).GetComponent<NetworkObject>();
        nob.transform.position = _playerSpawns[_spawnIndex].position;
        InstanceFinder.ServerManager.Spawn(nob, conn);
        nob.GetComponent<ChickPlayerController>().SetNameLabel(_playerConnectionNameDict[conn]);

        ++_spawnIndex;
    }

    public Transform GetCapturedChickSpawn()
    {
        return _capturedChickSpawn;
    }

    private void UpdatePlayerInfo()
    {
        if (!IsServer)
            return;
        
        foreach (var conn in InstanceFinder.ServerManager.Clients)
        {
            var foundInfo = _playerInfo.Find(x => x.Connection == conn.Value);
            if (foundInfo.Name == null)
            {
                // Properties
                foundInfo.Connection = conn.Value;
                foundInfo.Name = _playerConnectionNameDict[conn.Value];
            
                var firstObject = conn.Value.FirstObject;
                if (firstObject != null)
                {
                    foundInfo.playerObject = firstObject.transform;
                }
                else
                {
                    Debug.Log("couldnt find player object");
                }
            
                _playerInfo.Add(foundInfo);
            }
        }
    }

    public PlayerInfo GetPlayerInfo(NetworkConnection conn)
    {
        return _playerInfo.Find(x => x.Connection == conn);
    }
    
    public List<PlayerInfo> GetAllPlayerInfo()
    {
        return _playerInfo;
    }
    
    private void RefreshPlayerConnectionsAndBroadcast()
    {
        UpdatePlayerInfo();
        PiersEvent.Post(PiersEventKey.EventKey.PlayerCacheUpdated, GetPlayerTransforms());
    }

    public List<Transform> GetPlayerTransforms()
    {
        List<Transform> playerTransforms = new List<Transform>();

        foreach (var info in _playerInfo)
        {
            if (info.playerObject == null)
            {
                Debug.Log("connection had nothing in its objects list");
            }
            
            if(info.playerObject != null)
                playerTransforms.Add(info.playerObject);
        }

        return playerTransforms;
    }
}
