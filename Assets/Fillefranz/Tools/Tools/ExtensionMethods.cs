using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FillefranzTools;

namespace FillefranzTools
{
    public static class ExtensionMethods
    {

        #region Vectors

        #region Vector2
        /// <summary>
        /// Returns a (X, Z) vector.
        /// </summary>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static Vector2 FromXZ(this Vector3 v3)
        {
            return new Vector2(v3.x, v3.z);
        }

        /// <summary>
        /// Returns a (Y, X) vector.
        /// </summary>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector2 FlipAxis(this Vector2 v2)
        {
            return new Vector2(v2.y, v2.x);
        }

        /// <summary>
        /// Returns a (Y, X) vector.
        /// </summary>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector2Int FlipAxis(this Vector2Int v2)
        {
            return new Vector2Int(v2.y, v2.x);
        }

        /// <summary>
        /// Returns X * Y.
        /// </summary>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static int Area(this Vector2Int v2)
        {
            return v2.x * v2.y;
        }

        /// <summary>
        /// Returns X * Y.
        /// </summary>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static float Area(this Vector2 v2)
        {
            return v2.x * v2.y;
        }

        public static float Average(this Vector2 v2)
        {
            return (v2.x + v2.y)/2; 
        }

        public static int Average(this Vector2Int v2)
        {
            return (v2.x + v2.y) / 2;
        }

        #endregion

        #region Vector3
        /// <summary>
        /// Returns a (X, 0, Y) vector.
        /// </summary>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static Vector3 ToXZ(this Vector2 v3)
        {
            return new Vector3(v3.x, 0, v3.y);
        }

        public static bool IsBetweenAB(this Vector3 c, Vector3 a, Vector3 b)
        {
            return Vector3.Dot((b - a).normalized, (c - b).normalized) < 0f && Vector3.Dot((a - a).normalized, (c - a).normalized) < 0f;
        }


        /// <summary>
        /// Returns a (X, 0, Y) vector.
        /// </summary>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static Vector3 ToXZ(this Vector2Int v3)
        {
            return new Vector3(v3.x, 0, v3.y);
        }

        /// <summary>
        /// Replaces the y value of the vector with the given value.
        /// </summary>
        /// <param name="v3"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Vector3 OverrideY(this Vector3 v3, float y)
        {
            return new Vector3(v3.x, y, v3.z);
        }

        /// <summary>
        /// Returns a (Z, Y, X) vector.
        /// </summary>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static Vector3 FlipXZ(this Vector3 v3)
        {
            return new Vector3(v3.z, v3.y, v3.x);
        }

        /// <summary>
        /// Snaps all values to the vector 1, - 1 or 0.
        /// </summary>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector2Int To01(this Vector2Int v2)
        {

            Vector2Int ret = Vector2Int.zero;

            if (v2.x != 0)
            {
                ret.x = (int)Mathf.Sign(v2.x);
            }


            if (v2.y != 0)
            {
                ret.y = (int)Mathf.Sign(v2.y);
            }


            return ret;

        }


        /// <summary>
        /// Returns the closest of 4 directions this vector is pointing (up, down, left or right).
        /// </summary>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector2Int Simplify(this Vector2Int v2)
        {
            if (Mathf.Abs(v2.x) > Mathf.Abs(v2.y))
            {
                return new Vector2Int((int)Mathf.Sign(v2.x), 0);
            }

            else
            {
                return new Vector2Int(0, (int)Mathf.Sign(v2.y));
            }
        }

        /// <summary>
        /// Returns the closest of 4 directions this vector is pointing (up, down, left or right).
        /// </summary>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector2Int FourDirectional(this Vector2 v2)
        {
            if (Mathf.Abs(v2.x) > Mathf.Abs(v2.y))
            {
                return new Vector2Int((int)Mathf.Sign(v2.x), 0);
            }

            else
            {
                return new Vector2Int(0, (int)Mathf.Sign(v2.y));
            }
        }

