using FishNet.Object;
using FishNet.Object.Prediction;
using UnityEngine;

public class PlayerSignalManager : NetworkBehaviour
{
    
    void Start()
    {
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        PostPresence();
    }

    [ServerRpc]
    private void PostPresence()
    {
        PiersEvent.Post<Transform>(PiersEventKey.EventKey.ClientJoined, transform);
    }
}
