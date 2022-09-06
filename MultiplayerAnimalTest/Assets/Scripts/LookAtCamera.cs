using FishNet.Object;
using UnityEngine;

public class LookAtCamera : NetworkBehaviour
{
    private Camera _camera;

    private void Start()
    {
        if(Owner.IsLocalClient)
            gameObject.SetActive(false);
        else
            FindLocalPlayerCamera();
    }

    private void FindLocalPlayerCamera()
    {
        _camera = Camera.main;
        
        //_camera = FreeRangePlayerManager.Instance.GetPlayerInfo(LocalConnection).playerObject
        //.GetComponent<ChickPlayerController>().GetCamera();
    }

    // Update is called once per frame
    void Update()
    {
        if (_camera == null)
            return;
        
        transform.LookAt(_camera.transform);
    }
}
