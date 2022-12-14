using FishNet.Example.Prediction.CharacterControllers;
using FishNet.Object;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ChickPlayerController : NetworkBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _nameLabel;
    
    [SerializeField]
    private Camera _camera;
    
    [SerializeField]
    private Camera _chickVisionCamera;
    
    private CharacterControllerPrediction _characterPrediction;
    private PlayerInventory _inventory;

    private float _equipCooldown = 1f;
    private float _equipCooldownCounter;
    private bool _canEquip = true;
    
    // Foot Steps
    private float _footStepCount;
    private float _normalFootStepTime = 0.417f;
    private float _maxFootStepTime;
    
    // Chick Vision
    private bool _isMainCameraOn = true;
    private float _chickVisionCooldown = 0.5f;
    private float _chickVisionCount;
    
    // Hiding
    private bool _isHidden;
    
    // Interaction
    private UnityAction<Transform> _localInteractionCallback;

    private void Awake()
    {
        _characterPrediction = GetComponent<CharacterControllerPrediction>();
        _inventory = GetComponent<PlayerInventory>();
    }

    private void Start()
    {
        PiersEvent.Post(PiersEventKey.EventKey.CameraTargetBroadcast, _camera);
        _chickVisionCamera.gameObject.SetActive(false);
    }

    private void Update()
    {
        HandleInteraction();
        HandleFootStepAudio();
        
        if (!_canEquip)
        {
            _equipCooldownCounter -= Time.deltaTime;

            if (_equipCooldownCounter <= 0)
            {
                _canEquip = true;
            }
        }

        if (_chickVisionCount <= 0)
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                _chickVisionCount = _chickVisionCooldown;
                ToggleCamera();
            }
        }
        else
        {
            _chickVisionCount -= Time.deltaTime;
        }
    }

    
    private void HandleInteraction()
    {
        if (IsOwner && Input.GetKeyDown(KeyCode.E))
        {
            TriggerInteraction();
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void TriggerInteraction()
    {
        _localInteractionCallback?.Invoke(transform);
    }

    private void ToggleCamera()
    {
        if (_isMainCameraOn)
        {
            _camera.gameObject.SetActive(false);
            _chickVisionCamera.gameObject.SetActive(true);
            _isMainCameraOn = false;
        }
        else
        {
            _camera.gameObject.SetActive(true);
            _chickVisionCamera.gameObject.SetActive(false);
            _isMainCameraOn = true;
        }
    }

    private void HandleFootStepAudio()
    {
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");
        
        if (vertical == 0 && horizontal == 0)
            return;

        if (_footStepCount < _maxFootStepTime)
        {
            _footStepCount += Time.deltaTime;
        }
        else
        {
            OnFootStep();
            _maxFootStepTime = _normalFootStepTime;
            _footStepCount = 0;
        }
    }

    private void OnFootStep()
    {
        var soundToUse = Random.Range(0, 3);

        var track = soundToUse switch
        {
            0 => AudioTrackTypes.PlayerFootStepsGrass01,
            1 => AudioTrackTypes.PlayerFootStepsGrass02,
            2 => AudioTrackTypes.PlayerFootStepsGrass03,
            _ => AudioTrackTypes.PlayerFootStepsGrass01
        };

        AudioUtilityManager.Instance.PlaySound(transform, transform.position, track.ToString());
    }

    public Camera GetCamera()
    {
        return _camera;
    }

    public void EquipPickUp(string pickupID)
    {
        if (IsServer && _canEquip)
        {
            if (_inventory.GetEquippedItem() != EPickUpID.Nothing)
            {
                // picking up with something in hand, dropping item
                _inventory.DropEquippedItem();
            }

            _inventory.EquipPickUp(pickupID);

            OnEquip();
        }
    }

    public void SetInteractionCallback(UnityAction<Transform> callback)
    {
        Debug.Log("interaction callback set for player (is null): " + (callback == null));
        _localInteractionCallback = callback;
    }

    [ObserversRpc]
    public void SetNameLabel(string name)
    {
        _nameLabel.text = name;
    }

    private void OnEquip()
    {
        _canEquip = false;
        _equipCooldownCounter = _equipCooldown;
    }

    public EPickUpID GetEquippedPickUp()
    {
        return _inventory.GetEquippedItem();
    }

    public void TeleportToLocation(Vector3 position, bool canMoveAfterTele = true)
    {
        _characterPrediction.GetCharacterController().enabled = false;
        transform.position = position;
        _characterPrediction.GetCharacterController().enabled = true;
        _characterPrediction.SetWasTeleported(Owner, canMoveAfterTele);
    }

    public void BackStabFarmer(Transform attackTarget)
    {
        // Do a little stab animation on the weapon slot, so we can "use" things
    }

    public bool GetIsHidden()
    {
        return _isHidden;
    }

    public void SetHidden(bool isHidden)
    {
        _isHidden = isHidden;
    }
}
