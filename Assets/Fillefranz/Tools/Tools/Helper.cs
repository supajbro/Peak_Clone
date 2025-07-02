using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FillefranzTools;


namespace FillefranzTools
{
    //This Script provides a variety of functions and values to simplify writing code.
    public static class Helper
    {

        #region Vectors
        #region Vector2
        /// <summary>
        /// A quick way to round a Vector2 to a Vector2Int
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector2Int RoundedVector2(Vector2 vector)
        {
            return vector.Round();
        }

        /// <summary>
        /// A quick way to floor a Vector2 to a Vector2Int
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector2Int FloorVector2(Vector2 vector)
        {
            return vector.Floor();
        }

        /// <summary>
        /// A quick way to ceil a Vector2 to a Vector2Int
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector2Int CeilVector2(Vector2 vector)
        {
            return vector.Ceil();
        }

        /// <summary>
        /// Returns a Vector2 where both values are equal to the passed in value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector2 SimpleVector2(float value)
        {
            return new Vector2(value, value);
        }

        /// <summary>
        /// Returns a Vector2Int where both values are equal to the passed in value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector2Int SimpleVector2(int value)
        {
            return new Vector2Int(value, value);
        }
        /// <summary>
        /// Returns a normalized Vector2 pointing from currentPoint to targetPoint
        /// </summary>
        /// <param name="currentPoint"></param>
        /// <param name="targetPoint"></param>
        /// <returns></returns>
        public static Vector2 Vector2To(Vector2 currentPoint, Vector2 targetPoint)
        {
            return (targetPoint - currentPoint).normalized;
        }

        public static bool IsInRectangle(Vector2 bound1, Vector2 bound2, Vector2 point)
        {
            float leftBound = Mathf.Min(bound1.x, bound2.x);
            float rightBound = Mathf.Max(bound1.x, bound2.x);
            float bottomBound = Mathf.Min(bound1.y, bound2.y);
            float topBound = Mathf.Max(bound1.y, bound2.y);

            return point.x >= leftBound && point.x <= rightBound && point.y >= bottomBound && point.y <= topBound;
        }

        public static Vector2 DivideVector(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x / b.x, a.y / b.y);
        }

        #endregion Vector2

        #region Vector3
        /// <summary>
        /// A quick way to round a Vector3 to an Vector3Int
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3Int RoundedVector3(Vector3 vector)
        {
            return vector.Round();
        }

        /// <summary>
        /// A quick way to floor a Vector3 to an Vector3Int
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3Int FloorVector3(Vector3 vector)
        {
            return vector.Floor();
        }

        /// <summary>
        /// A quick way to ceil a Vector3 to an Vector3Int
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3Int CeilVector3(Vector3 vector)
        {
            return vector.Ceil();
        }

        /// <summary>
        /// Returns a Vector3 where all values are equal to the passed in value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector3 SimpleVector3(float value)
        {
            return new Vector3(value, value, value);
        }

        /// <summary>
        /// Returns a Vector3Int where all values are equal to the passed in value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector3Int SimpleVector3(int value)
        {
            return new Vector3Int(value, value, value);
        }

        /// <summary>
        /// Returns a normalized Vector3 pointing from currentPoint to targetPoint
        /// </summary>
        /// <param name="currentPoint"></param>
        /// <param name="targetPoint"></param>
        /// <returns></returns>
        public static Vector3 Vector3To(Vector3 currentPoint, Vector3 targetPoint)
        {
            return (targetPoint - currentPoint).normalized;
        }


        public static Vector3 DivideVector(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
        }


        #endregion Vector3
        #endregion Vectors

        #region Math
        /// <summary>
        /// Returns the square of the given float value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float Square(float value)
        {
            return value.Square();
        }

        /// <summary>
        /// Returns the square of the given int value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Square(int value)
        {
            return value.Square();
        }

        /// <summary>
        /// Returns true if the given int is even
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool IsEven(int number)
        {
            return number.IsEven();
        }

        /// <summary>
        /// Returns true if the given int is odd
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>

        public static bool IsOdd(int number)
        {
            return number.IsOdd();
        }

        /// <summary>
        /// Returns the nth root of the given value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="root"></param>
        /// <returns></returns>
        public static float Root(float value, float root = 2)
        {
            return value.Root(root);
        }

