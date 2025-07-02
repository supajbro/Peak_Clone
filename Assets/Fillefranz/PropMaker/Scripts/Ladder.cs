using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FillefranzTools;

namespace PropMaker
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter), typeof(MeshCollider))]
    public class Ladder : Prop
    {
        //Shape
        public Vector3 start;
        public Vector3 end;
        public float rotation = 0;
        public float width;

        //Rail Settings
        public float railWidth;
        public float railThickness;
        public enum EndMode { Flat, Rounded}
        public EndMode endMode = EndMode.Flat;

        //Steps
        public bool fixedStepAmount = false;
        public float stepSpacing = 0.25f;
        public float stepRadius = 0.1f;
        public float stepPadding = 0f;
        public int numberOfSteps = 10;
        public int stepResolution = 12;


        //Editor Variables
        public bool autoCenter = true; //Call Recenter() when a change is made?
        public PropEditMode editMode = PropEditMode.GameObject;
        public EditOrientation editOrientation = EditOrientation.World;


        List<int> tris = new List< int>();

        public Vector3 forward => (end - start).normalized;
        public Vector3 up
        {
            get
            {
                if (start.OverrideY(0) == end.OverrideY(0)) 
                    return Quaternion.AngleAxis(rotation, forward) * Vector3.forward;
                else
                    return Vector3.Cross(forward, Quaternion.LookRotation(Vector3.right) * (end - start).OverrideY(0).normalized).normalized;
            }
        }
            
            
        Vector3 right => Vector3.Cross(forward, up).normalized;
        float length = 0;


        public override void UpdateProp()
        {
            length = Vector3.Distance(start, end);
            Clear();
            Recenter();
            CreateRail(1);
            CreateRail(-1);
            CalculateStepValues();
            PlaceSteps();
            SetMesh();
        }



        void CreateRail(int sign)
        {
            Vector3 start = this.start + right * sign * (width * 0.5f);
            

            CreatePlane(start, new Vector2(railWidth, length), right * sign, forward, Vector2.one, tris, sign < 0);
            CreatePlane(start, new Vector2(railThickness, length), -up, forward, Vector2.one, tris, sign > 0, true);


            start = this.start + right * sign * (width * 0.5f) - up * railThickness;
            CreatePlane(start, new Vector2(railWidth, length), right * sign, forward, Vector2.one, tris, sign > 0, true);

            start = this.start + right * sign * (width * 0.5f + railWidth) - up * railThickness;
            CreatePlane(start, new Vector2(railThickness, length), up, forward, Vector2.one, tris, sign > 0, true);

            if (endMode == EndMode.Flat)
            {
                start = this.start + right * sign * width * 0.5f;
                CreatePlane(start, new Vector2(railWidth, railThickness), right * sign, -up, Vector2.one, tris, sign > 0);
                start = end + right * sign * width * 0.5f;
                CreatePlane(start, new Vector2(railWidth, railThickness), right * sign, -up, Vector2.one, tris, sign < 0);
            }
        }

        void CalculateStepValues()
        {
            if (fixedStepAmount)
            {
                float remainingLength = length;
                int numberOfSpaces = numberOfSteps + 1;
                remainingLength -= numberOfSteps * stepRadius * 2;
                remainingLength -= stepPadding * 2;
                stepSpacing = remainingLength / numberOfSpaces;
            }

            else
            {
                float totalSize = stepSpacing;

                int amount = 0;

                while (totalSize + stepRadius * 2 + stepSpacing< length)
                {
                    totalSize += stepRadius * 2+ stepSpacing;
                    amount++;
                }

                stepPadding = (length - totalSize) * 0.5f;

                numberOfSteps = amount;
            }
        }

        void PlaceSteps()
        {
            for (int i = 0; i < numberOfSteps; i++)
            {
                float dst = stepPadding + i * stepRadius * 2  + stepRadius + stepSpacing * (i +1);
                Vector3 centerA = start + forward * dst + right * width * 0.5f - up * railThickness * 0.5f;
                Vector3 centerB = centerA - right * width;
                Vector3[] circleA = Helper.PointsOnCircle(stepRadius, stepResolution, centerA, up, right);
                Vector3[] circleB = Helper.PointsOnCircle(stepRadius, stepResolution, centerB, up, right);

                ConnectCircles(circleB, circleA, tris, 0, 1, false);
            }
        }

        protected override void SetMesh()
        {
            Quaternion orientation = Quaternion.AngleAxis(rotation, forward);
            for (int i = 0; i < vertices.Count; i++)
                vertices[i] = orientation * vertices[i];

            for (int i = 0; i < normals.Count; i++)
                normals[i] = orientation * normals[i];

            if (vertices.Count >= 65534)
                mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            else
                mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt16;

            mesh.SetVertices(vertices);
            mesh.SetTriangles(tris, 0);
            mesh.SetUVs(0, uvs);
            mesh.SetNormals(normals);
            mesh.name = meshName;
            RefreshReferences();

            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;

            vertexCount = vertices.Count;
            triangleCount = tris.Count / 3;

            normals.Clear();
            tris.Clear();
            vertices.Clear();
            uvs.Clear();
        }


        public override void Clear()
        {
            if (mesh == null) mesh = new Mesh();
            normals.Clear();
            mesh.Clear();
            tris.Clear();
            vertices.Clear();
            uvs.Clear();
        }

        /// <summary>
        /// Shifts the objects global position so it is in the center of the ladder.
        /// </summary>
        public void Recenter()
        {
            Vector3 center = (start + end) / 2;
            start -= center;
            end -= center;
            transform.position += center;
        }
    }
}


