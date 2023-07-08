using System.Collections;
using Assets.Scripts.Extensions;
using UnityEngine;

namespace Assets.Scripts.Cells
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Cell : MonoBehaviour, IPuppet
    {
        public Player ControllingPlayer;

        [SerializeField]
        protected Attack _attack;

        [SerializeField]
        protected Rigidbody2D _rigidbody;

        public float Speed = 3f;
        public float DashSpeedIncrease = 3f;
        public float DashSpeedTime = 1f;
        protected float ShootInterval = 0.3f;
        private float _lastShootTime = 0;

        void Awake()
        {
            this.EnsureHasReference(ref _attack);
            this.EnsureHasReference(ref _rigidbody);

            _rigidbody.drag = 7f;
        }

        void FixedUpdate()
        {
            if (ControllingPlayer == null)
            {
                EnemyUpdate();
            }
        }

        public void Dash()
        {
            LeanTween.value(gameObject, (value) => Speed = value, Speed, Speed + DashSpeedIncrease, 1f);
        }

        public float FaceToward(Vector2 pointToFace)
        {
            var angle = Vector2.SignedAngle(Vector2.up, pointToFace - (Vector2)transform.position);
            _rigidbody.MoveRotation(angle);
            return angle;
        }

        public void Move(Vector2 moveVector)
        {
            // Speed check to prevent cell from moving faster than intented
            // if (direction.magnitude > 1)
            // {
            //     direction.Normalize();
            // }
            var movementDelta = moveVector * Speed;
            _rigidbody.AddForce(movementDelta);
        }

        public void Shoot()
        {
            _attack?.UseAttack((Vector2)transform.up);
        }

        public void EnemyUpdate()
        {
            var playerLoc = Player.Instance.GetPosition();
            // Currently assumes player is ~1 unit in size
            // Maintain a min/max distance threshold from player
            var delta = playerLoc - transform.position;
            if (delta.magnitude > 8)
            {
                Move(delta.normalized);
            }
            else if (delta.magnitude < 4)
            {
                Move(-delta.normalized);
            }
            else
            {
                Move(Vector2.zero);
            }

            FaceToward(playerLoc);

            if (Time.time - _lastShootTime > ShootInterval)
            {
                Shoot();
                _lastShootTime = Time.time;
            }
        }
    }
}