using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FillefranzTools;

namespace PropMaker
{
    public abstract class Prop : MonoBehaviour
    {

        //Editor variables
        [HideInInspector] public int editorTab = 0; //Curretly selected tab.
        [HideInInspector] public bool autoUpdate = true; //Call UpdateBridge() when a change is made?
        [HideInInspector] public bool hasLoaded = false; //Used to update the bridge from the editor when first created. 
        [HideInInspector] public int seed = 0;
        [HideInInspector] public int vertexCount = 0;
        [HideInInspector] public int triangleCount = 0;


        //References
        public MeshFilter meshFilter { get; protected set; }
        public MeshCollider meshCollider { get; protected set; }
        public MeshRenderer meshRenderer { get; protected set; }

        //Mesh Data
        public Mesh mesh { get; protected set; }
        public List<Vector3> vertices { get; protected set; } = new List<Vector3>(); //All vertices in the mesh.
        protected List<Vector3> normals = new List<Vector3>(); //All normal vectors.
        protected List<Vector2> uvs = new List<Vector2>(); //UV0.
        public string meshName;
        public abstract void Clear();
        public abstract void UpdateProp();
        protected abstract void SetMesh();

        /// <summary>
        /// Makes sure none of the references is null.
        /// </summary>
        public virtual void RefreshReferences()
        {
            if (meshFilter == null)
                meshFilter = GetComponent<MeshFilter>();
            if (meshRenderer == null)
                meshRenderer = GetComponent<MeshRenderer>();
            if (meshCollider == null)
                meshCollider = GetComponent<MeshCollider>();

        }

        /// <summary>
        /// Connects two circles into a cylinder.
        /// </summary>
        /// <param name="circleA"></param>
        /// <param name="circleB"></param>
        /// <param name="tris"></param>
        /// <param name="uvStart"></param>
        /// <param name="uvEnd"></param>
        /// <param name="flipTris"></param>
        protected void ConnectCircles(Vector3[] circleA, Vector3[] circleB, List<int> tris, float uvStart = 0, float uvEnd = 1, bool flipTris = false)
        {

            int normalSign = flipTris ? -1 : 1;

            if (circleA.Length != circleB.Length) return;

            int vert = vertices.Count;

            vertices.AddRange(circleA);

            Vector3 centerA = Vector3.zero;

            for (int i = 0; i < circleA.Length; i++)
                centerA += circleA[i];

            centerA /= circleA.Length;




            vertices.AddRange(circleB);


            Vector3 centerB = Vector3.zero;

            for (int i = 0; i < circleB.Length; i++)
                centerB += circleB[i];

            centerB /= circleB.Length;


            Vector3 up = (centerB - centerA).normalized;

            if (centerA.y > centerB.y)
                up = -up;

            float radiusA = Vector3.Distance(circleA[0], centerA);
            float radiusB = Vector3.Distance(circleB[0], centerB);



            if (centerA != centerB)
            {
                float a = Vector3.Distance(centerA, centerB);
                float c = Vector3.Distance(circleA[0], circleB[0]);
                float Ө = 90 - Mathf.Rad2Deg * Mathf.Asin(a / c);
                if (Ө == float.NaN)
                    Ө = 0;




                if (radiusB > radiusA)
                    Ө = -Ө;

                for (int i = 0; i < circleA.Length; i++)
                {
                    Vector3 normal = (circleA[i] - centerA).normalized;


                    //The radius calculation has a small marginal of error that this if check takes into account.
                    if (Mathf.Abs(radiusA - radiusB) > 0.001f)
                    {
                        Vector3 side = Vector3.Cross(normal, up);
                        normal = Quaternion.AngleAxis(Ө, side) * normal;
                    }

                    normals.Add(normal * normalSign);
                }

                for (int i = 0; i < circleB.Length; i++)
                {
                    Vector3 normal = (circleB[i] - centerB).normalized;


                    //The radius calculation has a small marginal of error that this if check takes into account.
                    if (Mathf.Abs(radiusA - radiusB) > 0.001f)
                    {
                        Vector3 side = Vector3.Cross(normal, up);
                        normal = Quaternion.AngleAxis(Ө, side) * normal;
                    }

                    normals.Add(normal * normalSign);
                }


            }

            else
            {
                for (int i = 0; i < circleA.Length + circleB.Length; i++)
                {
                    if (radiusA > radiusB)
                        normals.Add(up * normalSign);
                    else
                        normals.Add(-up * normalSign);
                }
            }


            for (int i = 0; i < circleA.Length - 1; i++)
            {
                if (!flipTris)
                {
                    tris.Add(vert);
                    tris.Add(vert + 1);
                    tris.Add(vert + circleA.Length);

                    tris.Add(vert + 1);
                    tris.Add(vert + circleA.Length + 1);
                    tris.Add(vert + circleA.Length);
                }

                else
                {
                    tris.Add(vert + circleA.Length);
                    tris.Add(vert + 1);
                    tris.Add(vert);

                    tris.Add(vert + circleA.Length);
                    tris.Add(vert + circleA.Length + 1);
                    tris.Add(vert + 1);
                }

                vert++;
            }


            vertices.Add(circleA[0]); //count -1
            normals.Add(normals[normals.Count - circleA.Length]);
            vertices.Add(circleB[0]); //count -2
            normals.Add(normals[normals.Count - circleB.Length - 1]);

            if (!flipTris)
            {
                tris.Add(vertices.Count - 1);
                tris.Add(vertices.Count - 3);
                tris.Add(vertices.Count - 2);

                tris.Add(vertices.Count - 2);
                tris.Add(vertices.Count - 3);
                tris.Add(vertices.Count - 3 - circleA.Length);
            }

            else
            {
                tris.Add(vertices.Count - 2);
                tris.Add(vertices.Count - 3);
                tris.Add(vertices.Count - 1);

                tris.Add(vertices.Count - 3 - circleA.Length);
                tris.Add(vertices.Count - 3);
                tris.Add(vertices.Count - 2);
            }

            for (int i = 0; i < circleA.Length; i++)
            {
                uvs.Add(new Vector2((float)i / circleA.Length, uvStart));
            }

            for (int i = 0; i < circleB.Length; i++)
            {
                uvs.Add(new Vector2((float)i / circleB.Length, uvEnd));
            }

            uvs.Add(new Vector2(1, uvStart));
            uvs.Add(new Vector2(1, uvEnd));



        }

