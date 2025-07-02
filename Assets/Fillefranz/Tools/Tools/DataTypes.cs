using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PropMaker;



namespace FillefranzTools
{
    public enum PropEditMode { GameObject, Points }
    public enum EditOrientation { World, LocalXZ, Local }
    public enum PlaneOptions { XY, XZ, YZ }
    public enum MeshType { Procedural, Premade}
    public enum Facing { X, Z, Automatic}
    public enum SegmentType { Line, Bezier}

    [Serializable]
    public struct MinMax
    {

        [SerializeField] float min;
        [SerializeField] float max;

        public float Min
        {
            get { return min; }
            set { min = Mathf.Min(value, max - Mathf.Epsilon); }
        }
        public float Max
        {
            get { return max; }
            set { max = Mathf.Min(value, min + Mathf.Epsilon); }
        }

        public float Middle => (min + max) / 2;
        public float Random
        {
            get
            {
                if (min == max) return min;
                else return UnityEngine.Random.Range(min, max);
            }
        }

        public float Size => max - min;


        public MinMax(float min, float max)
        {
            this.min = Mathf.Min(min, max);
            this.max = Mathf.Max(min, max);
        }

        public override bool Equals(object obj)
        {
            return obj is MinMax other &&
                   min == other.min &&
                   max == other.max;
        }

        public void Scale(float value)
        {
            min *= value;
            max *= value;
        }

        public void Shrink(float value)
        {
            min += value;
            max -= value;
        }

        public void Grow(int value)
        {
            min -= value;
            max += value;
        }

        public bool IsInBounds(float value)
        {
            return value >= min && value <= max;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(min, max);
        }

        public static bool operator ==(MinMax value, MinMax other)
        {
            return value.Equals(other);
        }

        public static bool operator !=(MinMax value, MinMax other)
        {
            return !value.Equals(other);
        }

        public static bool Overlaps(MinMax interval1, MinMax interval2)
        {
            if (interval1.min > interval2.min && interval1.min < interval2.max)
            {
                return true;
            }

            if (interval1.max < interval2.max && interval1.max > interval2.min)
            {
                return true;
            }

            if (interval1.max == interval2.max && interval1.min == interval2.min)
            {
                return true;
            }

            return false;
        }

    }

    [Serializable]
    public struct MinMaxInt
    {

        [SerializeField] int min;
        [SerializeField] int max;

        public int Min
        {
            get { return min; }
            set { min = Mathf.Min(value, max - 1); }
        }

        public int Max
        {
            get { return max; }
            set { max = Mathf.Max(value, min + 1); }
        }

        public int Middle => (min + max) / 2;
        public int Random
        {
            get
            {
                if (min == max) return min;
                else return UnityEngine.Random.Range(min, max);
            }
        }
        public int InclusiveRandom
        {
            get
            {
                if (min == max) return min;
                else return UnityEngine.Random.Range(min, max + 1);
            }
        }

        public int Size => max - min;

        public MinMaxInt(int min, int max)
        {
            this.min = Mathf.Min(min, max);
            this.max = Mathf.Max(min, max);
        }

        public void Scale(int value)
        {
            min *= value;
            max *= value;
        }

        public void Shrink(int value)
        {
            Min += value;
            Max -= value;
        }

        public void Expand(int value)
        {
            min -= value;
            max += value;
        }

        public bool IsInBounds(int value)
        {
            return value >= min && value <= max;
        }


        public override bool Equals(object obj)
        {
            return obj is MinMaxInt other &&
                   min == other.min &&
                   max == other.max;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(min, max);
        }

        public static bool operator ==(MinMaxInt value, MinMaxInt other)
        {
            return value.Equals(other);
        }

        public static bool operator !=(MinMaxInt value, MinMaxInt other)
        {
            return !value.Equals(other);
        }


        public static bool Overlaps(MinMaxInt interval1, MinMaxInt interval2)
        {
            if (interval1.min > interval2.min && interval1.min < interval2.max)
            {
                return true;
            }

            if (interval1.max < interval2.max && interval1.max > interval2.min)
            {
                return true;
            }

            if (interval1.max == interval2.max && interval1.min == interval2.min)
            {
                return true;
            }

            return false;
        }

    }

    public struct Vertex
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector2 uv;

        public Vertex(Vector3 position)
        {
            this.position = position;
            normal = Vector3.zero;
            uv = Vector2.zero;
        }

