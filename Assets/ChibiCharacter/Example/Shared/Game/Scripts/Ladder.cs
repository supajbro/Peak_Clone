using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChibiCharacter
{

    public class Ladder : MonoBehaviour
    {
        [SerializeField]
        private int numRungs = 20;

        [SerializeField]
        private GameObject rung;

        // Start is called before the first frame update
        void Start()
        {
            for (int i = 1; i < numRungs; i++)
            {
                GameObject currentRung = Instantiate(rung, gameObject.transform);
                currentRung.transform.localPosition = new Vector3(0, i);
            }

        }

    }
}
