using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace FillefranzTools
{
    /// <summary>
    /// A parabolic path between two points.
    /// </summary>
    /// 

    //Math inspired by: https://www.youtube.com/watch?v=Qxs3GrhcZI8&ab_channel=ABitOfGameDev
    [System.Serializable]
    public struct Parabola
    {
        

        public Vector3 startPoint, endPoint;
        public bool fixHeight;
        public float fixedHeight;
        public bool curveDown;
        public float flatness;

        public float a { get; private set; }
        public float b { get; private set; }
        public float c { get; private set; }

        public string equation => $"{a}x^2 + {b}x +{c}";


        float angle, v0, time, height, gndDst;
        Vector3 direction, groundDirection, targetPos;

        public float Height => height;
        public float Angle => angle;
        public float GroundDistance => gndDst;

        //Accesors
        public Vector3 Direction => direction.normalized;
        public Vector3 GroundDirection => groundDirection.normalized;
        public Vector3 Center => Evaluate(0.5f);


        /// <summary>
        /// Constructor for a parabola without a fixed height;
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        public Parabola(Vector3 startPoint, Vector3 endPoint)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
            curveDown = false;
            flatness = 2;
            fixHeight = false;
            fixedHeight = 3;
            angle = v0 = time = height = 0;
            direction = groundDirection = targetPos = Vector3.zero;
            a = 1;
            b = c = 0;
            gndDst = Vector3.Distance(endPoint.OverrideY(0), startPoint.OverrideY(0));
            Recalculate();
        }

        /// <summary>
        /// Constructor for a parabola with a fixed height;
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="fixedHeight"></param>
        public Parabola(Vector3 startPoint, Vector3 endPoint, float fixedHeight)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
            this.fixedHeight = fixedHeight;
            fixHeight = true;
            curveDown = false;
            flatness = 2;
            angle = v0 = time = height = 0;
            direction = groundDirection = targetPos = Vector3.zero;
            a = 1;
            b = c = 0;
            gndDst = Vector3.Distance(endPoint.OverrideY(0), startPoint.OverrideY(0));
            Recalculate();



        }
        /// <summary>
        /// Gets a point along the parabola based on the given t - value (0 - 1)
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Vector3 Evaluate(float t)
        {
            if (t == 0) return startPoint;
            if (t == 1) return endPoint;

            float scaledTime = t * time;

            float x = v0 * scaledTime * Mathf.Cos(angle);
            float y = v0 * scaledTime * Mathf.Sin(angle) - 0.5f * -Physics.gravity.y * Mathf.Pow(scaledTime, 2);

            if (curveDown && startPoint.y - endPoint.y < height) y = -y;

            return startPoint + groundDirection.normalized * x + Vector3.up * y;

        }
        
        public float GetTFromY(float y, float sign)
        {
            //t = (sin(a) ± ?(sin(a)^2 + 2 * g * y)) / g
            float time = Mathf.Abs((Mathf.Sign(angle) + sign * Mathf.Sqrt(Mathf.Sin(angle).Square() + 2 * -Physics.gravity.y * y)) / -Physics.gravity.y);
            return  time / this.time -1;
        }

        public float GetTFromX(float x)
        {
            return x / (v0 * Mathf.Cos(angle));
        }


        /// <summary>
        /// Returns the outcome of ax^2 + bx +c
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public float GetY(float x) =>  a* x.Square() + b* x + c;
        public float GetX(float y, float sign) => (-b + sign * Mathf.Sqrt(b - 4 * a * (c - y))) / 2 * a;

        /// <summary>
        /// Does all the necessary calculations for a parabolic path and returns false if any NaN values are the result. Call this after changing values. 
        /// </summary>
        public bool Recalculate()
        {
            direction = endPoint - startPoint;
            groundDirection = direction;
            groundDirection.y = 0;
            targetPos = new Vector3(groundDirection.magnitude, direction.y, 0);

            //Determines the height of the parabola. 
            if (fixHeight)
                height = fixedHeight;
            else
                height = targetPos.y + targetPos.magnitude / flatness;



            CalculatePathWithHeight(targetPos, height, out v0, out angle, out time);

            //Returns false if any NaN values are detected.
            return v0 != float.NaN && angle != float.NaN && time != float.NaN;
        }

        private void CalculatePathWithHeight(Vector3 targetPos, float h, out float v0, out float angle, out float time)
        {
            if (h == 0)
            {
                h = 0.00001f;
                height = h;
            }
            
            if(h < 0)
            {
                curveDown = !curveDown;
                h = Mathf.Abs(h);
                height = h;
            }

            float xt = targetPos.x;
            float yt = !curveDown ? targetPos.y : -targetPos.y;
            float g = -Physics.gravity.y;

            //ax^2 + bx + c = 0
            a = -0.5f * g;
            b = Mathf.Sqrt(2 * g * h);
            c = -yt;



            float tPlus = Helper.QuadraticFormula(a, b, c, 1);
            float tMinus = Helper.QuadraticFormula(a, b, c, -1);

            //Tries to avoid a NaN value that can occur from a the square root calculation in the quadratic formula.
            if (tPlus == float.NaN) time = tMinus;
            else if (tMinus == float.NaN) time = tPlus;
            else time = Mathf.Max(tPlus, tMinus);

            angle = Mathf.Atan(b * time / xt);

            v0 = b / Mathf.Sin(angle);

        }

        /// <summary>
        /// Estimated length of full parabola
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public float Length(float step)
        {
            float length = 0;

            for (float t = 0; t <= 1; t += step)
            {
                float dst = Vector3.Distance(Evaluate(t), Evaluate(t + step));
                length += dst;
            }

            return length;
        }

        /// <summary>
        /// Estimated length from t = 0 to t = maxT.
        /// </summary>
        /// <param name="step"></param>
        /// <param name="maxT"></param>
        /// <returns></returns>
        public float Length(float step, float maxT)
        {
            float length = 0;

            for (float t = 0; t <= maxT; t += step)
            {
                float dst = Vector3.Distance(Evaluate(t), Evaluate(t + step));
                length += dst;
            }

            return length;
        }

        public int Sections(float step)
        {

            int i = 0;
            for (float t = 0; t <= 1; t += step)
            {
                i++;
            }

            return i;
        }


        /// <summary>
        /// Converts a distance to a t-value on the parabola.
        /// </summary>
        /// <param name="step"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public float DstToTime(float step, float distance)
        {
            float length = Length(step);

            return distance / length;
        }

        /// <summary>
        /// Returns the Derivative (Steepness) at a given point in time.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public float Derivation(float t, float dX = 0.0001f)
        {
            Vector3 pointA = Evaluate(t);
            Vector3 pointB = Evaluate(t + dX);

            float dY = pointA.y - pointB.y;


            return dY / dX;
        }

        /// <summary>
        /// Returns the local forward direction of the parabola at a given t-value.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public Vector3 DirectionAtPoint(float t, float step, int sign = 1)
        {
            if (sign > 0)
            {
                Vector3 pointA = Evaluate(t);
                Vector3 pointB = Evaluate(t + step);

                return (pointB - pointA).normalized;
            }

            else
            {
                Vector3 pointA = Evaluate(t);
                Vector3 pointB = Evaluate(t - step);

                return (pointB - pointA).normalized;
            }
        }

        /// Returns the local up direction of the parabola at a given t-value.
        public Vector3 NormalAtPoint(float t, float step)
        {
            Vector3 pointA = Evaluate(t);
            Vector3 pointB = Evaluate(t + step);

            return Quaternion.LookRotation((pointB - pointA).normalized) * Vector3.up;
        }

        public float GetClosestTFromPos(Vector3 pos, float step)
        {
            float finalT = 0;
            float minDst = float.MaxValue;

            for (float t = 0; t <= 1; t += step)
            {
                float dst = Vector3.Distance(pos, Evaluate(t));

                if (dst < minDst)
                {
                    minDst = dst;
                    finalT = t;
                }
            }

            return finalT;
        }

        public float GetHeightFromXZ(Vector3 position, float step = 0.01f)
        {
            float height = 0;

            float minDst = float.MaxValue;
            for (float t = 0; t <= 1; t+= step)
            {
                Vector3 checkPoint = Evaluate(t);
                float dst = Vector2.Distance(position.FromXZ(), checkPoint.FromXZ());

                if (dst < minDst)
                {
                    minDst = dst;
                    height = checkPoint.y;
                }
            }


            return Mathf.Abs(height /Evaluate(0.5f).y * Height );
        }

        public Vector3 MaxPoint(float step = 0.01f)
        {
            Vector3 maxPoint = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            for (float t = 0; t <= 1; t += step)
            {
                Vector3 checkedPoint = Evaluate(t);

                if (checkedPoint.y > maxPoint.y)
                {
                    maxPoint = checkedPoint;
                }
            }

            return maxPoint;
        }

        public float MaxPointT(float step = 0.01f)
        {
            Vector3 maxPoint = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            float maxT = -1;

            for (float t = 0; t <= 1; t += step)
            {
                Vector3 checkedPoint = Evaluate(t);

                if (checkedPoint.y > maxPoint.y)
                {
                    maxPoint = checkedPoint;
                    maxT = t;
                }
            }

            return maxT;
        }

        public float StartToMaxDst(float step = 0.01f)
        {
            Vector3 maxPoint = MaxPoint(step);
            return Vector2.Distance(maxPoint.FromXZ(), startPoint.FromXZ());
        }

        public float DstToMax(Vector3 point, float step = 0.01f)
        {
            Vector3 maxPoint = MaxPoint(step);
            return Vector2.Distance(maxPoint.FromXZ(), point.FromXZ());
        }

        public float GetAngleAtPoint(float t, float step = 0.01f)
        {
            Vector3 fwd= DirectionAtPoint(t, step);
            return Vector3.Angle( fwd, direction);
        }


    }

}
