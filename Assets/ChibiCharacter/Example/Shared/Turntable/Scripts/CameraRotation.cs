using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChibiCharacter
{
    public class CameraRotation : MonoBehaviour
    {
        [SerializeField]
        private Slider rotationSlider;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, rotationSlider.value, 0));
        }
    }
}
