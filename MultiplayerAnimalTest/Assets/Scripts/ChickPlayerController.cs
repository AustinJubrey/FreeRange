using FishNet.Example.Prediction.CharacterControllers;
using FishNet.Object;
using TMPro;
using UnityEngine;

public class ChickPlayerController : NetworkBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _nameLabel;
    
    [SerializeField]
    private Camera _camera;
    
    private CharacterControllerPrediction _characterPrediction;
    private PlayerInventory _inventory;
    private PlayerAnimatorHelper _animatorHelper;

    private float _equipCooldown = 1f;
    private float _equipCooldownCounter;
    private bool _canEquip = true;
    
    // Foot Steps
    private float _footStepCount;
    private float _normalFootStepTime = 0.35f;
    private float _maxFootStepTime;

    private void Awake()
    {
        _characterPrediction = GetComponent<CharacterControllerPrediction>();
        _inventory = GetComponent<PlayerInventory>();
        _animatorHelper = GetComponent<PlayerAnimatorHelper>();
    }

    private void Start()
    {
        PiersEvent.Post(PiersEventKey.EventKey.CameraTargetBroadcast, _camera);
    }

    private void Update()
    {
        HandleFootStepAudio();
        
        if (!_canEquip)
        {
            _equipCooldownCounter -= Time.deltaTime;

            if (_equipCooldownCounter <= 0)
            {
                _canEquip = true;
            }
        }
    }
    
    private void HandleFootStepAudio()
    {
        float vertical = Input.GetAxisRaw("Vertical");
        
        if (vertical == 0)
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

    public void TeleportToLocation(Vector3 position)
    {
        _characterPrediction.GetCharacterController().enabled = false;
        transform.position = position;
        _characterPrediction.GetCharacterController().enabled = true;
    }

    public void BackStabFarmer(Transform attackTarget)
    {
        // Do a little stab animation on the weapon slot, so we can "use" things
    }
}
