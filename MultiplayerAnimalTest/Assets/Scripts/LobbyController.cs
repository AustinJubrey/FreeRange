using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using TMPro;
using UnityEngine;

public class LobbyController : NetworkBehaviour
{
    [SyncObject]
    private readonly SyncDictionary<NetworkConnection, string> _playerNames =
        new SyncDictionary<NetworkConnection, string>();
    
    [SyncObject]
    private readonly SyncDictionary<NetworkConnection, bool> _playerReadyDict =
        new SyncDictionary<NetworkConnection, bool>();

    [SerializeField]
    private TMP_InputField _nameField;
    
    [SerializeField]
    private TextMeshProUGUI _playerNamesLabel;
    
    [SerializeField]
    private TextMeshProUGUI _readyButtonLabel;
    
    [SerializeField]
    private NetworkManager _networkManager;

    private bool _isReady;
    private bool _hasSetName;
    
    private static string API_URL = "https://api.cloud.playflow.app/";
    private static string version = "2";

    private void Awake()
    {
        FindAndSetFirstServerAddress();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        _playerNames.OnChange += _playerNames_OnChange;
        _playerReadyDict.OnChange += __playerReadyDict_OnChange;
        UpdateReadinessDictionary(_isReady);
        _readyButtonLabel.color = new Color32(254,9,0,255);
        Reinitialize();
    }

    private async Task ConnectToExistingServer(Server server)
    {
        _networkManager.TransportManager.Transport.SetClientAddress(server.match_id);
        Debug.Log("Connected to: " + server.match_id);
    }

    private async Task FindAndSetFirstServerAddress()
    {
        string response = await GetActiveServers("468ff68469868b4285933c61c28e8135", "us-east", true);
        Server[] servers = JsonHelper.FromJson<Server>(response);
        
        if(servers != null && servers.Length > 0)
            ConnectToExistingServer(servers[0]);
        else
        {
            Debug.Log("couldn't find servers");
        }
    }

    // For setting things up after returning from a game
    private void Reinitialize()
    {
        if (_playerNames.Count > 0 && _playerNames[LocalConnection] != null && _playerNames[LocalConnection] != "")
        {
            _hasSetName = true;
            _nameField.text = _playerNames[LocalConnection];
        }
    }

    private void __playerReadyDict_OnChange(SyncDictionaryOperation op, NetworkConnection key, bool value, bool asServer)
    {
        UpdatePlayerList();
    }

    private void _playerNames_OnChange(SyncDictionaryOperation op, NetworkConnection key, string value, bool asServer)
    {
        UpdatePlayerList();
    }

    public void OnConfirmName()
    {
        if (_nameField.text != "")
            _hasSetName = true;
        UpdateDictionaryRpc(_nameField.text);
    }
    
    public void OnReadyButton()
    {
        if (!_hasSetName)
            return;
        _isReady = !_isReady;
        _readyButtonLabel.color = _isReady ? new Color32(0,254,111,255) : new Color32(254,9,0,255);
        UpdateReadinessDictionary(_isReady);
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateDictionaryRpc(string name, NetworkConnection conn = null)
    {
        _playerNames[conn] = name;
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void UpdateReadinessDictionary(bool isReady, NetworkConnection conn = null)
    {
        _playerReadyDict[conn] = isReady;

        if (CheckIfAllReady())
        {
            GoToGameScene();
        }
    }

    private bool CheckIfAllReady()
    {
        if (_playerReadyDict.Count == 0)
            return false;
        
        foreach (var player in _playerReadyDict)
        {
            if (!player.Value)
                return false;
        }

        return true;
    }

    private void GoToGameScene()
    {
        SceneLoadData sld = new SceneLoadData("TestFarmScene");
        sld.ReplaceScenes = ReplaceOption.All;
        sld.Params.ClientParams = SerializePlayerNames();
        InstanceFinder.SceneManager.LoadGlobalScenes(sld);
    }

    private Dictionary<int, string> GetPlayerDictionaryForSerialization()
    {
        var newDict = new Dictionary<int, string>();
        foreach (var player in _playerNames)
        {
            newDict.Add(player.Key.ClientId, player.Value);
        }

        return newDict;
    }

    private byte[] SerializePlayerNames()
    {
        var binFormatter = new BinaryFormatter();
        var memoryStream = new MemoryStream();
        binFormatter.Serialize(memoryStream, GetPlayerDictionaryForSerialization());
        return memoryStream.ToArray();
    }

    private void UpdatePlayerList()
    {
        string updatedString = "Players:\n";
        foreach (var player in _playerNames)
        {
            if(!_playerReadyDict.ContainsKey(player.Key))
                continue;
            
            var nameString = _playerReadyDict[player.Key]
                ? "<color=green>" + player.Value + "</color> \n"
                : "<color=red>" + player.Value + "</color> \n";
            
            updatedString += nameString;
        }

        _playerNamesLabel.text = updatedString;
    }
    
    public void SetPlayerDataFromGame(Dictionary<int, string> nameDict)
    {
        if (!IsServer)
            return;
        
        _playerNames.Clear();
        foreach (var entry in InstanceFinder.ServerManager.Clients)
        {
            if(!_playerNames.ContainsKey(entry.Value))
                _playerNames.Add(entry.Value, nameDict[entry.Value.ClientId]);
        }
    }
    
    public static async Task<string> GetActiveServers(string token,string region, bool includelaunchingservers)
    {
        String output = "";
        try
        {
        
            string actionUrl = API_URL + "list_servers";

            using (var client = new HttpClient())
            using (var formData = new MultipartFormDataContent())
            {
                formData.Headers.Add("token", token);
                formData.Headers.Add("region", region);
                formData.Headers.Add("version", version);
                formData.Headers.Add("includelaunchingservers", includelaunchingservers.ToString());

            
                var response = await client.PostAsync(actionUrl, formData);
            
                if (!response.IsSuccessStatusCode)
                {
                    Debug.Log(System.Text.Encoding.UTF8.GetString( await response.Content.ReadAsByteArrayAsync()));
                }
            
                output = System.Text.Encoding.UTF8.GetString(await response.Content.ReadAsByteArrayAsync());
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

        return output;
    }
    
    [Serializable]
    public class Server
    {
        public string ssl_port;
        public bool ssl_enabled;
        public string server_arguments;
        public string status;
        public string port;
        public string match_id;
        public string ssl_url;

    }


    [Serializable]
    public class MatchInfo
    {
        public string match_id;
        public string server_url;
        public string ssl_port;
    }

    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.servers;
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.servers = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.servers = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] servers;
        }
    }
}
