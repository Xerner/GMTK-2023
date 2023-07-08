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

        public float Speed = 1f;
        protected float ShootInterval = 0.3f;
        private float _lastShootTime = 0;

        void Start()
        {
            this.EnsureHasReference(ref _attack);
            this.EnsureHasReference(ref _rigidbody);
        }

        void Update()
        {
            if (ControllingPlayer == null)
            {
                EnemyUpdate();
            }
        }

        public void Dash()
        {
            throw new System.NotImplementedException();
        }

        public float FaceToward(Vector2 pointToFace)
        {
            var angle = Vector2.SignedAngle(Vector2.up, pointToFace - (Vector2)transform.position);
            _rigidbody.MoveRotation(angle);
            return angle;
        }

        public void Move(Vector2 direction)
        {
            // Speed check to prevent cell from moving faster than intented
            // if (direction.magnitude > 1)
            // {
            //     direction.Normalize();
            // }
            var movementDelta = direction * Speed;
            _rigidbody.velocity = movementDelta;
        }

        public void Shoot()
        {
            _attack?.UseAttack(transform.forward);
        }

        public void EnemyUpdate()
        {
            var playerLoc = Player.Instance.transform.position;
            // Currently assumes player is ~1 unit in size
            // Maintain distance of 5-10 units from player
            var delta = playerLoc - transform.position;
            if (delta.magnitude > 10)
            {
                Move(delta);
            }
            else if (delta.magnitude < 5)
            {
                Move(-delta);
            }
            else
            {
                Move(Vector2.zero);
            }

            // FaceToward(playerLoc);

            if (Time.time - _lastShootTime > ShootInterval)
            {
                // Shoot();
                _lastShootTime = Time.time;
            }
        }
    }
}