        protected void FillCircle(Vector3[] circle, List<int> tris, bool flipFace = false)
        {
            Vector3 center = Vector3.zero;

            for (int i = 0; i < circle.Length; i++)
                center += circle[i];

            center /= circle.Length;

            vertices.AddRange(circle); // count - upperCircle.Length + i
            vertices.Add(center); //count - 1;

            Vector2 lowerBound = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 upperBound = new Vector2(float.MinValue, float.MinValue);

            // Calculate the normal vector
            Vector3 normal = Vector3.zero;
            for (int i = 0; i < circle.Length; i++)
            {
                Vector3 current = circle[i];
                Vector3 next = circle[(i + 1) % circle.Length];

                // Compute the cross product
                normal.x += (current.y - center.y) * (next.z - center.z) - (current.z - center.z) * (next.y - center.y);
                normal.y += (current.z - center.z) * (next.x - center.x) - (current.x - center.x) * (next.z - center.z);
                normal.z += (current.x - center.x) * (next.y - center.y) - (current.y - center.y) * (next.x - center.x);
            }

            normal.Normalize(); // Normalize the normal vector to get unit length

            if(flipFace)
                for (int i = 0; i <= circle.Length; i++)
                    normals.Add(-normal);
            else
                for (int i = 0; i <= circle.Length; i++)
                    normals.Add(normal);

            for (int i = 0; i < circle.Length - 1; i++)
            {
                if (flipFace)
                {
                    //Connects vertices in a pizza slice like fashion.
                    tris.Add(vertices.Count - circle.Length + i);
                    tris.Add(vertices.Count - 1);
                    tris.Add(vertices.Count - circle.Length + i + 1);
                }

                else
                {
                    tris.Add(vertices.Count - circle.Length + i + 1);
                    tris.Add(vertices.Count - 1);
                    tris.Add(vertices.Count - circle.Length + i);
                }

                //Finds out which vertex should be (0,0) on the UV map.
                if (circle[i].x > upperBound.x)
                    upperBound.x = circle[i].x;
                if (circle[i].z > upperBound.y)
                    upperBound.y = circle[i].z;
                if (circle[i].x < lowerBound.x)
                    lowerBound.x = circle[i].x;
                if (circle[i].z < lowerBound.y)
                    lowerBound.y = circle[i].z;
            }

            for (int i = 0; i < circle.Length; i++)
            {
                //Remaps the values to work as UV-coordinates.
                float x = Helper.Remap(circle[i].x, lowerBound.x, upperBound.x, 0, 1f);
                float y = Helper.Remap(circle[i].z, lowerBound.y, upperBound.y, 0, 1f);
                uvs.Add(new Vector2(x, y));
            }

            //Final 2 slices
            if (flipFace)
            {
                //Final 2 slices
                tris.Add(vertices.Count - 2);
                tris.Add(vertices.Count - 1);
                tris.Add(vertices.Count - circle.Length - 1);
                tris.Add(vertices.Count - circle.Length - 1);
                tris.Add(vertices.Count - 1);
                tris.Add(vertices.Count - circle.Length);
            }

            else
            {
                tris.Add(vertices.Count - circle.Length);
                tris.Add(vertices.Count - 1);
                tris.Add(vertices.Count - circle.Length - 1);
                tris.Add(vertices.Count - circle.Length - 1);
                tris.Add(vertices.Count - 1);
                tris.Add(vertices.Count - 2);
            }








            //Center UV
            uvs.Add(new Vector2(0.5f, 0.5f));
        }

