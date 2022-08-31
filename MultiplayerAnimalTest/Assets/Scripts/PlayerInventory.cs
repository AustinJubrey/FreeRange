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
    private HeldItem _equippedKnife;
        
    [SerializeField]
    private HeldItem _equippedTruckKey;
    
    [SerializeField]
    private GameObject _droppedKnife;
    
    [SerializeField]
    private GameObject _droppedTruckKey;
    
    private EPickUpID _equippedItem = EPickUpID.Nothing;

    // If we can figure out spawning the items correctly on the character,
    // this function would instead just get the prefab from the PickUpData
    [ServerRpc(RunLocally = true, RequireOwnership = false)]
    public void EquipPickUp(string stringID)
    {
        Enum.TryParse(stringID, out EPickUpID id);
        
        switch (id)
        {
            case EPickUpID.Nothing:
                _equippedItem = EPickUpID.Nothing;
                break;
            case EPickUpID.Knife:
                Spawn(_equippedKnife.gameObject);
                _equippedItem = EPickUpID.Knife;
                break;
            case EPickUpID.TruckKeys:
                Spawn(_equippedTruckKey.gameObject);
                _equippedItem = EPickUpID.TruckKeys;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(id), id, null);
        }
    }
    
    public void DropEquippedItem()
    {
        GameObject droppedItem = null;

        switch (_equippedItem)
        {
            case EPickUpID.Nothing:
                Debug.Log("Tried to drop nothing");
                break;
            case EPickUpID.Knife:
                droppedItem = _droppedKnife;
                _equippedKnife.DeactivateItem();
                break;
            case EPickUpID.TruckKeys:
                droppedItem = _droppedTruckKey;
                _equippedTruckKey.DeactivateItem();
                break;
        }
        _equippedItem = EPickUpID.Nothing;

        if (droppedItem == null)
            return;

        GameObject droppedGameObject = Instantiate(droppedItem);
        droppedGameObject.transform.position = transform.position;
        Spawn(droppedGameObject);
    }

    public EPickUpID GetEquippedItem()
    {
        return _equippedItem;
    }
}
