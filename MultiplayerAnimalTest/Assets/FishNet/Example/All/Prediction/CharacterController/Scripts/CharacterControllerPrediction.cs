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
        }
        public struct ReconcileData
        {
            public Vector3 Position;
            public Quaternion Rotation;
            public ReconcileData(Vector3 position, Quaternion rotation)
            {
                Position = position;
                Rotation = rotation;
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
        private bool _canMove = true;
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
                Reconciliation(default, false);
                CheckInput(out MoveData md);
                Move(md, false);
            }
            if (IsServer)
            {
                Move(default, true);
            }
        }

        private void TimeManager_OnPostTick()
        {
            if (transform == null)
                return;
            
            ReconcileData rd = new ReconcileData(transform.position, transform.rotation);
            Reconciliation(rd, true);
        }

        private void CheckInput(out MoveData md)
        {
            md = default;

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            float mouseHorizontal = Input.GetAxisRaw("Mouse X");

            if (horizontal == 0f && vertical == 0f && mouseHorizontal == 0f)
                return;

            md = new MoveData()
            {
                Horizontal = horizontal,
                Vertical = vertical,
                MouseHorizontal = mouseHorizontal
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

            if (_canMove && (md.Horizontal != 0f || md.Vertical != 0f))
            {
                Vector3 move = new Vector3(md.Horizontal, 0f, md.Vertical).normalized + new Vector3(0f, Physics.gravity.y, 0f);
                _characterController.Move(transform.rotation * move * _moveRate * (float)TimeManager.TickDelta);
            }
        }

        [Reconcile]
        private void Reconciliation(ReconcileData rd, bool asServer)
        {
            transform.position = rd.Position;
            transform.rotation = rd.Rotation;
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


    }


}