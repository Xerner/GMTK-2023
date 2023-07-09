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
        #region Properties

        public Player ControllingPlayer;
        public SFXController audioController;

        bool suspended = false;
        public bool Activated = false;

        [SerializeField]
        protected Rigidbody2D _rigidbody;
        [SerializeField]
        protected SpriteRenderer _sprite;
        [SerializeField]
        protected SpriteRenderer _weakSpot;
        private float weakSpotAlpha = 0f;
        private float spriteAlpha = 1f;
        private float spriteFlashAlpha = 1f;

        [Header("Base Stats")]
        [SerializeField] float _currentHealth;
        [SerializeField] float totalHealth = 100f;
        [SerializeField]
        [Description("Seconds")]
        const float INVINCIBILITY_TWEEN_TIMING = 0.15f;
        const int INVINCIBILITY_TWEEN_PINGPONGS = 3;
        float invincibilityTime = INVINCIBILITY_TWEEN_TIMING * 2 * INVINCIBILITY_TWEEN_PINGPONGS; // x2 because it ping pongs
        float currentInvincibilityTime = 0f;

        public Action<float, float> OnHealthChange;

        public float Speed => IsPlayer
            ? BaseSpeed
            : 1f + (BaseSpeed * (_currentHealth / totalHealth));
        public float BaseSpeed = 3f;
        public float RotationSpeed => IsPlayer
            ? BaseRotationSpeed
            : 1f + (BaseRotationSpeed * (_currentHealth / totalHealth));
        private float BaseRotationSpeed = 15f;

        [Header("Attacks")]
        [SerializeField]
        protected Attack _attack;
        [SerializeField]
        [Description("Attacks per second")]
        protected float ShootInterval = 0.5f;
        private float _lastShootTime = 0;

        [Header("Dash")]
        public float DashForce = 1000f;
        public float DashSpeedTime = 1f;
        [SerializeField] float _dashCooldown = 2f;
        float _currentDashCooldown = 0f;
        public Action<float, float> OnDashCooldownChange;

        [Header("Assimilate - Player Only")]
        [SerializeField] float _assimilateCooldown = 5f;
        float _currentAssimilateCooldown = 5f;
        public Action<float, float> OnAssimilateCooldownChange;

        [Header("Enemy Stats")]
        [SerializeField] float minDistanceFromPlayer = 4f;
        [SerializeField] float maxDistanceFromPlayer = 8f;
        [SerializeField] float minDistanceFromEnemies = 2f;
        [SerializeField] float maxDistanceFromEnemies = 6f;

        public bool IsPlayer => ControllingPlayer != null;

        #endregion

        #region Unity Methods

        void OnValidate()
        {
            if (_sprite == null)
                _sprite = transform.FindFirst<SpriteRenderer>();
            if (_sprite == null)
                Debug.LogError("Could not find a SpriteRenderer", this);
        }

        void Awake()
        {
            this.EnsureHasReference(ref _attack);
            this.EnsureHasReference(ref _rigidbody);
            SetHealth(totalHealth);
            _rigidbody.drag = 5f;
            _rigidbody.angularDrag = 5f;

            // Runs forever
            var weakSpotTween = LeanTween.value(gameObject, (alpha) =>
            {
                weakSpotAlpha = alpha;
            }, 0f, 1f, 0.5f);
            weakSpotTween.setLoopPingPong();
        }

        void FixedUpdate()
        {
            if (ControllingPlayer == null)
            {
                enemyUpdate();
            }
            else
            {
                playerUpdate();
            }

            var finalAlpha = spriteAlpha * spriteFlashAlpha * (suspended ? 0.35f : 1f);
            var color = _sprite.color;
            color.a = finalAlpha;
            _sprite.color = color;
            var light = GetComponentInChildren<UnityEngine.Rendering.Universal.Light2D>();
            if (light != null)
                light.intensity = finalAlpha;

            _currentDashCooldown = Mathf.Clamp(_currentDashCooldown - Time.deltaTime, 0f, _dashCooldown);
            OnDashCooldownChange?.Invoke(_dashCooldown, _currentDashCooldown);
            if (IsPlayer && ControllingPlayer.IsAssimilating)
            {
                _currentAssimilateCooldown = Mathf.Clamp(_currentAssimilateCooldown - Time.deltaTime, 0f, _assimilateCooldown);
                OnAssimilateCooldownChange?.Invoke(_assimilateCooldown, _currentAssimilateCooldown);
            }
            else
            {
                OnAssimilateCooldownChange?.Invoke(_assimilateCooldown, _assimilateCooldown);
            }
            currentInvincibilityTime -= Time.deltaTime;
        }
        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (suspended) return;
            if (IsPlayer
                && collision.gameObject.TryGetComponent<WeakSpot>(out var weakspot)
                && weakspot.transform.parent.gameObject.TryGetComponent<Cell>(out var cellToTakeover))
            {
                TakeoverCell(cellToTakeover);
            }

            // Uh oh, stinky
            if (collision.gameObject.layer != gameObject.layer)
            {
                if (collision.gameObject.TryGetComponent<Projectile>(out var projectile))
                    TakeDamage(projectile.BulletStrength);
            }
        }

        #endregion

        public void TakeoverCell(Cell cellToTakeover)
        {
            audioController.PlayRandom(audioController.invadeAudio);
            ControllingPlayer.SwapControl(cellToTakeover);
        }

        public void TakeDamage(float damage)
        {
            if (IsPlayer)
            {
                if (currentInvincibilityTime <= 0f)
                {
                    currentInvincibilityTime = invincibilityTime;
                    PlayDamagedAnimation();
                }
                else
                {
                    // not so stinky
                    return;
                }
            }

            audioController.PlayRandom(audioController.hitAudio);
            if (LayerMask.LayerToName(gameObject.layer) == "Player")
                SetHealth(_currentHealth - damage);
            else if (LayerMask.LayerToName(gameObject.layer) == "Enemy")
                SetHealth(_currentHealth - damage * 2);
        }

        public void SetHealth(float newHealth, float? totalHealth = null)
        {
            if (totalHealth != null)
                this.totalHealth = totalHealth.Value;

            _currentHealth = newHealth;
            OnHealthChange?.Invoke(_currentHealth, this.totalHealth);

            if (!IsPlayer && _currentHealth <= 0f)
            {
                suspended = true;
            }
        }

        public void PlayDamagedAnimation()
        {
            LTDescr descr = LeanTween.value(gameObject, a => spriteFlashAlpha = a, 1f, 0f, INVINCIBILITY_TWEEN_TIMING);
            descr.setLoopPingPong(INVINCIBILITY_TWEEN_PINGPONGS);
        }

        public void Dash()
        {
            if (suspended) return;
            if (_currentDashCooldown <= 0f)
            {
                _currentDashCooldown = _dashCooldown;
                Vector2 dashForce = transform.up * DashForce;
                _rigidbody.AddForce(dashForce);
                audioController.PlayRandom(audioController.dashAudio);
            }
        }

        public float FaceToward(Vector2 pointToFace)
        {
            var desiredFacing = (pointToFace - (Vector2)transform.position);
            var desiredRot = Quaternion.LookRotation(forward: desiredFacing, upwards: Vector3.back);
            var curRot = Quaternion.Euler(0, 0, _rigidbody.rotation);
            var limited = Quaternion.RotateTowards(curRot, desiredRot, RotationSpeed);
            _rigidbody.MoveRotation(limited);

            return 0; // Quaternion.Angle(transform.rotation, desiredRot);
        }

        public void Move(Vector2 moveVector)
        {
            if (suspended) return;
            var movementDelta = moveVector * Speed;
            _rigidbody.AddForce(movementDelta);
        }

        public void Shoot()
        {
            if (suspended) return;
            _attack?.UseAttack((Vector2)transform.up);
            if (IsPlayer)
            {
                audioController.PlayRandom(audioController.shootAudio);
            }
        }

        private void enemyUpdate()
        {
            if (_weakSpot != null)
            {
                Color wsColor = _weakSpot.color;
                wsColor.a = weakSpotAlpha * (1 - (_currentHealth / totalHealth));
                _weakSpot.color = wsColor;
            }
            if (suspended) return;

            var playerLoc = Player.Instance.GetPosition();

            if (!Activated)
            {
                if (Vector2.Distance(playerLoc, transform.position) < 20f)
                {
                    Activated = true;
                }
                else
                {
                    return;
                }
            }

            // Maintain a min/max distance threshold from player
            maintainDistanceRangeFrom(playerLoc, minDistanceFromPlayer, maxDistanceFromPlayer);

            foreach (var cell in getOtherCells())
            {
                if (!cell.Activated) continue;
                maintainDistanceRangeFrom(cell.transform.position, minDistanceFromEnemies, maxDistanceFromEnemies);
            }

            FaceToward(playerLoc);

            if (Time.time - _lastShootTime > ShootInterval)
            {
                Shoot();
                _lastShootTime = Time.time;
            }
        }

        private void playerUpdate()
        {
            if (_weakSpot != null)
            {
                Color wsColor = _weakSpot.color;
                wsColor.a = 0;
                _weakSpot.color = wsColor;
            }

            if (_currentHealth <= 0)
            {
                StartCoroutine(die());
            }
        }

        private IEnumerator die()
        {
            spriteAlpha = 0;
            yield return new WaitForSeconds(1.5f);
            Application.LoadLevel("MainMenu");
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

        public void Suspend()
        {
            suspended = true;
            if (IsPlayer)
                gameObject.SetActive(false);
            else
                Destroy();
        }

        public void Resume()
        {
            suspended = false;
            if (IsPlayer)
                gameObject.SetActive(true);
        }

        public void Destroy()
        {
            if (!IsPlayer) Destroy(this);
        }
    }
}