        /// <summary>
        /// Creates a cube 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="tris"></param>
        /// <param name="scale"></param>
        /// <param name="orientation"></param>
        public void CreateCube(Vector3 center, List<int> tris, Vector3 scale, Quaternion orientation)
        {
            Vector3 bottomRightFwd = center + orientation * Vector3.Scale((Vector3.down + Vector3.right + Vector3.forward) * 0.5f, scale);
            Vector3 bottomLeftFwd = center + orientation * Vector3.Scale((Vector3.down + Vector3.left + Vector3.forward) * 0.5f, scale);
            Vector3 bottomRightBack = center + orientation * Vector3.Scale((Vector3.down + Vector3.right + Vector3.back) * 0.5f, scale);
            Vector3 bottomLeftBack = center + orientation * Vector3.Scale((Vector3.down + Vector3.left + Vector3.back) * 0.5f, scale);

            Vector3 topRightFwd = center + orientation * Vector3.Scale((Vector3.up + Vector3.right + Vector3.forward) * 0.5f, scale);
            Vector3 topLeftFwd = center + orientation * Vector3.Scale((Vector3.up + Vector3.left + Vector3.forward) * 0.5f, scale);
            Vector3 topRightBack = center + orientation * Vector3.Scale((Vector3.up + Vector3.right + Vector3.back) * 0.5f, scale);
            Vector3 topLeftBack = center + orientation * Vector3.Scale((Vector3.up + Vector3.left + Vector3.back) * 0.5f, scale);

            for (int i = 0; i < 6; i++)
            {
                uvs.Add(Vector2.one);
                uvs.Add(Vector2.up);
                uvs.Add(Vector2.right);
                uvs.Add(Vector2.zero);
            }

            //Top Plane
            vertices.AddRange(new Vector3[] { topLeftFwd, topRightFwd, topLeftBack, topRightBack });
            for (int i = 0; i < 4; i++)
                normals.Add(orientation * Vector3.up);
            SetFace();

            //Bottom plane
            vertices.AddRange(new Vector3[] { bottomRightFwd, bottomLeftFwd, bottomRightBack, bottomLeftBack });
            for (int i = 0; i < 4; i++)
                normals.Add(orientation * Vector3.down);
            SetFace();

            //Front Plane
            vertices.AddRange(new Vector3[] { bottomLeftFwd, bottomRightFwd, topLeftFwd, topRightFwd });
            for (int i = 0; i < 4; i++)
                normals.Add(orientation * Vector3.forward);
            SetFace();

            //Back Plane
            vertices.AddRange(new Vector3[] { bottomRightBack, bottomLeftBack, topRightBack, topLeftBack });
            for (int i = 0; i < 4; i++)
                normals.Add(orientation * Vector3.back);
            SetFace();

            //Right Plane
            vertices.AddRange(new Vector3[] { bottomRightBack, topRightBack, bottomRightFwd, topRightFwd });
            for (int i = 0; i < 4; i++)
                normals.Add(orientation * Vector3.right);
            SetFace();

            //Left Plane
            vertices.AddRange(new Vector3[] { bottomLeftFwd, topLeftFwd, bottomLeftBack, topLeftBack });
            for (int i = 0; i < 4; i++)
                normals.Add(orientation * Vector3.left);
            SetFace();


            //Function for triangle connection
            void SetFace()
            {
                int vert = vertices.Count;

                tris.Add(vert - 4);
                tris.Add(vert - 3);
                tris.Add(vert - 2);

                tris.Add(vert - 3);
                tris.Add(vert - 1);
                tris.Add(vert - 2);
            }
        }

