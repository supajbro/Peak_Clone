using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChibiCharacter
{
    public class Bomb : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] particleSystems;

        [SerializeField]
        private float explosionRadius = 4;

        [SerializeField]
        private float gravityScale = 1;

        private Rigidbody rb;

        private bool inWater = false;

        // Start is called before the first frame update
        void Start()
        {
            rb = gameObject.GetComponent<Rigidbody>();
            rb.useGravity = false;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            Vector3 gravity = Physics.gravity.y * gravityScale * Vector3.up;
            rb.AddForce(gravity, ForceMode.Acceleration);
        }

        private void OnCollisionEnter(Collision collision)
        {
            //bomb will not explode in water
            if (inWater == false)
            {
                foreach (GameObject g in particleSystems)
                {
                    GameObject particle = Instantiate(g);
                    particle.transform.position = gameObject.transform.position;
                    g.GetComponent<ParticleSystem>().Play();
                }


                Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, explosionRadius);
                for (int i = hitColliders.Length - 1; i >= 0; i--)
                {
                    //if it hit the crate then destroy the crate
                    if (hitColliders[i].gameObject.name == "Crate")
                    {
                        GameObject.Destroy(hitColliders[i].gameObject, 0.1f);
                    }
                }
            }

            //destroy the bomb
            GameObject.Destroy(gameObject, 0.1f);
        }

        private void OnTriggerEnter(Collider other)
        {
            //water slows the the bomb and will prevent it from exploding
            if (other.gameObject.name == "Water")
            {
                rb.velocity = new Vector3(rb.velocity.x / 5, rb.velocity.y / 5, rb.velocity.z / 5);
                gravityScale = 1;
                inWater = true;
            }
        }
    }

}
