using System;
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

        [SerializeField] float _health = 100f;
        float _currentHealth = 100f;
        public Action<float, float> OnHealthChange;

        public float Speed = 3f;
        public float DashForce = 1000f;
        public float DashSpeedTime = 1f;
        [SerializeField] float _dashCooldown = 2f;
        float _currentDashCooldown = 0f;
        public Action<float, float> OnDashCooldownChange;
        protected float ShootInterval = 0.3f;
        private float _lastShootTime = 0;

        void Awake()
        {
            this.EnsureHasReference(ref _attack);
            this.EnsureHasReference(ref _rigidbody);
            SetHealth(_health);
            _rigidbody.drag = 7f;
        }

        void FixedUpdate()
        {
            if (ControllingPlayer == null)
            {
                EnemyUpdate();
            }
            _currentDashCooldown = Mathf.Clamp(_currentDashCooldown - Time.deltaTime, 0f, _dashCooldown);
            OnDashCooldownChange?.Invoke(_dashCooldown, _currentDashCooldown);
        }

        public void SetHealth(float newHealth, float? totalHealth = null)
        {
            if (totalHealth != null)
                _health = totalHealth.Value;

            _currentHealth = newHealth;
            OnHealthChange?.Invoke(_health, _currentHealth);
        }

        public void Dash()
        {
            if (_currentDashCooldown <= 0f)
            {
                _currentDashCooldown = _dashCooldown;
                Vector2 dashForce = transform.up * DashForce;
                Debug.Log(dashForce);
                _rigidbody.AddForce(dashForce);
            }
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

            // FaceToward(playerLoc);

            if (Time.time - _lastShootTime > ShootInterval)
            {
                // Shoot();
                _lastShootTime = Time.time;
            }
        }
    }
}