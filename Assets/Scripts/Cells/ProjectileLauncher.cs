using System;
using System.Collections;
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

        void Start()
        {
            this.EnsureHasReference(ref _projectilePrefab);
        }

        public override void UseAttack(Vector2 direction) => Shoot(direction);

        /// <summary>
        /// Shoots projectiles
        /// </summary>
        protected virtual void Shoot(Vector2 direction)
        {
            // This method is deliberatly virtualized so other launcher variants can be extended
            var normalizeDir = direction.normalized;

            InitializeBullet(normalizeDir * .5f, normalizeDir);
        }

        private void InitializeBullet(Vector2 offset, Vector2 direction)
        {
            throw new NotImplementedException();
            // WIP code, this should be initialized with the normal facing the direction passed in
            //Instantiate(_projectilePrefab, offset, Quaternion.An)
        }
    }
}