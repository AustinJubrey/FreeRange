using System;
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
    
    protected virtual void OnPlayerInteraction(Transform playerTransform)
    {
        // Player has interacted, do the thing
        Debug.Log("player interacted with thing");
    }

    protected void OnTriggerEnter(Collider other)
    {
        var playerController = other.GetComponent<ChickPlayerController>();
        if (playerController != null)
        {
            OnPlayerTriggered(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var playerController = other.GetComponent<ChickPlayerController>();
        if (playerController != null)
        {
            OnPlayerExitTrigger(other.transform);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnPlayerExitTrigger(Transform playerTransform)
    {
        var playerController = playerTransform.GetComponent<ChickPlayerController>();
        if (playerController != null)
        {
            playerController.SetInteractionCallback(null);
            _nearbyPlayers.Remove(playerTransform);
            _exitCallback?.Invoke();
        }
    }
}
