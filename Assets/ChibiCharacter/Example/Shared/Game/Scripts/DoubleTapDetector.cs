using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChibiCharacter
{

    public class DoubleTapDetector
    {
        private Dictionary<KeyCode, float> DoubleTapDictionary;

        //the maximum time allowed between taps for a double tap to be registered
        //smaller values means user must do a faster double tap to count as a double-tap
        //bigger values allows double tap to be slower
        //values between 0.25 and 0.5 seem best (default is 0.5)
        const float maxDoubleTapType = 0.5f;

        public DoubleTapDetector()
        {
            DoubleTapDictionary = new Dictionary<KeyCode, float>();
        }

        #region adding keys to dictionary
        /// <summary>
        /// Adds a single key for detecting double taps
        /// </summary>
        /// <param name="key">The key to track for double taps</param>
        public void Add(KeyCode key)
        {
            DoubleTapDictionary.Add(key, 0);
        }

        /// <summary>
        /// Adds an array of keys, which will each detect double taps
        /// </summary>
        /// <param name="keys">The array of keys to track for double taps</param>
        public void Add(KeyCode[] keys)
        {
            foreach (KeyCode k in keys)
            {
                Add(k);
            }
        }

        /// <summary>
        /// Adds a list of keys, which will each detect double taps
        /// </summary>
        /// <param name="keys">The list of keys to track for double taps</param>
        public void Add(List<KeyCode> keys)
        {
            foreach (KeyCode k in keys)
            {
                Add(k);
            }
        }
        #endregion

        /// <summary>
        /// Returns true during the Update frame that the user starts the double tap of the key. (Does not work in FixedUpdate)
        /// </summary>
        /// <param name="key">The key to detect double tapped</param>
        /// <returns>True if the key was a double tap in the frame, false otherwise</returns>
        public bool GetKeyDoubleTap(KeyCode key)
        {
            if (DoubleTapDictionary.ContainsKey(key))
            {
                if (Input.GetKeyDown(key))
                {
                    if (Time.fixedUnscaledTime - DoubleTapDictionary[key] < maxDoubleTapType)
                    {
                        DoubleTapDictionary[key] = Time.fixedUnscaledTime;
                        return true;
                    }
                    else
                    {
                        DoubleTapDictionary[key] = Time.fixedUnscaledTime;
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                throw new System.Exception("Key " + key.ToString() + " not added to the DoubleTapDetector yet!");
            }

        }

    }
}

