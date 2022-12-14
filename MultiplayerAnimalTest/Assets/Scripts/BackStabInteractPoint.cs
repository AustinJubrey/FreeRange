using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.Events;

public class BackStabInteractPoint : NetworkBehaviour
{
    [SerializeField]
    private FarmerController _farmerController;
    
    private UnityAction _backStabPointEnterCallback;
    private UnityAction _backStabPointExitCallback;
    
    [SyncVar]
    private bool _playerWithinRange;
    [SyncVar]
    private Transform _nearbyPlayer;

    private bool _hasBeenDownedRecently;
    private float _downedCount = 0;
    private float _downedImmunityTime = 45;

    private void Update()
    {
        if (_hasBeenDownedRecently)
        {
            _downedCount += Time.deltaTime;

            if (_downedCount >= _downedImmunityTime)
            {
                _downedCount = 0;
                _hasBeenDownedRecently = false;
                //there could be other behaviour than just getting back up, depending on the item used to down, or something
                _farmerController.RecoverFromDowned();
            }
        }
        else if (_playerWithinRange && Input.GetKeyDown(KeyCode.E) && _nearbyPlayer != null)
        {
            OnBackStabInitiated();
        }
    }
    
    private void OnBackStabInitiated()
    {
        _backStabPointExitCallback?.Invoke();
        _nearbyPlayer.GetComponent<ChickPlayerController>().BackStabFarmer(transform);
        DownFarmer();
    }
    
    private void DownFarmer()
    {
        _farmerController.GetDowned();
        _hasBeenDownedRecently = true;
    }

    public void SetEnterCallback(UnityAction callback)
    {
        _backStabPointEnterCallback = callback;
    }
    
    public void SetExitCallback(UnityAction callback)
    {
        _backStabPointExitCallback = callback;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (_hasBeenDownedRecently)
            return;
        
        var playerController = other.GetComponent<ChickPlayerController>();
        if (playerController != null && playerController.GetEquippedPickUp() == EPickUpID.Knife)
        {
            _nearbyPlayer = other.transform;
            OnPlayerTriggered(_nearbyPlayer);
            
        }
    }

    [ServerRpc(RunLocally = true)]
    private void OnPlayerTriggered(Transform playerTransform)
    {
        _nearbyPlayer = playerTransform;
        _playerWithinRange = true;
        _backStabPointEnterCallback?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (_hasBeenDownedRecently)
            return;
        
        var playerController = other.GetComponent<ChickPlayerController>();
        if (playerController != null && playerController.GetEquippedPickUp() == EPickUpID.Knife)
        {
            _nearbyPlayer = null;
            _playerWithinRange = false;
            _backStabPointExitCallback?.Invoke();
        }
    }
}
