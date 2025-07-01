using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChibiCharacter
{
    /// <summary>
    /// Basic platform effector 3d for example scene.
    /// Note that no rigidbodies (except player) should be on this platform as they may fall through!
    /// </summary>
    public class PlatformEffector3D : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody player;

        private Collider col;

        // Start is called before the first frame update
        void Start()
        {
            col = gameObject.GetComponent<Collider>();
        }

        // Update is called once per frame
        void Update()
        {
            //if player is moving up then player can pass through
            if (player.velocity.y > 0.1f)
            {
                col.enabled = false;
            }
            //if player is falling then they will collide with the platform
            else
            {
                col.enabled = true;
            }

        }
    }
}
