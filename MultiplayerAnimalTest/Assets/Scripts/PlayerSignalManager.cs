using FishNet.Object;
using FishNet.Object.Prediction;
using UnityEngine;

public class PlayerSignalManager : NetworkBehaviour
{
    public override void OnStartClient()
    {
        base.OnStartClient();
        PostPresence();
    }
    
    public override void OnStopClient()
    {
        base.OnStopClient();
        PostExit();
    }

    [ServerRpc]
    private void PostPresence()
    {
        PiersEvent.Post(PiersEventKey.EventKey.ClientJoined);
    }
    
    [ServerRpc]
    private void PostExit()
    {
        //PiersEvent.Post(PiersEventKey.EventKey.ClientExited);
    }
}
