using Assets.Scripts.Extensions;
using UnityEngine;

namespace Assets.Scripts.Cells
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody2D _rigidbody;

        private float LaunchSpeedOffset = 0f;

        public float BulletStrength = 10f;
        public float RemainingLifeTime { get; private set; } = 5f;
        public float Speed { get; private set; } = 4f;

        void Start()
        {
            this.EnsureHasReference(ref _rigidbody);
            SetRbVelocity();
        }

        void Update()
        {
            RemainingLifeTime -= Time.deltaTime;
            if (RemainingLifeTime < 0)
            {
                Destroy(gameObject);
            }
        }

        public void UpdateProperties(float playerSpeedOffset, float? speed = null, float? remainingLifetime = null)
        {
            // Speed offset added to bullet speeds when player is moving in same direction as shooting direction
            LaunchSpeedOffset = playerSpeedOffset;

            if (remainingLifetime.HasValue) 
            {
                RemainingLifeTime = remainingLifetime.Value;
            }

            if (speed.HasValue)
            {
                Speed = speed.Value;
                SetRbVelocity();
            }
        }

        private void SetRbVelocity() => _rigidbody.velocity = transform.up * (LaunchSpeedOffset + Speed);
        
    }
}