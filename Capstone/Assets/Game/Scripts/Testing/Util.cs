using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone.Testing
{
    public static class Util
    {
        public static IEnumerator DamageMaterialTimer(Renderer renderer)
        {
            renderer.material.color = Color.red;

            yield return new WaitForSeconds(1);

            renderer.material.color = Color.white;
        }

        public static IEnumerator DestroyTimer(float timeToDestruction, GameObject obj)
        {
            yield return new WaitForSeconds(timeToDestruction);

            GameObject.Destroy(obj);
        }
    }
}

