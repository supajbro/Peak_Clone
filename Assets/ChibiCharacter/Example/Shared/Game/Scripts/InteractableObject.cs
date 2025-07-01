using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChibiCharacter
{
    public class InteractableObject : MonoBehaviour
    {
        private enum InteractableObjectTypes
        {
            Object,
            Character
        }

        [SerializeField]
        [Tooltip("Type determines the animation that will play when interacting with this object")]
        private InteractableObjectTypes type;

        [SerializeField]
        [Tooltip("The object to show when interacting")]
        private GameObject objectToShow;

        // Start is called before the first frame update
        void Start()
        {

        }

        public string getAnimationTriggerName()
        {
            if (type == InteractableObjectTypes.Object)
            {
                return "interact";
            }
            else if (type == InteractableObjectTypes.Character)
            {
                return "wave";
            }
            else
            {
                Debug.LogError("animation type not found");
                return "interact";
            }

        }

        public void interact()
        {
            objectToShow.SetActive(true);
            StartCoroutine(disableObject());
        }

        /// <summary>
        /// hides object after 8 seconds
        /// </summary>
        /// <returns></returns>
        IEnumerator disableObject()
        {
            yield return new WaitForSeconds(8);
            objectToShow.SetActive(false);
        }

    }
}
