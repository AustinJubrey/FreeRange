using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using FishNet;
using FishNet.Connection;
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

    private bool _isReady;

    private bool _hasSetName;

    public override void OnStartClient()
    {
        base.OnStartClient();
        _playerNames.OnChange += _playerNames_OnChange;
        _playerReadyDict.OnChange += __playerReadyDict_OnChange;
        UpdateReadinessDictionary(_isReady);
        _readyButtonLabel.color = new Color32(254,9,0,255);
    }

    private void __playerReadyDict_OnChange(SyncDictionaryOperation op, NetworkConnection key, bool value, bool asServer)
    {
        UpdatePlayerReadyDict();
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
        UpdateDictionary(_nameField.text);
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
    private void UpdateDictionary(string name, NetworkConnection conn = null)
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
    
    private void UpdatePlayerReadyDict()
    {
        foreach (var player in _playerReadyDict)
        {
            //update visuals
        }
    }
}