        /// <summary>
        /// Rotates a up, down, left or right vector 90 degrees.
        /// </summary>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector2Int RotateFourDirectional(this Vector2Int v2)
        {
            Vector2Int vector = v2.Simplify();

            if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
            {
                return new Vector2Int(0, vector.x);
            }

            else
            {
                return new Vector2Int(-vector.y, 0);
            }
        }

        #endregion

        #endregion

        #region Math

        /// <summary>
        /// Returns value * value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Square(this int value)
        {
            return value * value;
        }

        /// <summary>
        /// Returns value * value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float Square(this float value)
        {
            return value * value;
        }

        /// <summary>
        /// Raises this int to the given power.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="power"></param>
        /// <returns></returns>
        public static int Pow(this int value, int power)
        {
            return Mathf.RoundToInt(Mathf.Pow(value, power));
        }

        /// <summary>
        /// Raises this float to the given power.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="power"></param>
        /// <returns></returns>
        public static float Pow(this float value, float power)
        {
            return Mathf.Pow(value, power);
        }

        /// <summary>
        /// Returns the nth root of value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="root"></param>
        /// <returns></returns>
        public static float Root(this float value, float root)
        {
            return Mathf.Pow(value, 1 / root);
        }

        /// <summary>
        /// Returns the average value of this array
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static int Average(this int[] values)
        {
            int totalValue = 0;

            for (int i = 0; i < values.Length; i++)
            {
                totalValue += values.Length;
            }

            return totalValue / values.Length;
        }

        /// <summary>
        /// Returns the average value of the array
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static float Average(this float[] values)
        {
            float totValue = 0;

            for (int i = 0; i < values.Length; i++)
            {
                totValue += values.Length;
            }

            return totValue / values.Length;
        }

