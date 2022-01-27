using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone.Testing
{
    public static class Util
    {
        /// <summary>
        /// Sets the color of the given renderer to red for 1 second
        /// </summary>
        /// <param name="renderer">The renderer to change the color of. 
        /// Mesh Renderer and Skinned Mesh Renderer both inherit from this</param>
        /// <returns>Coroutine variable</returns>
        public static IEnumerator DamageMaterialTimer(Renderer renderer)
        {
            var originalColor = renderer.material.color;
            
            renderer.material.color = Color.red;

            yield return new WaitForSeconds(1);

            renderer.material.color = originalColor;
        }

        /// <summary>
        /// Destroys the given object after a given time
        /// </summary>
        /// <param name="timeToDestruction">How long to wait before destroying the object</param>
        /// <param name="obj">The object to destroy</param>
        /// <returns>Coroutine variable</returns>
        public static IEnumerator DestroyTimer(float timeToDestruction, GameObject obj)
        {
            yield return new WaitForSeconds(timeToDestruction);

            GameObject.Destroy(obj);
        }
    }
}