        /// <summary>
        /// Creates a cube without the front and back face
        /// </summary>
        /// <param name="center"></param>
        /// <param name="tris"></param>
        /// <param name="scale"></param>
        /// <param name="orientation"></param>
        public void Create4FaceCube(Vector3 center, List<int> tris, Vector3 scale, Quaternion orientation)
        {
            Vector3 bottomRightFwd = center + orientation * Vector3.Scale((Vector3.down + Vector3.right + Vector3.forward) * 0.5f, scale);
            Vector3 bottomLeftFwd = center + orientation * Vector3.Scale((Vector3.down + Vector3.left + Vector3.forward) * 0.5f, scale);
            Vector3 bottomRightBack = center + orientation * Vector3.Scale((Vector3.down + Vector3.right + Vector3.back) * 0.5f, scale);
            Vector3 bottomLeftBack = center + orientation * Vector3.Scale((Vector3.down + Vector3.left + Vector3.back) * 0.5f, scale);

            Vector3 topRightFwd = center + orientation * Vector3.Scale((Vector3.up + Vector3.right + Vector3.forward) * 0.5f, scale);
            Vector3 topLeftFwd = center + orientation * Vector3.Scale((Vector3.up + Vector3.left + Vector3.forward) * 0.5f, scale);
            Vector3 topRightBack = center + orientation * Vector3.Scale((Vector3.up + Vector3.right + Vector3.back) * 0.5f, scale);
            Vector3 topLeftBack = center + orientation * Vector3.Scale((Vector3.up + Vector3.left + Vector3.back) * 0.5f, scale);


            //UVs
            for (int i = 0; i < 4; i++)
            {
                uvs.Add(Vector2.one);
                uvs.Add(Vector2.up);
                uvs.Add(Vector2.right);
                uvs.Add(Vector2.zero);
            }

            //Top Plane
            vertices.AddRange(new Vector3[] { topLeftFwd, topRightFwd, topLeftBack, topRightBack });
            for (int i = 0; i < 4; i++)
                normals.Add(orientation * Vector3.up);
            SetFace();

            //Bottom plane
            vertices.AddRange(new Vector3[] { bottomRightFwd, bottomLeftFwd, bottomRightBack, bottomLeftBack });
            for (int i = 0; i < 4; i++)
                normals.Add(orientation * Vector3.down);
            SetFace();

            //Right Plane
            vertices.AddRange(new Vector3[] { bottomRightBack, topRightBack, bottomRightFwd, topRightFwd });
            for (int i = 0; i < 4; i++)
                normals.Add(orientation * Vector3.right);
            SetFace();

            //Left Plane
            vertices.AddRange(new Vector3[] { bottomLeftFwd, topLeftFwd, bottomLeftBack, topLeftBack });
            for (int i = 0; i < 4; i++)
                normals.Add(orientation * Vector3.left);
            SetFace();


            //Function for triangle connection
            void SetFace()
            {
                int vert = vertices.Count;

                tris.Add(vert - 4);
                tris.Add(vert - 3);
                tris.Add(vert - 2);

                tris.Add(vert - 3);
                tris.Add(vert - 1);
                tris.Add(vert - 2);
            }
        }

