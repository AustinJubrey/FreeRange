using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using FishNet;
using FishNet.Managing.Scened;
using UnityEngine;

public class SceneDataHelper : MonoBehaviour
{
    private Dictionary<int, string> _playerConnectionDict =
        new Dictionary<int, string>();

    private void Awake()
    {
        InstanceFinder.SceneManager.OnLoadEnd += SceneManagerOnLoadEnd;
    }

    private void SceneManagerOnLoadEnd(SceneLoadEndEventArgs obj)
    {
        var data = obj.QueueData;
        var serializedData = data.SceneLoadData.Params.ClientParams;
        var mStream = new MemoryStream();
        var binFormatter = new BinaryFormatter();

        mStream.Write (serializedData, 0, serializedData.Length);
        mStream.Position = 0;
        
        var connectionDictionary = binFormatter.Deserialize(mStream) as Dictionary<int, string>;

        if (connectionDictionary == null)
            return;
        
        _playerConnectionDict = connectionDictionary;
        var playerManager = FreeRangePlayerManager.Instance;

        if (playerManager != null)
        {
            playerManager.SetPlayerDataFromLobby(_playerConnectionDict);
        }
        else
        {
            var lobbyController = FindObjectOfType<LobbyController>();
            if (lobbyController != null)
            {
                lobbyController.SetPlayerDataFromGame(_playerConnectionDict);
            }
        }
    }
}
