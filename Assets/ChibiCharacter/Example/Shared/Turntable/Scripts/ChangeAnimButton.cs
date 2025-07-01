using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChibiCharacter
{
    public class ChangeAnimButton : MonoBehaviour
    {
        [SerializeField]
        private DropdownSetAnimation character;

        [SerializeField]
        [Tooltip("The exact name of the animation")]
        private string animationName;

        [SerializeField]
        [Tooltip("An optional label to put on the button if not using animationName. If blank text will say animationName")]
        private string buttonLabel;

        [SerializeField]
        [Tooltip("The button text")]
        private Text buttonText;

        // Start is called before the first frame update
        void Start()
        {
            if (buttonLabel == "")
            {
                buttonText.text = animationName;
            }
            else
            {
                buttonText.text = buttonLabel;
            }

            gameObject.GetComponent<Button>().onClick.AddListener(buttonPressed);

        }

        private void buttonPressed()
        {
            Debug.Log("animaiton button pushed");
            character.changeAnimation(animationName);

        }


    }
}
