using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class FarmerController : NetworkBehaviour
{
    private FarmerNavMesh _navController;
    
    [SerializeField]
    private BackStabInteractPoint _backStabPoint;

    [SerializeField]
    private GameObject _ankleLight;
    
    [SerializeField]
    private GameObject _dropItem;

    private bool _hasDroppedItem = false;

    private void Start()
    {
        _navController = GetComponent<FarmerNavMesh>();
        _backStabPoint.SetEnterCallback(OnBackStabAvailable);
        _backStabPoint.SetExitCallback(OnBackStabNoLongerAvailable);
        
        PiersEvent.Listen<List<Transform>>(PiersEventKey.EventKey.PlayerCacheUpdated, OnPlayerCacheUpdated);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void GetDowned()
    {
        _navController.GetDowned();
        
        if(!_hasDroppedItem)
            DropItem();
    }

    [ServerRpc(RequireOwnership = false)]
    public void RecoverFromDowned()
    {
        _navController.RecoverFromDowned();
    }

    private void DropItem()
    {
        if (_dropItem == null)
            return;

        GameObject drop = Instantiate(_dropItem);
        drop.transform.position = transform.position;
        Spawn(drop);
        _hasDroppedItem = true;
    }

    [ObserversRpc]
    private void OnBackStabAvailable()
    {
        _ankleLight.SetActive(true);
    }

    [ObserversRpc]
    private void OnBackStabNoLongerAvailable()
    {
        _ankleLight.SetActive(false);
    }

    private void OnPlayerCacheUpdated(List<Transform> transforms)
    {
        _navController.SetPlayerTargets(transforms);
    }
}