        public Vertex(Vector3 position, Vector3 normal, Vector2 uv)
        {
            this.position = position;
            this.normal = normal;
            this.uv = uv;
        }


    }

    [Serializable]
    public struct PropComponent
    {
        public GameObject gameObject { get; private set; }
        public MeshFilter meshFilter { get; private set; }
        public MeshRenderer meshRenderer { get; private set; }
        public MeshCollider meshCollider { get; private set; }
        public List<Vector3> vertices { get; private set; }
        public List<Vector2> uvs { get; private set; }
        public List<Vector3> normals { get; private set; }
        public List<List<int>> triangleLists { get; private set; }
        public List<Material> materials { get; private set; }
        public bool isNull => gameObject == null;

        public PropComponent(GameObject gameObject)
        {
            this.gameObject = gameObject;
            meshFilter = gameObject.GetComponent<MeshFilter>();

            if(meshFilter == null)
                meshFilter = gameObject.AddComponent<MeshFilter>();

            meshRenderer= gameObject.GetComponent<MeshRenderer>();

            if(meshRenderer == null)
                meshRenderer = gameObject.AddComponent<MeshRenderer>();

            meshCollider = gameObject.GetComponent<MeshCollider>();

            if(meshCollider == null)
                gameObject.AddComponent<MeshCollider>();


            vertices = new List<Vector3>();
            uvs = new List<Vector2>();
            normals= new List<Vector3>();
            triangleLists = new List<List<int>>();
            materials = new List<Material>();

            meshFilter.mesh = new Mesh();
        }

        public List<int> GetTriangleList(Material material)
        {

            for (int i = 0; i < materials.Count; i++)
            {
                if(material == materials[i])
                {
                    while (triangleLists.Count < i)
                    {
                        triangleLists.Add(new List<int>());
                    }

                    return triangleLists[i];
                }
                    
            }

            materials.Add(material);
            triangleLists.Add(new List<int>());
            return triangleLists[triangleLists.Count - 1];
        }

        public void SetMesh()
        {
            meshRenderer.sharedMaterials = materials.ToArray();

            if (meshRenderer.sharedMaterials.Length > 0 && meshFilter.sharedMesh != null)
            {
                try
                {
                    ReduceNumberOfSubmeshes();
                }
                catch { }
            }
            

            Mesh mesh = new Mesh();

            if (vertices.Count >= 65534)
                mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            else
                mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt16;

            mesh.SetVertices(vertices);
            mesh.SetNormals(normals);
            mesh.SetUVs(0, uvs);

            mesh.subMeshCount = triangleLists.Count;

            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                mesh.SetTriangles(triangleLists[i], i);
            }

            if(meshFilter == null) 
                meshFilter = gameObject.GetComponent<MeshFilter>();
            if(meshFilter == null)
                meshFilter= gameObject.AddComponent<MeshFilter>();

            meshFilter.sharedMesh = mesh;
        }

        void ReduceNumberOfSubmeshes()
        {
            List<Material> mats = new List<Material>();
            

            

            for (int i = 0; i < meshRenderer.sharedMaterials.Length; i++)
            {
                if (!mats.Contains(meshRenderer.sharedMaterials[i]))
                {
                    mats.Add(meshRenderer.sharedMaterials[i]);
                }
            }

            List<List<int>> newTris = new List<List<int>>();

            for (int i = 0; i < mats.Count; i++)
            {
                newTris.Add(new List<int>());
            }

            for (int i = 0; i < triangleLists.Count; i++)
            {
                int idx = mats.IndexOf(meshRenderer.sharedMaterials[i]);
                if (idx < 0 || idx >= mats.Count) continue;
                newTris[idx].AddRange(triangleLists[i]);
            }

            triangleLists = newTris;
            meshRenderer.materials = mats.ToArray();

        }

        public void Clear()
        {
            if (vertices == null) return;

            vertices.Clear();
            triangleLists?.Clear();
            normals.Clear();
            uvs.Clear();
            materials.Clear();
        }

    }

    [Serializable]
    public struct RoofRectangle
    {
        public Vector3Int pointA { get; private set; }
        public Vector3Int pointB { get; private set; }
        public Color color { get; private set; }
        public Facing facing;

        public RoofRectangle(Vector3Int pointA, Vector3Int pointB, Color color)
        {
            this.pointA = pointA;
            this.pointB = pointB;
            this.color = color;
            facing = Facing.Automatic;
        }
    }

    [Serializable]
    public struct Segment
    {
        public Vector3 pointA;
        public Vector3 pointB;
        public Vector3 anchorA;
        public Vector3 anchorB;
        public SegmentType segmentType;

        public Vector3[] SamplePointAtDistance(float distance)
        {
            return null;
        }
    }

}







