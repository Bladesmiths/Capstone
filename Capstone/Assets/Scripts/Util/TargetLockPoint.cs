using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone {
    public class TargetLockPoint : MonoBehaviour
    {
        [field: SerializeField]
        public int ID { get; private set; }

        private void Start()
        {
            ID = transform.parent.GetComponent<IIdentified>().ID;
            transform.parent.GetComponent<Character>().OnDestruction += Disable;
        }

        private void Disable(int id)
        {
            GetComponent<Collider>().enabled = false;
        }
    }
}
