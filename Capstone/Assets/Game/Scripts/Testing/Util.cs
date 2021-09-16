using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone.Testing
{
    public static class Util
    {
        public static IEnumerator DamageMaterialTimer(GameObject obj)
        {
            obj.GetComponentInChildren<MeshRenderer>().material.color = Color.red;

            yield return new WaitForSeconds(1);

            obj.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
        }

        public static IEnumerator DestroyTimer(float timeToDestruction, GameObject obj)
        {
            yield return new WaitForSeconds(timeToDestruction);

            GameObject.Destroy(obj);
        }
    }
}

