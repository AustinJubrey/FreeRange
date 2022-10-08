using System.Collections;
using FishNet.Object;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class PigController : NetworkBehaviour
{
    [SerializeField]
    private Animator _animator;
    
    [SerializeField]
    private BoxCollider _wanderBox;
    
    private NavMeshAgent _navMeshAgent;
    private Vector2 _destination = Vector2.zero;

    private Vector2 _wanderDelayRange;
    
    private bool _isWalking;
    private bool _isRunning;
    private bool _targetReached;
    
    // Foot Steps
    private float _footStepCount;
    private float _normalFootStepTime = 0.625f;
    private float _maxFootStepTime;
    
    void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _wanderDelayRange = new Vector2(2f, 8f);
        SetRandomWanderDestination();
    }

    private void Update()
    {
        if (!IsServer)
            return;
        
        HandleMovement();
        HandleFootStepAudio();
    }

    private void HandleMovement()
    {
        if (_destination != Vector2.zero)
        {
            _navMeshAgent.destination = new Vector3(_destination.x, transform.position.y, _destination.y);
        }
        
        if (!_targetReached && CheckHasReachedDestination())
            OnReachWanderDestination();
    }
    
    private void HandleFootStepAudio()
    {
        if (!_isWalking)
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

    private void SetRandomWanderDestination()
    {
        if (_wanderBox == null)
            return;
        
        _destination = GetRandomPointWithinBounds(_wanderBox.bounds);
        _targetReached = false;
    }

    private Vector2 GetRandomPointWithinBounds(Bounds bounds)
    {
        return new Vector2(Random.Range(bounds.min.x, bounds.max.x),Random.Range(bounds.min.z, bounds.max.z));
    }
    
    private bool CheckHasReachedDestination()
    {
        if (_navMeshAgent.pathPending)
            return false;
        
        if (!_isWalking)
        {
            _isWalking = true;
            _animator.SetBool("IsWalking", true);
        }

        if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
            return !_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude == 0f;

        return false;
    }
    
    private void OnReachWanderDestination()
    {
        _targetReached = true;
        _navMeshAgent.ResetPath();
        
        if (_isWalking)
        {
            _isWalking = false;
            _animator.SetBool("IsWalking", false);
        }

        StartCoroutine(DelayedGoToNextTarget(Random.Range(_wanderDelayRange.x, _wanderDelayRange.y)));
    }
    
    private IEnumerator DelayedGoToNextTarget(float delay)
    {
        bool stoppedForASnack = false;
        if (Random.Range(0, 4) == 0)
        {
            stoppedForASnack = true;
            _animator.SetBool("IsEating", true);
        }
        
        yield return new WaitForSeconds(delay);
        
        if(stoppedForASnack)
            _animator.SetBool("IsEating", false);

        SetRandomWanderDestination();
    }
}
