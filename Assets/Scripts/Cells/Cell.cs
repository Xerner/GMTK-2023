using Assets.Scripts.Extensions;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Cells
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Cell : MonoBehaviour, IPuppet
    {
        [SerializeField]
        protected Attack _attack;

        [SerializeField]
        protected Rigidbody2D _rigidbody;

        protected float Speed { get; set; }

        void Start()
        {
            this.EnsureHasReference(ref _attack);
            this.EnsureHasReference(ref _rigidbody);
        }

        public void Dash()
        {
            throw new System.NotImplementedException();
        }

        public float FaceToward(Vector2 faceToward)
        {
            throw new System.NotImplementedException();
        }

        public void Move(Vector2 moveToward)
        {
            // Speed check to prevent cell from moving faster than intented
            if(moveToward.magnitude > 1)
            {
                moveToward.Normalize();
            }

            var movementDelta = moveToward * Time.deltaTime * Speed;
            _rigidbody.MovePosition(movementDelta);
        }

        public void Shoot()
        {
            _attack.UseAttack(transform.forward);
        }
    }
}