        /// <summary>
        /// Creates a plane of vertices with an even uv-tiling.
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="size"></param>
        /// <param name="xDir"></param>
        /// <param name="yDir"></param>
        /// <param name="tiling"></param>
        /// <param name="tris"></param>
        /// <param name="flipTris"></param>
        /// <param name="invertUVX"></param>
        /// <param name="invertUVY"></param>
        protected void CreatePlane(Vector3 startPoint, Vector2 size, Vector3 xDir, Vector3 yDir, Vector2 tiling, List<int> tris, bool flipTris = false, bool invertUVX = false, bool invertUVY = false)
        {
            float xSections = (size.x / tiling.x).Floor();
            float ySections = (size.y / tiling.y).Floor();
            float endUVX = (size.x - tiling.x * xSections) / tiling.x;
            float endUVY = (size.y - tiling.y * ySections) / tiling.y;


            int vertCount;

            int[] trisToAdd;


            Vector3 normal = Vector3.Cross(xDir, yDir);

            if (flipTris)
                normal = -normal;


            for (int x = 0; x < xSections; x++)
            {
                for (int y = 0; y < ySections; y++)
                {
                    vertCount = vertices.Count;
                    vertices.Add(startPoint + yDir * tiling.y * y + xDir * tiling.x * x);
                    vertices.Add(startPoint + yDir * tiling.y * y + xDir * tiling.x * (x + 1));
                    vertices.Add(startPoint + yDir * tiling.y * (y + 1) + xDir * tiling.x * x);
                    vertices.Add(startPoint + yDir * tiling.y * (y + 1) + xDir * tiling.x * (x + 1));
                    trisToAdd = new int[] { vertCount, vertCount + 1, vertCount + 2, vertCount + 1, vertCount + 3, vertCount + 2 };

                    if (flipTris)
                        trisToAdd = trisToAdd.ReverseOrder();

                    tris.AddRange(trisToAdd);
                    uvs.AddRange(new Vector2[] {
                        new Vector2(invertUVX ? 1 : 0, invertUVY ? 1 : 0),
                        new Vector2(invertUVX ? 0 : 1, invertUVY ? 1 : 0),
                        new Vector2(invertUVX ? 1 : 0, invertUVY ? 0 : 1),
                        new Vector2(invertUVX ? 0 : 1, invertUVY ? 0 : 1)});

                    for (int i = 0; i < 4; i++)
                        normals.Add(normal);
                }

                vertCount = vertices.Count;
                vertices.Add(startPoint + yDir * tiling.y * ySections + xDir * tiling.x * x);
                vertices.Add(startPoint + yDir * tiling.y * ySections + xDir * tiling.x * (x + 1));
                vertices.Add(startPoint + yDir * size.y + xDir * tiling.x * x);
                vertices.Add(startPoint + yDir * size.y + xDir * tiling.x * (x + 1));
                trisToAdd = new int[] { vertCount, vertCount + 1, vertCount + 2, vertCount + 1, vertCount + 3, vertCount + 2 };

                if (flipTris)
                    trisToAdd = trisToAdd.ReverseOrder();

                tris.AddRange(trisToAdd);
                uvs.AddRange(new Vector2[] {
                        new Vector2(invertUVX ? 1 : 0, invertUVY ? 1: 0),
                        new Vector2(invertUVX ? 0 : 1, invertUVY ? 1: 0),
                        new Vector2(invertUVX ? 1 : 0, invertUVY ? 1 - endUVY : endUVY),
                        new Vector2(invertUVX ? 0 : 1, invertUVY ? 1 - endUVY: endUVY)});
                //uvs.AddRange(new Vector2[] { Vector2.zero, Vector2.right, new Vector2(0, endUVY), new Vector2(1, endUVY) });

                for (int i = 0; i < 4; i++)
                    normals.Add(normal);
            }

            for (int y = 0; y < ySections; y++)
            {
                vertCount = vertices.Count;
                vertices.Add(startPoint + yDir * tiling.y * y + xDir * tiling.x * xSections);
                vertices.Add(startPoint + yDir * tiling.y * y + xDir * size.x);
                vertices.Add(startPoint + yDir * tiling.y * (y + 1) + xDir * tiling.x * xSections);
                vertices.Add(startPoint + yDir * tiling.y * (y + 1) + xDir * size.x);

                trisToAdd = new int[] { vertCount, vertCount + 1, vertCount + 2, vertCount + 1, vertCount + 3, vertCount + 2 };

                if (flipTris)
                    trisToAdd = trisToAdd.ReverseOrder();

                tris.AddRange(trisToAdd);
                uvs.AddRange(new Vector2[] {
                        new Vector2(invertUVX ? endUVX : 0, invertUVY ? 1 : 0),
                        new Vector2(invertUVX ? 0 : endUVX, invertUVY ? 1 : 0),
                        new Vector2(invertUVX ? endUVX : 0, invertUVY ? 0 : 1),
                        new Vector2(invertUVX ? 0 : endUVX, invertUVY ? 0 : 1)});

                for (int i = 0; i < 4; i++)
                    normals.Add(normal);
            }

            vertCount = vertices.Count;
            vertices.Add(startPoint + yDir * tiling.y * ySections + xDir * tiling.x * xSections);
            vertices.Add(startPoint + yDir * tiling.y * ySections + xDir * size.x);
            vertices.Add(startPoint + yDir * size.y + xDir * tiling.x * xSections);
            vertices.Add(startPoint + yDir * size.y + xDir * size.x);

            trisToAdd = new int[] { vertCount, vertCount + 1, vertCount + 2, vertCount + 1, vertCount + 3, vertCount + 2 };

            if (flipTris)
                trisToAdd = trisToAdd.ReverseOrder();

            tris.AddRange(trisToAdd);
            uvs.AddRange(new Vector2[] {
                        new Vector2(invertUVX ? endUVX : 0, invertUVY ? 1 : 0),
                        new Vector2(invertUVX ? 0 : endUVX, invertUVY ? 1: 0),
                        new Vector2(invertUVX ? endUVX : 0, invertUVY ? 1 - endUVY  : endUVY),
                        new Vector2(invertUVX ? 0 : endUVX, invertUVY ? 1 - endUVY : endUVY)});

            for (int i = 0; i < 4; i++)
                normals.Add(normal);


        }

