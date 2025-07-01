using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChibiCharacter
{
    public class DropdownSetAnimation : MonoBehaviour
    {
        private Animator anim;

        [SerializeField]
        private Dropdown dropdown;

        // Start is called before the first frame update
        void Start()
        {
            anim = gameObject.GetComponent<Animator>();
        }


        public void changeAnimation(int value)
        {
            //anim.SetInteger("animation", value);
            anim.Play(dropdown.options[value].text);
            //anim.CrossFade(dropdown.options[value].text, 0, 0);
        }

        public void changeAnimation(string newAnim)
        {
            //if (currentAnim=="")
            anim.Play(newAnim);
        }


    }
}
