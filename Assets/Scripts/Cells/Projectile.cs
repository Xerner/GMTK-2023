using Assets.Scripts.Extensions;
using UnityEngine;

namespace Assets.Scripts.Cells
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour
    {
        private Rigidbody2D _rigidbody;

        public float Speed = 4f;

        void Start()
        {
            this.EnsureHasReference(ref _rigidbody);
        }

        void FixedUpdate()
        {
            _rigidbody.MovePosition(transform.forward * Speed);
        }
    }
}