        /// <summary>
        /// Returns the highest value in the array.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int Max(this int[] array)
        {
            int max = int.MinValue;

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] > max)
                {
                    max = array[i];
                }
            }

            return max;
        }

        /// <summary>
        /// Returns true if this int is not divisible by 2.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsOdd(this int value)
        {
            if (value % 2 == 0)
            {
                return false;
            }

            else
            {
                return true;
            }
        }

        /// <summary>
        /// Returns true if this int is divisible by 2.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEven(this int value)
        {
            if (value % 2 == 0)
            {
                return true;
            }

            else
            {
                return false;
            }
        }



        #endregion

        #region Rounding
        /// <summary>
        /// Rounds this float to the nearest int.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Round(this float value)
        {
            return Mathf.RoundToInt(value);
        }
        /// <summary>
        /// Rounds this float down to the nearest int.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Floor(this float value)
        {
            return Mathf.FloorToInt(value);
        }

        /// <summary>
        /// Rounds this float up to the nearest int.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Ceil(this float value)
        {
            return Mathf.CeilToInt(value);
        }

        /// <summary>
        /// Returns this Vector2 rounded to a Vector2Int.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector2Int Round(this Vector2 vector)
        {
            return new Vector2Int(vector.x.Round(), vector.y.Round());
        }

        /// <summary>
        /// Rounds this Vector2 down to nearest Vector2Int.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector2Int Floor(this Vector2 vector)
        {
            return new Vector2Int(vector.x.Floor(), vector.y.Floor());
        }

        /// <summary>
        /// Rounds this Vector2 up to nearest Vector2Int.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector2Int Ceil(this Vector2 vector)
        {
            return new Vector2Int(vector.x.Ceil(), vector.y.Ceil());
        }

        /// <summary>
        /// Returns this Vector3 rounded to a Vector3Int.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>

        public static Vector3Int Round(this Vector3 vector)
        {
            return new Vector3Int(vector.x.Round(), vector.y.Round(), vector.z.Round());
        }

        /// <summary>
        /// Rounds this Vector3 down to nearest Vector3Int.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3Int Floor(this Vector3 vector)
        {
            return new Vector3Int(vector.x.Floor(), vector.y.Floor(), vector.z.Floor());
        }

        /// <summary>
        /// Rounds this Vector3 up to nearest Vector3Int.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3Int Ceil(this Vector3 vector)
        {
            return new Vector3Int(vector.x.Ceil(), vector.y.Ceil(), vector.z.Ceil());
        }

        #endregion

        #region Collections

        /// <summary>
        /// Makes sure there is only one of each item in the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> RemoveDuplicates<T>(this List<T> list)
        {
            List<T> newList = new List<T>();

            for (int i = 0; i < list.Count; i++)
            {
                if (!newList.Contains(list[i]))
                {
                    newList.Add(list[i]);
                }
            }

            return newList;

        }

        /// <summary>
        /// Counts the amount of times a certain item is in the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Occurrences<T>(this List<T> list, T value)
        {
            int amount = 0;

            for (int i = 0; i < list.Count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(list[i], value))
                {
                    amount++;
                }
            }

            return amount;
        }

        /// <summary>
        /// Counts the amount of times a certain item is in the array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Occurrences<T>(this T[] array, T value)
        {
            int amount = 0;

            for (int i = 0; i < array.Length; i++)
            {
                if (EqualityComparer<T>.Default.Equals(array[i], value))
                {
                    amount++;
                }
            }

            return amount;
        }

        /// <summary>
        /// Counts the occurences of and returns a list of items that occur more than minAmount times.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="minAmount"></param>
        /// <returns></returns>
        public static List<T> LimitOccurencesToAmount<T>(this List<T> list, int minAmount)
        {
            List<T> newList = new List<T>();


            for (int i = 0; i < list.Count; i++)
            {
                if (list.Occurrences(list[i]) >= minAmount && !newList.Contains(list[i]))
                {
                    newList.Add(list[i]);
                }
            }


            return newList;

        }

        /// <summary>
        /// Returns true if the given value is inside array bounds.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool IsInBounds<T>(this T[] values, int index)
        {
            return index < values.Length && index >= 0;
        }

        /// <summary>
        /// Returns true if the given values are inside array bounds.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        /// <returns></returns>
        public static bool IsInBounds<T>(this T[,] values, int index1, int index2)
        {
            int xSize = values.GetLength(0);
            int ySize = values.GetLength(1);

            return index1 >= 0 && index1 < xSize && index2 >= 0 && index2 < ySize;
        }

        /// <summary>
        /// Returns true if the given values are inside array bounds.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        /// <param name="index3"></param>
        /// <returns></returns>
        public static bool IsInBounds<T>(this T[,,] values, int index1, int index2, int index3)
        {
            int xSize = values.GetLength(0);
            int ySize = values.GetLength(1);
            int zSize = values.GetLength(2);


            return index1 >= 0 && index1 < xSize && index2 >= 0 && index2 < ySize && index3 >= 0 && index3 < zSize;
        }

        /// <summary>
        /// Returns true if the array contains the given value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool Contains<T>(this T[] collection, T item)
        {
            for (int i = 0; i < collection.Length; i++)
            {
                if (collection[i].Equals(item))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns a random item from the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T Random<T>(this List<T> list)
        {
            if(list.Count == 0) return default;

            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        /// <summary>
        /// Returns a random item from the array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static T Random<T>(this T[] array)
        {
            if (array.Length == 0) return default;
            return array[UnityEngine.Random.Range(0, array.Length)];
        }

        /// <summary>
        /// Returns a random item from the array that isn't the specified item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static T RandomExcluding<T>(this T[] array, T item)
        {
            List<T> container = new List<T>();
            container.AddRange(array);

            if (container.Contains(item))
                container.Remove(item);

            return container.Random();
        }

        /// <summary>
        /// Returns a random item from the array that isn't in the specified excluded array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="excludedArray"></param>
        /// <returns></returns>
        public static T RandomExcluding<T>(this T[] array, T[] excludedArray)
        {
            List<T> container = new List<T>();


            for (int i = 0; i < array.Length; i++)
            {
                if (!excludedArray.Contains(array[i]))
                {
                    container.Add(array[i]);
                }
            }

            return container.Random();
        }

        /// <summary>
        /// Returns a random item from the list that isn't the specified item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static T RandomExcluding<T>(this List<T> list, T item)
        {
            return list.ToArray().RandomExcluding(item);
        }
        /// <summary>
        /// Returns a random item from the list that isn't in the specified excluded array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="excludedArray"></param>
        /// <returns></returns>
        public static T RandomExcluding<T>(this List<T> list, T[] excludedArray)
        {
            return list.ToArray().RandomExcluding(excludedArray);
        }

        public static T[] ReverseOrder<T>(this T[] array)
        {
            if (array == null) return null;

            T[] result = new T[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                result[i] = array[array.Length - i - 1];
            }

            return result;
        }

        /// <summary>
        /// A foreach loop that loops through the given array in a random order and calls a forEach(int i) function for each item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="forEach"></param>
        public static void ForEachRandom<T>(this T[] collection, Helper.ForEach forEach)
        {
            System.Random r = new System.Random();
            foreach (int i in Enumerable.Range(0, collection.Length).OrderBy(x => r.Next()))
            {
                forEach(i);
            }
        }
        /// <summary>
        /// A foreach loop that loops through the given list in a random order and calls a forEach(int i) function for each item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="forEach"></param>
        public static void ForEachRandom<T>(this List<T> collection, Helper.ForEach forEach)
        {
            System.Random r = new System.Random();
            foreach (int i in Enumerable.Range(0, collection.Count).OrderBy(x => r.Next()))
            {
                forEach(i);
            }
        }
        /// <summary>
        /// A foreach loop that loops through the given list in a seeded random order and calls a forEach(int i) function for each item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="forEach"></param>
        /// <param name="seed"></param>
        public static void ForEachRandom<T>(this List<T> collection, Helper.ForEach forEach, int seed)
        {
            System.Random r = new System.Random(seed);
            foreach (int i in Enumerable.Range(0, collection.Count).OrderBy(x => r.Next()))
            {
                forEach(i);
            }
        }

        /// <summary>
        /// Returns true if the list contains any item from the other list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool ContainsAny<T>(this List<T> collection, List<T> other)
        {
            for (int i = 0; i < other.Count; i++)
            {
                if (collection.Contains(other[i]))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Noise

        /// <summary>
        /// Applies a Gaussian Blur effect to the noise map.
        /// </summary>
        /// <param name="noiseMap"></param>
        /// <param name="lengthKernel"></param>
        /// <returns></returns>
        public static float[,] ApplyGaussianBlur(this float[,] noiseMap, int lengthKernel)
        {
            if (lengthKernel <= 0) return noiseMap;

            int width = noiseMap.GetLength(0);
            int height = noiseMap.GetLength(1);
            float[,] result = new float[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    result[x, y] = BlurPixel(x, y);
                }
            }

            float BlurPixel(int x, int y)
            {
                float combinedValue = 0;
                int n = 0;

                for (int xi = Mathf.Max(x - lengthKernel, 0); xi < Mathf.Min(x + lengthKernel, width); xi++)
                {
                    for (int yi = Mathf.Max(y - lengthKernel, 0); yi < Mathf.Min(y + lengthKernel, height); yi++)
                    {
                        combinedValue += noiseMap[xi, yi];
                        n++;
                    }
                }

                return combinedValue / n;
            }




            return result;
        }
        #endregion

        #region Components
        public static T GetComponentInChildrenOnly<T>(this GameObject obj) where T : Component
        {
            T[] foundComponents = obj.gameObject.GetComponentsInChildren<T>();


            for (int i = 0; i < foundComponents.Length; i++)
            {
                if (foundComponents[i].gameObject != obj)
                {
                    return foundComponents[i];
                }
            }

            return null;


        }
        #endregion


    }
}


