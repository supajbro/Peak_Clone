using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.IO;

namespace ChibiCharacter_Editor
{
#if UNITY_EDITOR
    public class CreateChibiPopup : EditorWindow
    {
        #region private variables
        const string urpShaderPath = "Chibi Character/URP";
        const string builtinShaderPath = "Chibi Character/Built-in";

        const string litName = "ColorSwap_Lit";
        const string unlitName = "ColorSwap_Unlit";

        /// <summary>
        /// The current folder of project user is in
        /// </summary>
        public static string CurrentProjectFolderPath
        {
            get
            {
                Type projectWindowUtilType = typeof(ProjectWindowUtil);
                MethodInfo getActiveFolderPath = projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
                object obj = getActiveFolderPath.Invoke(null, new object[0]);
                return obj.ToString();
            }
        }
        #endregion

        #region editable variables
        /// <summary>
        /// The base model for the chibi. This is what is spawned in the scene
        /// </summary>
        GameObject chibiBase;

        /// <summary>
        /// Determines chibi's name in the heirarchy and the name of its materials folder.
        /// </summary>
        string chibiName;

        int materialSelected = 0;
        string[] materialTypes = new string[] { "None", "URP", "Built-in" };

        bool isUnlit = false;

        Texture2D bodyTexture;
        Texture2D armTexture;
        Texture2D feetTexture;
        #endregion

        //priority=2500: Places it on top of General, but under TMP
        [MenuItem("Window/Chibi Character/Chibi Spawner", priority = 2500)]
        public static void ShowWindow()
        {
            CreateChibiPopup w = (CreateChibiPopup)GetWindow(typeof(CreateChibiPopup), false, "Chibi Spawner");
        }


        private void OnGUI()
        {
            int space = 10;

            GUILayout.Space(10);
            EditorGUILayout.LabelField("Create a Chibi", EditorStyles.largeLabel);

            EditorGUILayout.LabelField("Basic", EditorStyles.largeLabel);
            chibiName = EditorGUILayout.TextField("Chibi Name", chibiName);

            chibiBase = (GameObject)EditorGUILayout.ObjectField("Chibi Model", chibiBase, typeof(GameObject), false);

            ///the chibi has 7 children
            if (chibiBase == null || chibiBase.transform.childCount != 7)
            {
                EditorGUILayout.HelpBox("Please choose Chibi.fbx (located in Essentials folder) as the Chibi Base.", MessageType.Error);

                //don't show anything else if the chibi base is invalid
                return;
            }


            GUILayout.Space(space);
            EditorGUILayout.LabelField("Materials", EditorStyles.largeLabel);
            materialSelected = EditorGUILayout.Popup("Materials:", materialSelected, materialTypes);


            string errorMessage = "";

            //only show textures if it is creating an object with materials
            if (materialSelected != 0)
            {
                //choose whether lit or unlit
                isUnlit = EditorGUILayout.Toggle("Unlit?", isUnlit);

                //show location where materials will be saved
                EditorGUILayout.LabelField("Location: " + CurrentProjectFolderPath + "/" + chibiName + "/Materials");

                //show error messages about chibi name
                if (string.IsNullOrEmpty(chibiName) || chibiName.Trim() == "")
                {
                    errorMessage = "Chibi Name cannot be blank!";
                    EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
                }
                else if (Directory.Exists(CurrentProjectFolderPath + "/" + chibiName))
                {
                    //error message for if there is already a folder with that name
                    errorMessage = chibiName + " already exists in " + CurrentProjectFolderPath + " folder.\nChoose a different Chibi name or change the current directory in the Project.";
                    EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
                }
                else if (System.Text.RegularExpressions.Regex.IsMatch(chibiName, "[^A-Za-z0-9]"))
                {
                    errorMessage = "Chibi Name should not contain special characters or spaces.";
                    EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
                }

                //draw the textures
                GUILayout.Space(space);
                EditorGUILayout.LabelField("Textures", EditorStyles.largeLabel);
                EditorGUILayout.BeginHorizontal();
                bodyTexture = TextureField("Body", bodyTexture);
                armTexture = TextureField("Arms", armTexture);
                feetTexture = TextureField("Feet", feetTexture);
                EditorGUILayout.EndHorizontal();
            }


            //if there are no errors, then allow the button to spawn
            if (errorMessage == "")
            {
                EditorGUILayout.Space(space);

                if (GUILayout.Button("Spawn \'" + chibiName + "\'"))
                {
                    spawnChibi();
                }
            }

        }

        private static Texture2D TextureField(string name, Texture2D texture)
        {
            GUILayout.BeginVertical();
            GUILayout.Label(name);
            var result = (Texture2D)EditorGUILayout.ObjectField(texture, typeof(Texture2D), false, GUILayout.Width(70), GUILayout.Height(70));
            GUILayout.EndVertical();
            return result;
        }