        /// <summary>
        /// Calculates the average value between a and b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float Average(float a, float b)
        {
            return (a + b) / 2;
        }

        /// <summary>
        /// Calculates the average value between a and b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int Average(int a, int b)
        {
            return (a + b) / 2;
        }

        /// <summary>
        /// Calculates the average value.
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public static float Average(float[] numbers)
        {
            return numbers.Average();


        }

        /// <summary>
        /// Calculates the average value.
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public static int Average(int[] numbers)
        {
            return numbers.Average();
        }

        /// <summary>
        /// Returns the overlap between the two.
        /// </summary>
        /// <param name="interval1"></param>
        /// <param name="interval2"></param>
        /// <returns></returns>
        public static MinMax IntervallBetween(MinMax interval1, MinMax interval2)
        {
            float min = Mathf.Max(interval1.Min, interval2.Min);
            float max = Mathf.Min(interval1.Max, interval2.Max);

            return new MinMax(min, max);
        }

        /// <summary>
        /// Returns the overlap between the two.
        /// </summary>
        /// <param name="interval1"></param>
        /// <param name="interval2"></param>
        /// <returns></returns>
        public static MinMaxInt IntervallBetween(MinMaxInt interval1, MinMaxInt interval2)
        {
            int min = Mathf.Max(interval1.Min, interval2.Min);
            int max = Mathf.Min(interval1.Max, interval2.Max);

            return new MinMaxInt(min, max);
        }

        /// <summary>
        /// Returns -1 for odd numbers, 1 for even and 0 for 0.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int SignedEvenOrOdd(int value)
        {
            if (value == 0)
            {
                return 0;
            }

            else if (value.IsOdd())
            {
                return -1;
            }

            else
            {
                return 1;
            }
        }

        /// <summary>
        /// Rounds to the closest multiple of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="multipleOf"></param>
        /// <returns></returns>
        public static float RoundTo(float value, float multipleOf)
        {
            return Mathf.Round(value / multipleOf) * multipleOf;
        }


        #endregion Math

        #region Constants
        public static readonly float gravitationalConstant = 6.6743f * Mathf.Pow(10, -11);
        public static readonly float gravitationalAcceleration = 9.80665f;

        /// <summary>
        /// Shorthand for a Vector2 pointing in four different directions
        /// </summary>
        public static readonly Vector2Int[] fourDirections =
        {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };

        /// <summary>
        /// Shorthand for a Vector2 pointing in eight different directions
        /// </summary>
        public static readonly Vector2Int[] eightDirections =
    {
        Vector2Int.up,
        Vector2Int.up +Vector2Int.left,
        Vector2Int.left,
        Vector2Int.left + Vector2Int.down,
        Vector2Int.down,
        Vector2Int.down + Vector2Int.right,
        Vector2Int.right,
        Vector2Int.right + Vector2Int.up
    };
        #endregion Constants

        #region Noise
        public static float[,] GeneratePerlinNoiseMap(int width, int height, float scale, Vector2 offset)
        {
            float[,] noiseMap = new float[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    noiseMap[x, y] = Mathf.Clamp01(Mathf.PerlinNoise(x / scale + offset.x, y / scale + offset.y));
                }
            }


