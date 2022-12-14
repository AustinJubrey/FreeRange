using FishNet.Object;
using UnityEngine;

public class ItemGiver : NetworkBehaviour
{
    [SerializeField]
    private PickUpData _pickUpData;
    
    [SerializeField]
    private Transform _spawnLocation;
    
    [SerializeField]
    private ChickJailInteractionPoint _interactionPoint;
    
    private void Start()
    {
        //_interactionPoint.SetEnterCallback(OnOpenDoorAvailableRpc);
        //_interactionPoint.SetExitCallback(OnOpenDoorNoLongerAvailableRpc);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnItem()
    {
        var spawnedObject = Instantiate(_pickUpData._worldPrefab, _spawnLocation);
        
        spawnedObject.transform.position = transform.position;
        spawnedObject.transform.SetParent(null);
        Spawn(spawnedObject);
    }
}
