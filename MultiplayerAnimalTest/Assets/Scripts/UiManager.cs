using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using FishNet;
using FishNet.Managing.Scened;
using FishNet.Object;
using UnityEngine;

public class UiManager : NetworkBehaviour
{
    [SerializeField] private GameObject _gameOverParent;
    [SerializeField] private GameOverScreen _gameOverScreen;
    
    private bool _backToLobbyTriggered = false;
    
    // Start is called before the first frame update
    void Start()
    {
        PiersEvent.Listen(PiersEventKey.EventKey.PlayersWin, OnPlayersWin);
        PiersEvent.Listen(PiersEventKey.EventKey.PlayersAllCaught, OnPlayersLose);
        PiersEvent.Listen(PiersEventKey.EventKey.SendBackToLobby, OnRequestedSendBackToLobby);
    }

    private void OnRequestedSendBackToLobby()
    {
        OnSendBackToLobby();
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnSendBackToLobby()
    {
        GoToLobbyScene();
    }

    [ObserversRpc]
    private void OnPlayersWin()
    {
        _gameOverScreen.SetTitle("Freedom!");
        _gameOverScreen.SetDescription("All of the chicks made it to the escape truck, avoiding a crispy fate.");
        _gameOverParent.SetActive(true);
    }

    [ObserversRpc]
    private void OnPlayersLose()
    {
        _gameOverScreen.SetTitle("Game Over");
        _gameOverScreen.SetDescription("All of the chicks were captured, and then turned into chicken nuggets.");
        _gameOverParent.SetActive(true);
    }
    
    private void GoToLobbyScene()
    {
        if(!IsServer || _backToLobbyTriggered)
            return;

        _backToLobbyTriggered = true;
        Debug.Log("Going back to the lobby");
        SceneLoadData sld = new SceneLoadData("Lobby");
        sld.ReplaceScenes = ReplaceOption.All;
        sld.Params.ClientParams = SerializePlayerNames();
        InstanceFinder.SceneManager.LoadGlobalScenes(sld);
    }
    
    private byte[] SerializePlayerNames()
    {
        var binFormatter = new BinaryFormatter();
        var memoryStream = new MemoryStream();
        binFormatter.Serialize(memoryStream, GetPlayerDictionaryForSerialization());
        return memoryStream.ToArray();
    }
    
    private Dictionary<int, string> GetPlayerDictionaryForSerialization()
    {
        var newDict = new Dictionary<int, string>();
        foreach (var player in FreeRangePlayerManager.Instance.GetAllPlayerInfo())
        {
            newDict.Add(player.Connection.ClientId, player.Name);
        }

        return newDict;
    }
}
