using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChibiCharacter
{
    public class RandomizeCharacter : MonoBehaviour
    {
        [Header("Object references")]
        [SerializeField]
        private SkinnedMeshRenderer headMesh;

        [SerializeField]
        private SkinnedMeshRenderer eyeMesh;

        [SerializeField]
        private SkinnedMeshRenderer mouthMesh;

        [SerializeField]
        private SkinnedMeshRenderer bodyMesh;

        [SerializeField]
        private SkinnedMeshRenderer armMesh;

        [SerializeField]
        private SkinnedMeshRenderer feetMesh;


        [Header("Customizations")]
        [SerializeField]
        private Texture[] bodyOutfits;

        //Whether it is using URP or not. If not using URP it is using default built-in render pipeline
        [SerializeField]
        private bool isURP;


        // Start is called before the first frame update
        void Start()
        {
            InvokeRepeating("randomize", 3, 3);
        }

        /// <summary>
        /// Randomizes the entire character
        /// </summary>
        public void randomize()
        {
            //setting the head color is simple, since it has no fancy shader
            headMesh.material.color = getRandomColor();

            //randomize the expressions for the eye and mouth
            randomizeEyeExpression();
            randomizeMouthExpression();

            //randomize body outfit and color
            randomizeBody();

            //randomize the colors of the various parts
            randomizePartColor(eyeMesh.material);
            randomizePartColor(armMesh.material);
            randomizePartColor(feetMesh.material);

        }


        /// <summary>
        /// Returns a random color
        /// </summary>
        /// <returns></returns>
        private Color getRandomColor()
        {
            float r = Random.Range(0f, 1f);
            float g = Random.Range(0f, 1f);
            float b = Random.Range(0f, 1f);

            return new Color(r, g, b, 1);
        }



        /// <summary>
        /// Changes the expression of the eyes to a random expression (there are 16 eye expressions)
        /// </summary>
        private void randomizeEyeExpression()
        {
            //if using URP you have to call SetFloat instead of SetInteger
            if (isURP)
            {
                eyeMesh.material.SetFloat("_Expression", Random.Range(1, 17));
            }
            //default renderer:
            else
            {
                eyeMesh.material.SetInteger("_Expression", Random.Range(1, 17));
            }
        }

        /// <summary>
        /// Changes the expression of the mouth to a random expression (there are 4 mouth expressions)
        /// </summary>
        private void randomizeMouthExpression()
        {
            //if using URP you have to call SetFloat instead of SetInteger
            if (isURP)
            {
                mouthMesh.material.SetFloat("_Expression", Random.Range(1, 5));
            }
            //default renderer:
            else
            {
                mouthMesh.material.SetInteger("_Expression", Random.Range(1, 5));
            }
        }


        private void randomizeBody()
        {
            //choose a random outfit
            bodyMesh.material.mainTexture = getRandomBodyOutfit();

            //randomizes the body color
            randomizePartColor(bodyMesh.material);
        }


        private Texture getRandomBodyOutfit()
        {
            return bodyOutfits[Random.Range(0, bodyOutfits.Length)];
        }


        /// <summary>
        /// Randomizes the material's color
        /// </summary>
        /// <param name="material"></param>
        private void randomizePartColor(Material material)
        {
            //_RTarget controls main color
            material.SetColor("_RTarget", getRandomColor());

            // _GTarget controls the secondary color
            material.SetColor("_GTarget", getRandomColor());

            // _BTarget is usually the smaller details
            material.SetColor("_BTarget", getRandomColor());

        }

    }
}
