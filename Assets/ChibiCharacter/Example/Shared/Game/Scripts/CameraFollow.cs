using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChibiCharacter
{
    public class CameraFollow : MonoBehaviour
    {

        public Transform player;

        float zPos;

        Vector3 pos;

        [SerializeField]
        Vector2 minPos;
        [SerializeField]
        Vector2 maxPos;

        // Start is called before the first frame update
        void Start()
        {
            zPos = gameObject.transform.position.z;
        }

        // Update is called once per frame
        void Update()
        {
            pos = new Vector3(player.position.x, player.position.y, zPos);

            pos.x = Mathf.Clamp(pos.x, minPos.x, maxPos.x);
            pos.y = Mathf.Clamp(pos.y, minPos.y, maxPos.y);

            gameObject.transform.position = pos;
        }
    }
}