        private void spawnChibi()
        {
            Debug.Log("Spawn the character with those specs");

            // Instantiate the prefab in the scene
            GameObject instance = PrefabUtility.InstantiatePrefab(chibiBase) as GameObject;
            instance.name = chibiName;

            //if creating materials with character
            if (materialSelected != 0)
            {
                Debug.Log("Creating materials...");

                string? materialPath = createMaterialsFolder(chibiName);

                //if material folder was successfully created
                if (materialPath != null)
                {
                    Material[] characterMaterials;
                    //if creating URP materials
                    if (materialSelected == 1)
                    {
                        characterMaterials = createMaterials(materialPath, urpShaderPath, isUnlit ? unlitName : litName);
                    }
                    //if creating built-in materials
                    else
                    {
                        characterMaterials = createMaterials(materialPath, builtinShaderPath, isUnlit ? unlitName : litName);
                    }

                    //assign those materials to the character
                    assignMaterials(instance, characterMaterials);

                }

            }


        }

        /// <summary>
        /// Creates a folder to hold the Chibi's materials.
        /// </summary>
        /// <param name="chibiName">The name of the folder to hold chibi's materials</param>
        private string? createMaterialsFolder(string chibiName)
        {
            //Creates a new folder to hold the chibi.
            //Note folder name may include a 1 if a chibi folder already exists.
            string chibiGuid = AssetDatabase.CreateFolder(CurrentProjectFolderPath, this.chibiName);
            string chibiFolderPath = AssetDatabase.GUIDToAssetPath(chibiGuid);

            //creates a subfolder called materials
            string materialGuid = AssetDatabase.CreateFolder(chibiFolderPath, "Materials");

            //error check
            if (string.IsNullOrEmpty(materialGuid))
            {
                string errorMessage = "Could not create a Material folder in " + chibiFolderPath;
                EditorUtility.DisplayDialog("Failed to Create Chibi", errorMessage, "OK");
                return null;
            }

            return AssetDatabase.GUIDToAssetPath(materialGuid);
        }

        /// <summary>
        /// Creates materials in the project. The materials are created in same order as children of the model, but with an offset of -1 since Armature is not recolorable. 
        /// </summary>
        /// <param name="materialFolderPath">The path to add the materials to</param>
        /// <param name="shaderPath">The path to the shader</param>
        /// <param name="shaderName">Determines the name of the shader under shaderPath.</param>
        /// <returns></returns>
        private Material[] createMaterials(string materialFolderPath, string shaderPath, string shaderName)
        {
            //there are 6 parts of the character that can be recolored
            //these materials should be in the same order as the children parts in the heirarchy
            int numChildren = 6;
            Material[] materials = new Material[numChildren];

            materials[0] = new Material(Shader.Find(shaderPath + "/" + shaderName));
            materials[0].mainTexture = armTexture;
            AssetDatabase.CreateAsset(materials[0], materialFolderPath + "/arms.mat");

            materials[1] = new Material(Shader.Find(shaderPath + "/" + shaderName));
            materials[1].mainTexture = bodyTexture;
            AssetDatabase.CreateAsset(materials[1], materialFolderPath + "/body.mat");

            //the eye already has the eye set as its default texture because of how the shader is defined
            materials[2] = new Material(Shader.Find(shaderPath + "/Eye"));
            AssetDatabase.CreateAsset(materials[2], materialFolderPath + "/eye.mat");

            materials[3] = new Material(Shader.Find(shaderPath + "/" + shaderName));
            materials[3].mainTexture = feetTexture;
            AssetDatabase.CreateAsset(materials[3], materialFolderPath + "/feet.mat");

            //Head does not yet have texture, so that field is blank
            materials[4] = new Material(Shader.Find(shaderPath + "/" + shaderName));
            materials[4].mainTexture = null;
            AssetDatabase.CreateAsset(materials[4], materialFolderPath + "/head.mat");

            materials[5] = new Material(Shader.Find(shaderPath + "/Mouth"));
            AssetDatabase.CreateAsset(materials[5], materialFolderPath + "/mouth.mat");

            AssetDatabase.SaveAssets(); // Save changes to the asset database
            return materials;
        }

        /// <summary>
        /// Assigns materials to the model
        /// </summary>
        /// <param name="spawnedModel">The scene model to assign materials to </param>
        /// <param name="materials">The materials to assign. Indices of these correspond to the children.</param>
        private void assignMaterials(GameObject spawnedModel, Material[] materials)
        {
            //set the materials for the parts
            for (int i = 0; i < materials.Length; i++)
            {
                //the 0th child is the armature
                spawnedModel.transform.GetChild(i + 1).GetComponent<SkinnedMeshRenderer>().material = materials[i];
            }
        }

    }
#endif
}