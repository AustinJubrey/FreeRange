using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Prediction;
using UnityEngine;

/*
* 
* See TransformPrediction.cs for more detailed notes.
* 
*/

namespace FishNet.Example.Prediction.CharacterControllers
{

    public class CharacterControllerPrediction : NetworkBehaviour
    {
        #region Types.
        public struct MoveData
        {
            public float Horizontal;
            public float Vertical;
            public float MouseHorizontal;
            public bool Jump;
        }
        public struct ReconcileData
        {
            public Vector3 Position;
            public Quaternion Rotation;
            public float VerticalVelocity;
            public ReconcileData(Vector3 position, Quaternion rotation, float verticalVelocity)
            {
                Position = position;
                Rotation = rotation;
                VerticalVelocity = verticalVelocity;
            }
        }
        #endregion

        #region Serialized.
        [SerializeField]
        private float _moveRate;
        #endregion

        #region Private.
        private CharacterController _characterController;
        private Vector3 _rotationSpeed = new Vector3(0,270,0);
        private float _ySpeed = 0;
        private float _jumpHeight = 0.4f;
        private float _gravity = -20f;
        private float _terminalVelocity = -12f;
        private bool _canMove = true;
        
        // Jumping
        private bool _canJump = true;
        private float _jumpCount;
        private float _jumpCooldown = 0.33f;
        
        // Grounded check
        public float GroundedOffset = -0.5f;
        public float GroundedRadius = 0.2f;
        #endregion

        private void Awake()
        {
            InstanceFinder.TimeManager.OnTick += TimeManager_OnTick;
            InstanceFinder.TimeManager.OnPostTick += TimeManager_OnPostTick;
            _characterController = GetComponent<CharacterController>();
        }

        public CharacterController GetCharacterController()
        {
            return _characterController;
        }

        private void OnDestroy()
        {
            if (InstanceFinder.TimeManager != null)
            {
                InstanceFinder.TimeManager.OnTick -= TimeManager_OnTick;
                InstanceFinder.TimeManager.OnPostTick -= TimeManager_OnPostTick;
            }
        }

        private void TimeManager_OnTick()
        {
            if (IsOwner)
            {
                if (!_canJump)
                {
                    ManageJumpTimer();
                }

                Reconciliation(default, false);
                GroundedCheck();
                CheckInput(out MoveData md);
                Move(md, false);
            }
            if (IsServer)
            {
                Move(default, true);
                ReconcileData rd = new ReconcileData(transform.position, transform.rotation, _ySpeed);
                Reconciliation(rd, true);
            }
        }

        private void TimeManager_OnPostTick()
        {
            if (transform == null)
                return;
            
            ReconcileData rd = new ReconcileData(transform.position, transform.rotation, _ySpeed);
            Reconciliation(rd, true);
        }

        private void ManageJumpTimer()
        {
            _jumpCount -= (float) TimeManager.TickDelta;

            if (_jumpCount <= 0)
            {
                _jumpCount = _jumpCooldown;
                _canJump = true;
            }
        }

        private void CheckInput(out MoveData md)
        {
            md = default;

            float horizontal = _canMove ? Input.GetAxisRaw("Horizontal") : 0f;
            float vertical = _canMove ? Input.GetAxisRaw("Vertical") : 0f;
            float mouseHorizontal = Input.GetAxisRaw("Mouse X");
            bool jump = _canMove && _canJump && Input.GetKeyDown(KeyCode.Space);

            if (horizontal == 0f && vertical == 0f && mouseHorizontal == 0f && !jump)
                return;

            if (jump)
            {
                _canJump = false;
            }

            md = new MoveData()
            {
                Horizontal = horizontal,
                Vertical = vertical,
                MouseHorizontal = mouseHorizontal,
                Jump = jump
            };
        }

        [Replicate]
        private void Move(MoveData md, bool asServer, bool replaying = false)
        {
            if (md.MouseHorizontal != 0f)
            {
                var rotation = new Vector3(md.MouseHorizontal, 0f, 0f).normalized.x * _rotationSpeed * (float)TimeManager.TickDelta;
                transform.Rotate(rotation);
            }

            if (md.Jump)
            {
                _ySpeed = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
            }
            
            if(_ySpeed > _terminalVelocity)
                _ySpeed += _gravity * (float)TimeManager.TickDelta;
            
            Vector3 movementDirection = new Vector3(md.Horizontal, 0f, md.Vertical).normalized;

            _characterController.Move((transform.rotation * movementDirection * _moveRate * (float)TimeManager.TickDelta) + new Vector3(0f, _ySpeed, 0f) * (float)TimeManager.TickDelta);
        }

        [Reconcile]
        private void Reconciliation(ReconcileData rd, bool asServer)
        {
            transform.position = rd.Position;
            transform.rotation = rd.Rotation;
            _ySpeed = rd.VerticalVelocity;
        }

        [TargetRpc]
        public void SetWasTeleported(NetworkConnection connection, bool canMove)
        {
            MoveData md = new MoveData();
            md.Vertical = 0.1f;
            md.Horizontal = 0.1f;
            md.MouseHorizontal = 0.1f;
            Move(md, false);
            SetCanMove(canMove);
        }

        [ServerRpc(RunLocally = true)]
        private void SetCanMove(bool canMove)
        {
            _canMove = canMove;
        }

        [ServerRpc(RunLocally = true)]
        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y + GroundedOffset,
                transform.position.z);
            _canJump = _canJump && Physics.CheckSphere(spherePosition, GroundedRadius);
        }
    }


}