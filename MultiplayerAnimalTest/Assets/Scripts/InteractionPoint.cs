using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.Events;

public class InteractionPoint : NetworkBehaviour
{
    protected UnityAction _enterCallback;
    protected UnityAction _exitCallback;
    
    [SyncVar]
    protected HashSet<Transform> _nearbyPlayers;

    private void Start()
    {
        _nearbyPlayers = new HashSet<Transform>();
    }

    public void SetEnterCallback(UnityAction callback)
    {
        _enterCallback = callback;
    }
    
    public void SetExitCallback(UnityAction callback)
    {
        _exitCallback = callback;
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void OnPlayerTriggered(Transform playerTransform)
    {
        _nearbyPlayers.Add(playerTransform);
        _enterCallback?.Invoke();
    }
    
    protected void OnTriggerEnter(Collider other)
    {
        var playerController = other.GetComponent<ChickPlayerController>();
        if (playerController != null)
        {
            _nearbyPlayers.Add(other.transform);
            OnPlayerTriggered(other.transform);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        var playerController = other.GetComponent<ChickPlayerController>();
        if (playerController != null)
        {
            _nearbyPlayers.Remove(other.transform);
            _exitCallback?.Invoke();
        }
    }
}
