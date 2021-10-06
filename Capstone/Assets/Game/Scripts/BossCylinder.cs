using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class BossCylinder : MonoBehaviour
    {
        [SerializeField] private GameObject well;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.RotateAround(well.transform.position, well.transform.up, 180 * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.transform.root.CompareTag("Player") == true)
            {
                Player player = other.gameObject.transform.root.gameObject.GetComponent<Player>();
                player.TakeDamage(1);
            }
        }
    }
}
