using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Extensions
{
    public static class MonobehaviorExtensions
    {
        public static void EnsureHasReference<T>(this MonoBehaviour script, ref T field)
        {
            if (field == null)
            {                
                Debug.LogError($"{script.GetType().Name} script is missing field of type {typeof(T)}. Manually acquiring...");
                field = script.GetComponent<T>() ?? throw new System.Exception("Cannot be manually acquired... loser");
            }
        }
    }
}