using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Cells
{
    public abstract class Attack : MonoBehaviour
    {
        public abstract void UseAttack(Vector2 direction);
    }
}