using System;
using System.Collections;
using System.Collections.Generic;
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

    private void SetRandomWanderDestination()
    {
        if (_wanderBox == null)
            return;
        
        Debug.Log("getting random destination");
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
        
        Debug.Log("reached destination");

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

        
        
        Debug.Log("waiting before next destination");
        yield return new WaitForSeconds(delay);
        
        if(stoppedForASnack)
            _animator.SetBool("IsEating", false);

        SetRandomWanderDestination();
    }
}
