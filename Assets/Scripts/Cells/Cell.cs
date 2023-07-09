using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        protected float ShootInterval = 0.5f;
        private float _lastShootTime = 0;

        void Awake()
        {
            this.EnsureHasReference(ref _attack);
            this.EnsureHasReference(ref _rigidbody);
            SetHealth(_health);
            _rigidbody.drag = 5f;
        }

        void FixedUpdate()
        {
            if (ControllingPlayer == null)
            {
                enemyUpdate();
            }
            _currentDashCooldown = Mathf.Clamp(_currentDashCooldown - Time.deltaTime, 0f, _dashCooldown);
            OnDashCooldownChange?.Invoke(_dashCooldown, _currentDashCooldown);
        }
        public void OnTriggerEnter2D(Collider2D collision)
        {
            var damage = collision.gameObject.GetComponent<Projectile>().BulletStrength;
            if (LayerMask.LayerToName(gameObject.layer) == "Player")
            {
                SetHealth(_currentHealth - damage);
            }
            else if (LayerMask.LayerToName(gameObject.layer) == "Enemy")
            {
                SetHealth(_currentHealth - damage*2);
            }
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
                _rigidbody.AddForce(dashForce);
            }
        }

        public float FaceToward(Vector2 pointToFace)
        {
            var angle = Vector2.SignedAngle(Vector2.up, pointToFace - (Vector2)transform.position);
            _rigidbody.MoveRotation(angle);
            //Debug.Log(angle);

            return angle;
        }

        public void Move(Vector2 moveVector)
        {
            var movementDelta = moveVector * Speed;
            _rigidbody.AddForce(movementDelta);
        }

        public void Shoot()
        {
            _attack?.UseAttack((Vector2)transform.up);
        }

        private void enemyUpdate()
        {
            var playerLoc = Player.Instance.GetPosition();
            // Currently assumes player is ~1 unit in size
            // Maintain a min/max distance threshold from player
            maintainDistanceRangeFrom(playerLoc, 4f, 8f);

            foreach (var cell in getOtherCells())
            {
                maintainDistanceRangeFrom(cell.transform.position, 2f, 6f);
            }

            FaceToward(playerLoc);

            if (Time.time - _lastShootTime > ShootInterval)
            {
                Shoot();
                _lastShootTime = Time.time;
            }
        }

        private void maintainDistanceRangeFrom(Vector3 pos, float min, float max)
        {
            var delta = pos - transform.position;
            if (delta.magnitude > max)
            {
                Move(delta.normalized);
            }
            else if (delta.magnitude < min)
            {
                Move(-delta.normalized);
            }
            else
            {
                Move(Vector2.zero);
            }
        }

        private IEnumerable<Cell> getOtherCells()
        {
            // Note that we use GetType() instead of Cell because we want the current subclass
            // if there is one.
            return FindObjectsOfType(GetType()).Where(cell => cell != this).Cast<Cell>();
        }
    }
}