        protected void AddMesh(Mesh mesh, List<int> tris, Vector3 position, Vector3 scale, Quaternion orientation)
        {
            Vector3[] vertices = mesh.vertices;
            int vertCount = this.vertices.Count;

            //Vertices
            for (int i = 0; i < mesh.vertices.Length; i++)
            {
                vertices[i] = mesh.vertices[i];
                vertices[i].Scale(scale);
                vertices[i] = orientation * vertices[i];
                vertices[i] += position;
            }

            this.vertices.AddRange(vertices);

            int[] trisToAdd = mesh.triangles;
            for (int i = 0; i < trisToAdd.Length; i++)
            {
                trisToAdd[i] += vertCount;
            }

            tris.AddRange(trisToAdd);

            Vector3[] normals = new Vector3[mesh.normals.Length];

            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = orientation * mesh.normals[i];
            }

            this.normals.AddRange(normals);

            //Uvs
            uvs.AddRange(mesh.uv);
        }

        protected void AddMesh(Mesh mesh, List<int> tris, Vector3 position, Vector3 scale)
        {
            Vector3[] vertices = mesh.vertices;
            int vertCount = this.vertices.Count;

            //Vertices
            for (int i = 0; i < mesh.vertices.Length; i++)
            {
                vertices[i] = mesh.vertices[i];
                vertices[i].Scale(scale);
                vertices[i] += position;
            }

            this.vertices.AddRange(vertices);

            int[] trisToAdd = mesh.triangles;
            for (int i = 0; i < trisToAdd.Length; i++)
            {
                trisToAdd[i] += vertCount;
            }

            tris.AddRange(trisToAdd);
            normals.AddRange(mesh.normals);
            uvs.AddRange(mesh.uv);
        }

        
    }
}
