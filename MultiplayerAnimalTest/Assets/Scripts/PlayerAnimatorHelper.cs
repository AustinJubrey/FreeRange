using FishNet;
using FishNet.Component.Animating;
using FishNet.Example.Prediction.CharacterControllers;
using FishNet.Object;
using FishNet.Object.Prediction;
using UnityEngine;

public struct MoveData
{
    public float Horizontal;
    public float Vertical;
}

public class PlayerAnimatorHelper : NetworkBehaviour
{
    private Animator _animator;
    private NetworkAnimator _networkAnimator;
    private bool _isWalkAnimating;
    
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsOwner)
        {
            _animator = GetComponent<Animator>();
            _networkAnimator = GetComponent<NetworkAnimator>();
        }
    }

    private void Awake()
    {
        InstanceFinder.TimeManager.OnTick += TimeManager_OnTick;
    }
    
    private void OnDestroy()
    {
        if (InstanceFinder.TimeManager != null)
        {
            InstanceFinder.TimeManager.OnTick -= TimeManager_OnTick;
        }
    }
    
    private void CheckInput(out MoveData md)
    {
        md = default;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal == 0f && vertical == 0f)
            return;

        md = new MoveData()
        {
            Horizontal = horizontal,
            Vertical = vertical
        };
    }

    private void TimeManager_OnTick()
    {
        if (IsOwner)
        {
            Reconciliation(default, false);
            CheckInput(out MoveData md);
            HandleAnimation(md, false);
        }
        if (IsServer)
        {
            Reconciliation(default, true);
        }
    }

    [Replicate]
    private void HandleAnimation(MoveData md, bool asServer, bool replaying = false)
    {
        if (!_isWalkAnimating && (md.Vertical != 0f || md.Horizontal != 0f))
        {
            _animator.SetBool("ShouldWalk", true);
            _isWalkAnimating = true;
        }
        else if (_isWalkAnimating && md.Vertical == 0f && md.Horizontal == 0f)
        {
            _animator.SetBool("ShouldWalk", false);
            _isWalkAnimating = false;
        }
    }
    
    [Reconcile]
    private void Reconciliation(CharacterControllerPrediction.ReconcileData rd, bool asServer)
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
