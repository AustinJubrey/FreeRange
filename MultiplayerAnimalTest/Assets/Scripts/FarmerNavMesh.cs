using System.Collections;
using System.Collections.Generic;
using FishNet.Example.Prediction.CharacterControllers;
using FishNet.Object;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class FarmerNavMesh : NetworkBehaviour
{
    [SerializeField]
    private List<FarmerTarget> _targetList;
    
    [SerializeField]
    private Animator _animator;
    
    private List<ChickPlayerController> _targetPlayers;
    private Transform _chaseTarget;

    private NavMeshAgent _navMeshAgent;
    private Transform _targetTransform;
    private int _targetIndex;
    private bool _targetReached;
    private bool _isWalking;
    private bool _isRunning;
    private bool _isChasingTarget;
    private bool _isDowned;

    private float _chaseCaptureDistance = 2f;
    private float _fieldOfViewAngle = 30f;
    private float _chaseFieldOfViewAngle = 60f;
    private float _lostChaseCount;
    private float _lostChaseTimer = 5f;
    
    // Foot Steps
    private float _footStepCount;
    private float _normalFootStepTime = 0.35f;
    private float _maxFootStepTime;

    private float _defaultSpeed;
    private float _chaseSpeed;
    
    void Awake()
    {
        _targetPlayers = new List<ChickPlayerController>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _targetTransform = _targetList[_targetIndex].transform;
        _defaultSpeed = _navMeshAgent.speed;
        _chaseSpeed = _defaultSpeed * 1.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDowned)
            return;
        
        HandleMovement();
        HandleFootStepAudio();

        if (!_isChasingTarget)
        {
            var seenPlayer = CanSeePlayer();

            if (seenPlayer != null)
            {
                _isChasingTarget = true;
                _chaseTarget = seenPlayer;
            }
        }

        if (_isChasingTarget)
        {
            if (!CanSeeChaseTarget())
            {
                _lostChaseCount += Time.deltaTime;
                
                if(_lostChaseCount >= _lostChaseTimer)
                    OnStopChasing();
            }
            else if (_lostChaseCount > 0)
                _lostChaseCount = 0;
        }
    }

    private void HandleFootStepAudio()
    {
        if (!_isWalking && !_isRunning)
            return;

        if (_footStepCount < _maxFootStepTime)
        {
            _footStepCount += Time.deltaTime;
        }
        else
        {
            OnFootStep();
            _maxFootStepTime = _isRunning ? 0.25f : _normalFootStepTime;
            _footStepCount = 0;
        }
    }

    private void OnFootStep()
    {
        var soundToUse = Random.Range(0, 2);

        var track = soundToUse switch
        {
            0 => AudioTrackTypes.FarmerFootStepsDirt01,
            1 => AudioTrackTypes.FarmerFootStepsDirt02,
            _ => AudioTrackTypes.FarmerFootStepsDirt01
        };

        AudioUtilityManager.Instance.PlaySound(transform, transform.position, track.ToString());
    }

    public void GetDowned()
    {
        _isDowned = true;
        _isWalking = false;
        _isRunning = false;

        _targetTransform = null;
        
        if(_isChasingTarget)
            OnStopChasing();
        
        _animator.SetBool("IsWalking", false);
        _animator.SetBool("IsDowned", true);
        
        _navMeshAgent.ResetPath();
    }

    public void RecoverFromDowned()
    {
        _isDowned = false;
        _animator.SetBool("IsDowned", false);
        _targetTransform = _targetList[_targetIndex].transform;
    }

    private void HandleMovement()
    {
        if (!IsServer)
            return;
        
        if (_isChasingTarget && _chaseTarget)
        {
            HandleChasing();
        }
        else if (_targetTransform)
        {
            _navMeshAgent.destination = _targetTransform.position;
        }
        else
        {
            return;
        }

        if (!_targetReached && CheckHasReachedDestination())
            OnReachFarmerTarget();
    }

    private void HandleChasing()
    {
        if (!_isRunning)
        {
            OnStartChasing();
        }

        _navMeshAgent.destination = _chaseTarget.position;

        if (!IsServer)
            return;

        if (Vector3.Distance(transform.position, _chaseTarget.position) < _chaseCaptureDistance)
        {
            _chaseTarget.GetComponent<ChickPlayerController>().TeleportToLocation(FreeRangePlayerManager.Instance.GetCapturedChickSpawn().position);
            OnStopChasing();
        }
    }

    public void SetPlayerTargets(List<Transform> transforms)
    {
        _targetPlayers = new List<ChickPlayerController>();
        foreach (var playerTransform in transforms)
        {
            _targetPlayers.Add(playerTransform.GetComponent<ChickPlayerController>());
        }
    }

    private void OnStartChasing()
    {
        _navMeshAgent.speed = _chaseSpeed;
        _isRunning = true;
        _animator.SetBool("IsRunning", true);
        AudioUtilityManager.Instance.PlaySound(transform, transform.position, AudioTrackTypes.FarmerNoticedPlayerSound.ToString());
    }
    
    private void OnStopChasing()
    {
        _navMeshAgent.speed = _defaultSpeed;
        _chaseTarget = null;
        _isRunning = false;
        _isChasingTarget = false;
        _lostChaseCount = 0;
        _animator.SetBool("IsRunning", false);
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

    private void OnReachFarmerTarget()
    {
        _targetReached = true;
        _navMeshAgent.ResetPath();
        
        if (_isWalking)
        {
            _isWalking = false;
            _animator.SetBool("IsWalking", false);
        }
        
        if (_isRunning)
        {
            _isRunning = false;
            _animator.SetBool("IsRunning", false);
        }

        var farmerTarget = _targetList[_targetIndex];
        var animationString = farmerTarget.GetAnimationString();
        
        if(animationString != string.Empty)
            _animator.SetBool(animationString, true);

        StartCoroutine(DelayedGoToNextTarget(farmerTarget.GetDelayTime(), animationString != string.Empty ? animationString : null));
    }

    private Transform CanSeePlayer()
    {
        if (_targetPlayers == null || _targetPlayers.Count == 0)
            return null;

        var transformsToDelete = new List<ChickPlayerController>();

        foreach (var playerController in _targetPlayers)
        {
            if (playerController == null)
            {
                transformsToDelete.Add(playerController);
                continue;
            }

            if (playerController.GetIsHidden())
                continue;

            var player = playerController.transform;

            var targetDir = new Vector3(player.position.x, player.position.y + 0.5f, player.position.z) - transform.position;
            var angle = Vector3.Angle(targetDir, transform.forward);
            
            if (angle < _fieldOfViewAngle)
            {
                RemoveNullPlayersFromList(transformsToDelete);
                return CheckIfHasLineOfSight(targetDir) ? player : null;
            }
        }

        RemoveNullPlayersFromList(transformsToDelete);

        return null;
    }

    private void RemoveNullPlayersFromList(List<ChickPlayerController> toRemove)
    {
        if (toRemove == null || toRemove.Count == 0)
            return;
        
        foreach (var controller in toRemove)
        {
            _targetPlayers.Remove(controller);
        }
    }

    private bool CanSeeChaseTarget()
    {
        if (_chaseTarget == null)
            return false;
        
        var targetDir = new Vector3(_chaseTarget.position.x, _chaseTarget.position.y + 0.5f, _chaseTarget.position.z) - transform.position;
        var angle = Vector3.Angle(targetDir, transform.forward);

        if (angle < _chaseFieldOfViewAngle)
        {
            return CheckIfHasLineOfSight(targetDir);
        }

        return false;
    }

    private bool CheckIfHasLineOfSight(Vector3 playerDirection)
    {
        var hit = new RaycastHit();
        Physics.Raycast(transform.position, playerDirection, out hit);

        if (hit.transform.GetComponent<CharacterControllerPrediction>() != null)
        {
            return true;
        }

        return false;
    }

    private IEnumerator DelayedGoToNextTarget(float delay, string animationString = null)
    {
        yield return new WaitForSeconds(delay);
        
        if(animationString != null)
            _animator.SetBool(animationString, false);

        SetTarget(_targetList[GetNextTargetIndex()]);
    }

    private int GetNextTargetIndex()
    {
        _targetIndex++;
        
        if (_targetIndex > _targetList.Count - 1)
            _targetIndex = 0;

        return _targetIndex;
    }

    private void SetTarget(FarmerTarget target)
    {
        _targetTransform = target.transform;
        _targetReached = false;
    }
}