            return noiseMap;
        }

        public static float[,] GeneratePerlinNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
        {
            float[,] noiseMap = new float[mapWidth, mapHeight];

            System.Random prng = new System.Random(seed);
            Vector2[] octaveOffsets = new Vector2[octaves];
            for (int i = 0; i < octaves; i++)
            {
                float offsetX = prng.Next(-100000, 100000) + offset.x;
                float offsetY = prng.Next(-100000, 100000) + offset.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }

            if (scale <= 0)
            {
                scale = 0.0001f;
            }

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            float halfWidth = mapWidth / 2f;
            float halfHeight = mapHeight / 2f;


            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {

                    float amplitude = 1;
                    float frequency = 1;
                    float noiseHeight = 0;

                    for (int i = 0; i < octaves; i++)
                    {
                        float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                        float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= persistance;
                        frequency *= lacunarity;
                    }

                    if (noiseHeight > maxNoiseHeight)
                    {
                        maxNoiseHeight = noiseHeight;
                    }
                    else if (noiseHeight < minNoiseHeight)
                    {
                        minNoiseHeight = noiseHeight;
                    }
                    noiseMap[x, y] = noiseHeight;
                }
            }

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                }
            }

            return noiseMap;

        }


        //Also known as cellular noise
        public static float[,] GenerateWorleyNoiseMap(int width, int height, int nValue, Vector2[] points)
        {
            float[,] noiseMap = new float[width, height];



            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float[] distances = new float[points.Length];
                    Vector2Int lookedAtPoint = new Vector2Int(x, y);

                    for (int i = 0; i < points.Length; i++)
                    {
                        distances[i] = Vector2.Distance(lookedAtPoint, points[i]);
                    }

                    Array.Sort(distances);
                    noiseMap[x, y] = distances[nValue];
                }
            }



            return noiseMap;
        }

        public static float[,] AlignNoiseToCurve(float[,] map, AnimationCurve curve)
        {
            if (curve == null) return map;
            if (map == null) return null;


            float[,] noisemap = map;

            for (int x = 0; x < noisemap.GetLength(0); x++)
            {
                for (int y = 0; y < noisemap.GetLength(1); y++)
                {
                    noisemap[x, y] = curve.Evaluate(noisemap[x, y]);
                }
            }

            return noisemap;
        }



        public static float[,] InverseNoise(float[,] map)
        {
            if (map == null) return null;

            float[,] noiseMap = new float[map.GetLength(0), map.GetLength(1)];

            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    noiseMap[x, y] = map[y, x];
                }
            }

            return noiseMap;

        }

        #endregion Noise

        #region Texture And Color

        public static Texture2D MonocromeTexture(Color color, int size = 1)
        {
            size = Mathf.Max(size, 1);

            Texture2D tex = new Texture2D(size, size);
            Color[] colors = new Color[size.Square()];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = color;
            }
            tex.SetPixels(colors);
            tex.Apply();
            return tex;
        }

        public static Texture2D DrawNoise(float[,] noiseMap)
        {
            if (noiseMap == null)
            {
                Debug.Log("Noise Map is null returned black image");
                return MonocromeTexture(Color.black);
            }

            Color[] pixels = new Color[noiseMap.Length];

            int index = 0;
            for (int y = 0; y < noiseMap.GetLength(0); y++)
            {
                for (int x = 0; x < noiseMap.GetLength(1); x++)
                {
                    float noiseValue = noiseMap[x, y];
                    pixels[index] = new Color(noiseValue, noiseValue, noiseValue, 1);
                    index++;

                }
            }

            Texture2D texture = new Texture2D(noiseMap.GetLength(0), noiseMap.GetLength(1));
            texture.SetPixels(pixels);
            texture.Apply();

            return texture;


        }

        public static Texture2D DrawNoise(float[,] noiseMap, float cutOff)
        {
            Color[] pixels = new Color[noiseMap.Length];

            int index = 0;
            for (int y = 0; y < noiseMap.GetLength(0); y++)
            {
                for (int x = 0; x < noiseMap.GetLength(1); x++)
                {
                    float noiseValue = noiseMap[x, y];
                    if (noiseValue > cutOff)
                    {
                        pixels[index] = new Color(1, 1, 1, 1);
                    }

                    else
                    {
                        pixels[index] = new Color(0, 0, 0, 1);
                    }

                    index++;

                }
            }

            Texture2D texture = new Texture2D(noiseMap.GetLength(0), noiseMap.GetLength(1));
            texture.SetPixels(pixels);
            texture.Apply();

            return texture;


        }

        public static Texture2D DrawNormalmap(Vector3[,] normals)
        {
            Texture2D normalMap = new Texture2D(normals.GetLength(0), normals.GetLength(1));
            Color[] pixels = new Color[normals.Length];


            int i = 0;
            for (int y = 0; y < normalMap.height; y++)
            {
                for (int x = 0; x < normalMap.width; x++)
                {

                    Vector3 normal = normals[x, y];
                    pixels[i] = new Color(normal.x, normal.y, normal.z);

                    i++;
                }
            }

            normalMap.SetPixels(pixels);

            normalMap.Apply();

            return normalMap;
        }

        public static Color RandomColor()
        {
            return new Color(Random01(), Random01(), Random01());
        }

        /// <summary>
        /// Returns a color with a random hue (0 - 1) and a saturaion anmd value of 1.
        /// </summary>
        /// <returns></returns>
        public static Color RandomHue()
        {
            return Color.HSVToRGB(Random01(), 1, 1);
        }

        public static Color[] colors = new Color[]
        {
            new Color(0, 1, 0.87f), //cyan
            new Color(1, 0, 0), // red
            new Color(0, 1, 0), //green
            new Color(0, 0, 1), //blue
            new Color(0.92f, 1, 0), //yellow
            new Color(0.75f, 1, 0), //lime
            new Color(0.6f, 0, 1), //Purple
            new Color(1, 0, 1), //magenta
            new Color(1, 0.62f, 0.83f) //Pink
        };


        #endregion Texture And Color

        #region Remapping
        /// <summary>
        /// If the given value is outside the specifeied interval it comes back on the other end of the interval
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Loop(int value, int min = 0, int max = 360)
        {
            int _value = value;

            if (value <= max && value >= min)
            {
                return _value;
            }

            else
            {
                if (value > max)
                {
                    _value = min;
                    return _value;

                }

                else
                {
                    _value = max;
                    return _value;
                }
            }

        }


        /// <summary>
        /// If the given value is outside the specifeied interval it comes back on the other end of the interval
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Loop(float value, float min = 0, float max = 360)
        {
            float _value = value;

            if (value <= max && value >= min)
            {
                return _value;
            }

            else
            {
                if (value > max)
                {
                    _value = min + _value - max;
                    return _value;
                }

                else
                {
                    _value = max - _value - min;
                    return _value;
                }
            }
        }

        public static float DecimalOnly(float value)
        {
            return value - value.Floor();
        }

        /// <summary>
        /// Rescales the given value to be proportional to the new max value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="currentMax"></param>
        /// <param name="newMax"></param>
        /// <returns></returns>
        public static float Rescale(float value, float currentMax, float newMax)
        {
            return Mathf.Clamp01(value / currentMax) * newMax;
        }

        /// <summary>
        /// Rescales the given value to be proportional to the new max value 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="currentMax"></param>
        /// <param name="newMax"></param>
        /// <returns></returns>
        public static int Rescale(int value, int currentMax, int newMax)
        {
            return Mathf.RoundToInt(Mathf.Clamp01((float)value / currentMax) * newMax);
        }


        /// <summary>
        /// Rescales the given vector to be proportional to the new max value 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="currentMax"></param>
        /// <param name="newMax"></param>
        /// <returns></returns>
        public static Vector2 Rescale(Vector2 values, float currentMax, float newMax)
        {
            return new Vector2(Rescale(values.x, currentMax, newMax), Rescale(values.y, currentMax, newMax));
        }

        /// <summary>
        /// Rescales the given vector to be proportional to the new max value 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="currentMax"></param>
        /// <param name="newMax"></param>
        /// <returns></returns>
        public static Vector2Int Rescale(Vector2Int values, int currentMax, int newMax)
        {
            return new Vector2Int(Rescale(values.x, currentMax, newMax), Rescale(values.y, currentMax, newMax));
        }

        /// <summary>
        /// Rescales the given vector to be proportional to the new max value
        /// </summary>
        /// <param name="values"></param>
        /// <param name="currentMax"></param>
        /// <param name="newMax"></param>
        /// <returns></returns>
        public static Vector3 Rescale(Vector3 values, float currentMax, float newMax)
        {
            return new Vector3(Rescale(values.x, currentMax, newMax), Rescale(values.y, currentMax, newMax), Rescale(values.z, currentMax, newMax));
        }

        /// <summary>
        /// Rescales the given vector to be proportional to the new max value (untested)
        /// </summary>
        /// <param name="values"></param>
        /// <param name="currentMax"></param>
        /// <param name="newMax"></param>
        /// <returns></returns>
        public static Vector3Int Rescale(Vector3Int values, int currentMax, int newMax)
        {
            return new Vector3Int(Rescale(values.x, currentMax, newMax), Rescale(values.y, currentMax, newMax), Rescale(values.z, currentMax, newMax));
        }

        /// <summary>
        /// Remaps the given value to be proportional to the new interval (untested)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="currentMin"></param>
        /// <param name="currentMax"></param>
        /// <param name="newMin"></param>
        /// <param name="newMax"></param>
        /// <returns></returns>
        public static float Remap(float value, float currentMin, float currentMax, float newMin, float newMax)
        {
            return newMin + (value - currentMin) * (newMax - newMin) / (currentMax - currentMin);

        }


        /// <summary>
        /// Remaps the given value to be proportional to the new interval (untested)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="currentMin"></param>
        /// <param name="currentMax"></param>
        /// <param name="newMin"></param>
        /// <param name="newMax"></param>
        /// <returns></returns>
        public static int Remap(int value, int currentMin, int currentMax, int newMin, int newMax)
        {
            float percent = Mathf.Clamp01((float)(value - currentMin) / (value - currentMax));

            return Mathf.RoundToInt(percent * (newMax - newMin) + newMin);

        }

        public static bool Int2Bool(int value)
        {
            return value > 0;
        }

        #endregion

        #region Random
        /// <summary>
        /// Returns a ranmdom float value between 0 and 1
        /// </summary>
        /// <returns></returns>
        public static float Random01()
        {
            float number = UnityEngine.Random.Range(0f, 1f);
            number = Mathf.Clamp01(number);
            return number;
        }

        /// <summary>
        /// Returns a ranmdom float value between 0 and 1, according to the specified seed
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static float Random01(int seed)
        {
            UnityEngine.Random.InitState(seed);
            float number = UnityEngine.Random.Range(0f, 1f);
            number = Mathf.Clamp01(number);
            UnityEngine.Random.InitState(Time.time.GetHashCode());
            return number;
        }

        /// <summary>
        /// Returns a random Vector3 on a unit circle (format (x, 0, z))
        /// </summary>
        /// <param name="maxAngle"></param>
        /// <returns></returns>
        public static Vector3 RandomInUnitCircle(float maxAngle = 360)
        {
            Vector3 randomVector;

            maxAngle = Mathf.Min(maxAngle, 360);

            float angle = UnityEngine.Random.Range(0f, maxAngle);
            randomVector.x = Mathf.Cos(Mathf.Deg2Rad * angle);
            randomVector.y = 0;
            randomVector.z = Mathf.Sin(Mathf.Deg2Rad * angle);

            return randomVector;
        }

        /// <summary>
        /// Returns a differennt intager every time the function is called.
        /// </summary>
        /// <returns></returns>
        public static int RandomSeed()
        {
            return DateTime.Now.GetHashCode();
        }

        /// <summary>
        /// Returns a random float in the given interval.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        public static float NextFloat(float min, float max, int seed)
        {
            System.Random random = new System.Random(seed);
            double val = (random.NextDouble() * (max - min) + min);
            return (float)val;
        }

        /// <summary>
        /// Returns a random float in the given interval.
        /// </summary>
        /// <param name="minMax"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        public static float NextFloat(MinMax minMax, System.Random random)
        {
            double val = (random.NextDouble() * (minMax.Max - minMax.Min) + minMax.Min);
            return (float)val;
        }

        /// <summary>
        /// Returns a random float in the given interval.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        public static float NextFloat(float min, float max, System.Random random)
        {
            double val = (random.NextDouble() * (max - min) + min);
            return (float)val;
        }

        #endregion

        #region Geometry
        /// <summary>
        /// Performs the quadratic formula on the given values with a specified sign
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        public static float QuadraticFormula(float a, float b, float c, float sign)
        {
            return (-b + sign * (float)System.Numerics.Complex.Abs(ComplexSqrt(b * b - 4 * a * c))) / (2 * a);
        }

        public static System.Numerics.Complex ComplexSqrt(float value)
        {
            return System.Numerics.Complex.Sqrt(value);
        }

        /// <summary>
        /// Returns the diagonal of a rectangle with the given side lengths.
        /// </summary>
        /// <param name="side1"></param>
        /// <param name="side2"></param>
        /// <returns></returns>
        public static float Diagonal(float side1, float side2)
        {
            return Mathf.Sqrt(side1.Square() + side2.Square());
        }

        /// <summary>
        /// Returns an array of evenly spaced Vector3 points along an arc, with support for rotaion
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="pointNum"></param>
        /// <param name="angle"></param>
        /// <param name="centerPoint"></param>
        /// <param name="forward"></param>
        /// <param name="forwardAngle"></param>
        /// <param name="orientation"></param>
        /// <returns></returns>
        public static Vector3[] PointsOnArc(float radius, int pointNum, float angle, Vector3 centerPoint, Vector3 forward, float forwardAngle, Quaternion orientation)
        {
            Vector3[] points = new Vector3[pointNum];
            float deg;

            for (int i = 0; i < points.Length; i++)
            {
                deg = Mathf.Lerp(0, angle, (float)i / points.Length);
                Vector3 direction = orientation * Quaternion.Euler(0, deg, 0) * Quaternion.Euler(0, forwardAngle, 0) * forward * radius;

                points[i] = centerPoint + direction;


            }

            return points;
        }

        /// <summary>
        /// Returns an array of evenly spaced Vector3 points along an arc
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="pointNum"></param>
        /// <param name="angle"></param>
        /// <param name="centerPoint"></param>
        /// <param name="forward"></param>
        /// <param name="forwardAngle"></param>
        /// <returns></returns>
        public static Vector3[] PointsOnArc(float radius, int pointNum, float angle, Vector3 centerPoint, Vector3 forward, float forwardAngle)
        {
            Vector3[] points = new Vector3[pointNum];
            float deg;

            for (int i = 0; i < points.Length; i++)
            {
                deg = Mathf.Lerp(0, angle, (float)i / points.Length);
                Vector3 direction = Quaternion.Euler(0, deg, 0) * Quaternion.Euler(0, forwardAngle, 0) * forward * radius;

                points[i] = centerPoint + direction;


            }

            return points;
        }

        /// <summary>
        /// Returns an array of evenly spaced Vector3 points along an circle
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="pointNum"></param>
        /// <param name="centerPoint"></param>
        /// <returns></returns>
        public static Vector3[] PointsOnCircle(float radius, int pointNum, Vector3 centerPoint)
        {
            Vector3[] points = new Vector3[pointNum];
            float deg;

            for (int i = 0; i < points.Length; i++)
            {
                deg = Mathf.Lerp(0, 360, (float)i / points.Length);
                Vector3 direction = Quaternion.Euler(0, deg, 0) * Vector3.forward * radius;

                points[i] = centerPoint + direction;


            }

            return points;
        }


        /// <summary>
        /// Returns an array of evenly spaced Vector3 points along an circle
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="pointNum"></param>
        /// <param name="centerPoint"></param>
        /// <param name="edgeDir"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        public static Vector3[] PointsOnCircle(float radius, int pointNum, Vector3 centerPoint, Vector3 edgeDir, Vector3 normal)
        {
            Vector3[] points = new Vector3[pointNum];
            float deg;

            for (int i = 0; i < points.Length; i++)
            {
                deg = Mathf.Lerp(0, 360, (float)i / points.Length);
                Vector3 direction = Quaternion.AngleAxis(deg, normal) * edgeDir * radius;

                points[i] = centerPoint + direction;


            }

            return points;
        }



        /// <summary>
        /// Returns an array of evenly spaced Vector3s pointing away from the circles origin.
        /// </summary>
        /// <param name="pointNum"></param>
        /// <returns></returns>
        public static Vector3[] DirectionsInCircle(int pointNum)
        {
            Vector3[] directions = new Vector3[pointNum];
            float deg;

            for (int i = 0; i < directions.Length; i++)
            {
                deg = Mathf.Lerp(0, 360, (float)i / directions.Length);
                Vector3 direction = Quaternion.Euler(0, deg, 0) * Vector3.forward;

                directions[i] = direction;


            }

            return directions;
        }

        /// <summary>
        /// The shortest distance from a point to a line.
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static float DistanceToLine(Ray ray, Vector3 point)
        {
            return Vector3.Cross(ray.direction, point - ray.origin).magnitude;
        }

        public static float DistanceToLine(Ray ray1, Ray ray2)
        {
            Vector3 originDelta = ray2.origin - ray1.origin;
            Vector3 directionCross = Vector3.Cross(ray1.direction, ray2.direction);
            Vector3 perpOriginDelta = Vector3.Cross(originDelta, ray2.direction);

            float numerator = Vector3.Dot(originDelta, directionCross);
            float denominator = directionCross.sqrMagnitude;

            // If the rays are parallel, the shortest distance is the distance between their origins.
            if (Mathf.Abs(denominator) < Mathf.Epsilon)
            {
                return originDelta.magnitude;
            }

            float t1 = numerator / denominator;
            float t2 = Vector3.Dot(perpOriginDelta, directionCross) / denominator;

            // Calculate the closest points on each ray
            Vector3 closestPointRay1 = ray1.origin + ray1.direction * t1;
            Vector3 closestPointRay2 = ray2.origin + ray2.direction * t2;

            // Calculate the distance between the closest points
            float distance = Vector3.Distance(closestPointRay1, closestPointRay2);

            return distance;
        }

        /// <summary>
        /// The absolute difference between a and b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float Distance(float a, float b) => Mathf.Abs(a - b);

        /// <summary>
        /// Combines two meshes into a new mesh.
        /// </summary>
        /// <param name="mesh1"></param>
        /// <param name="mesh2"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Mesh CombineMeshes(Mesh mesh1, Mesh mesh2, Vector3 offset)
        {
            Vector3[] vertices = new Vector3[mesh1.vertices.Length + mesh2.vertices.Length];
            int[] triangles = new int[mesh1.triangles.Length + mesh2.triangles.Length];
            Vector3[] normals = new Vector3[mesh1.normals.Length + mesh2.normals.Length];


            //Vertex Data
            for (int i = 0; i < mesh1.vertices.Length; i++)
            {
                vertices[i] = mesh1.vertices[i];
                normals[i] = mesh1.normals[i];
            }

            for (int i = mesh1.vertices.Length; i < vertices.Length; i++)
            {
                vertices[i] = mesh2.vertices[i - mesh1.vertices.Length] + offset;
                normals[i] = mesh2.normals[i - mesh1.vertices.Length];
            }


            //Triangles
            for (int i = 0; i < mesh1.triangles.Length; i++)
            {
                triangles[i] = mesh1.triangles[i];
            }

            for (int i = 0; i < mesh2.triangles.Length; i++)
            {
                triangles[mesh1.triangles.Length + i] = mesh2.triangles[i] + mesh1.vertices.Length;
            }




            Mesh mesh = new Mesh();

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.normals = normals;


            return mesh;
        }

        /// <summary>
        /// Combines two meshes into a new mesh.
        /// </summary>
        /// <param name="mesh1"></param>
        /// <param name="mesh2"></param>
        /// <returns></returns>
        public static Mesh CombineMeshes(Mesh mesh1, Mesh mesh2)
        {
            Vector3[] vertices = new Vector3[mesh1.vertices.Length + mesh2.vertices.Length];
            int[] triangles = new int[mesh1.triangles.Length + mesh2.triangles.Length];
            Vector3[] normals = new Vector3[mesh1.normals.Length + mesh2.normals.Length];


            //Vertex Data
            for (int i = 0; i < mesh1.vertices.Length; i++)
            {
                vertices[i] = mesh1.vertices[i];
                normals[i] = mesh1.normals[i];
            }

            for (int i = mesh1.vertices.Length; i < vertices.Length; i++)
            {
                vertices[i] = mesh2.vertices[i - mesh1.vertices.Length];
                normals[i] = mesh2.normals[i - mesh1.vertices.Length];
            }


            //Triangles
            for (int i = 0; i < mesh1.triangles.Length; i++)
            {
                triangles[i] = mesh1.triangles[i];
            }

            for (int i = 0; i < mesh2.triangles.Length; i++)
            {
                triangles[mesh1.triangles.Length + i] = mesh2.triangles[i] + mesh1.vertices.Length;
            }




            Mesh mesh = new Mesh();

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.normals = normals;


            return mesh;
        }

        #endregion Geometry

        #region Delegates
        public delegate void GridFunction2D(int x, int y);
        public delegate void GridFunction3D(int x, int y, int z);
        public delegate void ForEach(int i);
        public delegate void Function();

        #endregion





    }
}

