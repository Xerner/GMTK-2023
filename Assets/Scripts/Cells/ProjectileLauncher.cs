using Assets.Scripts.Extensions;
using UnityEngine;

namespace Assets.Scripts.Cells
{
    /// <summary>
    /// Base class for launching projectiles
    /// </summary>
    public class ProjectileLauncher : Attack
    {
        [SerializeField]
        private GameObject _projectilePrefab;

        [SerializeField]
        public AttackPattern _pattern;
        [SerializeField]
        float _overrideProjectileLifetime = -1f;
        [SerializeField]
        [Description("Equal to or less than zero overrides are considered invalid")]
        float _overrideProjectileSpeed = -1f;

        private Rigidbody2D _rigidbody;

        private float shotSpawnRadius = .5f;


        void Start()
        {
            this.EnsureHasReference(ref _projectilePrefab);

            TryGetComponent(out _rigidbody);
        }

        public override void UseAttack(Vector2 direction)
        {
            Shoot(direction, _overrideProjectileSpeed, _overrideProjectileLifetime);
        }

        /// <summary>
        /// Shoots projectiles, fr
        /// </summary>
        protected virtual void Shoot(Vector2 direction, float? overrideSpeed = null, float? overrideLifetimer = null)
        {
            // This method is deliberatly virtualized so other launcher variants can be extended
            var normalizeDir = direction.normalized;
            if (overrideSpeed <= 0) overrideSpeed = null;
            if (overrideLifetimer <= 0) overrideLifetimer = null;
            float speedOffset = 0f;
            if(_rigidbody != null)
            {
                // Speed offset added to bullet speeds when player is moving in same direction as shooting direction
                float dot = Vector2.Dot(normalizeDir, _rigidbody.velocity);
                speedOffset = Mathf.Max(speedOffset, dot);
            }

            switch (_pattern)
            {
                case AttackPattern.Single:
                    InitializeBullet(normalizeDir * shotSpawnRadius, normalizeDir, speedOffset, overrideSpeed, overrideLifetimer);
                    break;
                case AttackPattern.Arc3:
                    ShootArc3(normalizeDir, speedOffset, overrideSpeed, overrideLifetimer);
                    break;
                case AttackPattern.Arc5:
                    ShootArc5(normalizeDir, speedOffset, overrideSpeed, overrideLifetimer);
                    break;
                case AttackPattern.Burst:
                    ShootBurst(normalizeDir, speedOffset, overrideSpeed, overrideLifetimer);
                    break;
                case AttackPattern.Parallel:
                    ShootParallel(normalizeDir, speedOffset, overrideSpeed, overrideLifetimer);
                    break;
                case AttackPattern.Ring:
                    ShootRing(overrideSpeed, overrideLifetimer);
                    break;
            }
        }

        private void ShootArc3(Vector2 normalizedDirection, float speedOffset, float? overrideSpeed = null, float? overrideLifetimer = null)
        {
            var negDirection = Quaternion.AngleAxis(-15, Vector3.forward) * normalizedDirection;
            var posDirection = Quaternion.AngleAxis(15, Vector3.forward) * normalizedDirection;

            InitializeBullet(negDirection * shotSpawnRadius, negDirection, speedOffset, overrideSpeed, overrideLifetimer);
            InitializeBullet(normalizedDirection * shotSpawnRadius, normalizedDirection, speedOffset, overrideSpeed, overrideLifetimer);
            InitializeBullet(posDirection * shotSpawnRadius, posDirection, speedOffset, overrideSpeed, overrideLifetimer);
        }
        
        private void ShootArc5(Vector2 normalizedDirection, float speedOffset, float? overrideSpeed = null, float? overrideLifetimer = null)
        {
            var neg2Direction = Quaternion.AngleAxis(-20, Vector3.forward) * normalizedDirection;
            var negDirection = Quaternion.AngleAxis(-10, Vector3.forward) * normalizedDirection;
            var posDirection = Quaternion.AngleAxis(10, Vector3.forward) * normalizedDirection;
            var pos2Direction = Quaternion.AngleAxis(20, Vector3.forward) * normalizedDirection;

            InitializeBullet(neg2Direction * shotSpawnRadius, neg2Direction, speedOffset, overrideSpeed, overrideLifetimer);
            InitializeBullet(negDirection * shotSpawnRadius, negDirection, speedOffset, overrideSpeed, overrideLifetimer);
            InitializeBullet(normalizedDirection * shotSpawnRadius, normalizedDirection, speedOffset, overrideSpeed, overrideLifetimer);
            InitializeBullet(posDirection * shotSpawnRadius, posDirection, speedOffset, overrideSpeed, overrideLifetimer);
            InitializeBullet(pos2Direction * shotSpawnRadius, pos2Direction, speedOffset, overrideSpeed, overrideLifetimer);
        }   
        
        private void ShootBurst(Vector2 normalizedDirection, float speedOffset, float? overrideSpeed = null, float? overrideLifetimer = 1f)
        {
            for(int i = 0; i < 5; i++)
            {
                var angle = Random.Range(-30, 30);

                var direction = Quaternion.AngleAxis(angle, Vector3.forward) * normalizedDirection;
                InitializeBullet(direction * shotSpawnRadius, direction, speedOffset, overrideSpeed, overrideLifetimer);
            }
        }

        private void ShootParallel(Vector2 normalizedDirection, float speedOffset, float? overrideSpeed = 10f, float? overrideLifetimer = null)
        {
            var negDirection = Quaternion.AngleAxis(-30, Vector3.forward) * normalizedDirection;
            var posDirection = Quaternion.AngleAxis(30, Vector3.forward) * normalizedDirection;

            InitializeBullet(negDirection * shotSpawnRadius, normalizedDirection, speedOffset, overrideSpeed, overrideLifetimer);
            InitializeBullet(normalizedDirection * shotSpawnRadius, normalizedDirection, speedOffset, overrideSpeed, overrideLifetimer);
            InitializeBullet(posDirection * shotSpawnRadius, normalizedDirection, speedOffset, overrideSpeed, overrideLifetimer);
        }

        private void ShootRing(float? overrideSpeed = 2f, float? overrideLifetimer = 8f)
        {
            var shootVec = Vector2.up;
            var rotation = Quaternion.AngleAxis(30, Vector3.forward);


            for (var i = 0; i < 360; i += 30)
            {
                shootVec = rotation * shootVec;
                InitializeBullet(shootVec * shotSpawnRadius, shootVec, 0, overrideSpeed: overrideSpeed, overrideLifetimer);
            }
        }

        private void InitializeBullet(Vector2 offset, Vector2 direction, float speedOffset, float? overrideSpeed = null, float? overrideLifetimer = null)
        {
            if (overrideLifetimer.HasValue && overrideLifetimer.Value <= 0) 
                overrideLifetimer = null;

            if (overrideSpeed.HasValue && overrideSpeed.Value <= 0)
                overrideSpeed = null;

            var bulletGO = Instantiate(_projectilePrefab, transform.position + (Vector3)offset, Quaternion.FromToRotation(Vector3.up, direction));
            bulletGO.transform.parent = transform;
            Projectile projectile = bulletGO.GetComponent<Projectile>();
            bulletGO.layer = gameObject.layer;

            projectile.UpdateProperties(speedOffset, overrideSpeed, overrideLifetimer);
        }

        public enum AttackPattern
        {
            Single,
            Arc3,
            Arc5,
            Parallel,
            Burst,
            Ring
        }
    }
}