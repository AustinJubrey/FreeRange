using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Serializing;
using UnityEngine;

public class PlayerInventory : NetworkBehaviour
{

    [SerializeField]
    private PickUpData _chickJailKeyData;
    
    [SerializeField]
    private PickUpData _truckKeysData;
    
    [SerializeField]
    private PickUpData _knifeData;
    
    [SerializeField]
    private GameObject _weaponSlot;
    
    private EPickUpID _equippedItem = EPickUpID.Nothing;
    
    private HeldItem _heldItem = null;

    [ServerRpc(RunLocally = true, RequireOwnership = false)]
    public void EquipPickUp(string stringID)
    {
        Enum.TryParse(stringID, out EPickUpID id);
        GameObject go = null;
        
        switch (id)
        {
            case EPickUpID.Nothing:
                _equippedItem = EPickUpID.Nothing;
                break;
            case EPickUpID.Knife:
                go = Instantiate(_knifeData._equippedPrefab, _weaponSlot.transform);
                _equippedItem = EPickUpID.Knife;
                break;
            case EPickUpID.TruckKeys:
                go = Instantiate(_truckKeysData._equippedPrefab, _weaponSlot.transform);
                _equippedItem = EPickUpID.TruckKeys;
                break;
            case EPickUpID.CoopCageKey:
                go = Instantiate(_chickJailKeyData._equippedPrefab, _weaponSlot.transform);
                _equippedItem = EPickUpID.CoopCageKey;
                break;
        }

        if (go != null)
        {
            _heldItem = go.GetComponent<HeldItem>();
            go.transform.SetParent(_weaponSlot.transform);
            Spawn(go);
        }
    }
    
    public void DropEquippedItem()
    {
        GameObject droppedItem = null;
        Debug.Log("dropping item");

        switch (_equippedItem)
        {
            case EPickUpID.Nothing:
                Debug.Log("Tried to drop nothing");
                break;
            case EPickUpID.Knife:
                droppedItem = Instantiate(_knifeData._worldPrefab, transform);
                break;
            case EPickUpID.TruckKeys:
                droppedItem = Instantiate(_truckKeysData._worldPrefab, transform);
                break;
            case EPickUpID.CoopCageKey:
                droppedItem = Instantiate(_chickJailKeyData._worldPrefab, transform);
                break;
        }
        _equippedItem = EPickUpID.Nothing;

        if (droppedItem == null)
            return;

        RemoveWeaponGameObject();
        droppedItem.transform.position = transform.position;
        droppedItem.transform.SetParent(null);
        Spawn(droppedItem);
    }

    private void RemoveWeaponGameObject()
    {
        _heldItem.DeactivateItem();
    }

    public EPickUpID GetEquippedItem()
    {
        return _equippedItem;
    }
}
