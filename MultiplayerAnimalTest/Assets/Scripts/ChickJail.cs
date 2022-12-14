using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class ChickJail : NetworkBehaviour
{
    [SerializeField]
    private Animator _animator;
    
    [SerializeField]
    private GameObject _interactionLight;
    
    [SerializeField]
    private ChickJailInteractionPoint _interactionPoint;

    [SyncVar]
    private bool _doorOpen;

    private void Start()
    {
        _interactionPoint.SetEnterCallback(OnOpenDoorAvailableRpc);
        _interactionPoint.SetExitCallback(OnOpenDoorNoLongerAvailableRpc);
    }

    public void OpenDoor()
    {
        if (!IsServer || _doorOpen)
            return;
        
        _animator.SetBool("DoorOpen", true);
        _doorOpen = true;
    }

    public void CloseDoor()
    {
        if (!IsServer || !_doorOpen)
            return;
        
        _animator.SetBool("DoorOpen", false);
        _doorOpen = false;
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void OnOpenDoorAvailableRpc()
    {
        OnOpenDoorAvailable();
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnOpenDoorNoLongerAvailableRpc()
    {
        OnOpenDoorNoLongerAvailable();
    }
    
    [ObserversRpc]
    private void OnOpenDoorAvailable()
    {
        _interactionLight.SetActive(true);
    }

    [ObserversRpc]
    private void OnOpenDoorNoLongerAvailable()
    {
        _interactionLight.SetActive(false);
    }
}
