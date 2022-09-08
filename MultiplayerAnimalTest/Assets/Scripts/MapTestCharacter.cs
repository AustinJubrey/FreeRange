using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTestCharacter : MonoBehaviour
{
    [SerializeField]
    private float _moveRate;
    
    private Animator _animator;
    private bool _isWalkAnimating;
    private CharacterController _characterController;
    private Vector3 _rotationSpeed = new Vector3(0,180,0);

    // Start is called before the first frame update
    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput(out MoveData md);
        Move(md);
        HandleAnimation(md);
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
    
    private void HandleAnimation(MoveData md)
    {
        if (!_isWalkAnimating && md.Vertical > 0f)
        {
            _animator.SetBool("ShouldWalk", true);
            _isWalkAnimating = true;
        }
        else if (_isWalkAnimating && md.Vertical <= 0f)
        {
            _animator.SetBool("ShouldWalk", false);
            _isWalkAnimating = false;
        }
    }
    
    private void Move(MoveData md)
    {
        Vector3 move = new Vector3(0f, 0f, md.Vertical).normalized + new Vector3(0f, Physics.gravity.y, 0f);
        var rotation = new Vector3(md.Horizontal, 0f, 0f).normalized.x * _rotationSpeed * Time.deltaTime;
        transform.Rotate(rotation);
        _characterController.Move(transform.rotation * move * _moveRate * Time.deltaTime);
    }
}
