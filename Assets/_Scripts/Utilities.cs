using System.Collections;
using UnityEngine;

namespace _Scripts
{
    public static class Utilities
    {
        public static IEnumerator DestroyAfterDelay(GameObject gameObject, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (gameObject) Object.Destroy(gameObject);
        }
    }
}