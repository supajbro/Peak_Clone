using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GenerateWall;

public class GenerateWall : NetworkBehaviour
{
    [SerializeField] private WallFace[] _wallFaces;
    [SerializeField] private Block[] _climbBlockPrefabs;
    private List<BlockSpawnData> _spawnDataList = new List<BlockSpawnData>();
    [SerializeField] private Transform _parent;

    [SyncVar] private int _generationSeed;

    public struct BlockSpawnData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public int PrefabIndex;
    }

    /// <summary>
    /// When the host joins the server
    /// </summary>
    public override void OnStartServer()
    {
        base.OnStartServer();
        _generationSeed = Random.Range(int.MinValue, int.MaxValue);
        GenerateAndSpawn(_generationSeed);
    }

    /// <summary>
    /// When another client joins the server
    /// </summary>
    public override void OnStartClient()
    {
        base.OnStartClient();

        // already handled
        if (isServer)
        {
            return;
        }

        // already spawned
        if (NetworkServer.active)
        {
            return;
        }

        if (_generationSeed != 0)
        {
            // Wait until SyncVar arrives
            GenerateAndSpawn(_generationSeed);
        }
    }

    [Server]
    private void GenerateAndSpawn(int seed)
    {
        if (!NetworkServer.active && !isServer)
        {
            return;
        }

        // Set the random seed
        Random.InitState(seed);

        // Generate the terrain
        foreach (var wall in _wallFaces)
        {
            wall.Generate(_climbBlockPrefabs, _spawnDataList);
        }

        // Spawn these over the networked
        foreach (var data in _spawnDataList)
        {
            GameObject prefab = _climbBlockPrefabs[data.PrefabIndex].Prefab;
            GameObject obj = Instantiate(prefab, data.Position, data.Rotation, _parent);
            obj.GetComponent<ProceduralBlock>().Init(GetComponent<NetworkIdentity>());
            NetworkServer.Spawn(obj);
        }
    }
}

[System.Serializable]
public class WallFace
{
    public Transform Face;
    public float BlockSpacing = 15f;
    public float SpawnChance = .6f;
    public int[] BlocksToIgnore;

    public void Generate(Block[] blockPrefabs, List<BlockSpawnData> outSpawnList)
    {
        Vector3 up = Face.forward;
        Vector3 right = Face.right;
        Vector3 forward = Face.up;

        Renderer rend = Face.GetComponent<Renderer>();
        Vector3 wallSize = rend.bounds.size;
        Vector3 basePos = Face.position - up * (wallSize.y / 2f);

        for (float z = 0; z < wallSize.y; z += BlockSpacing)
        {
            for (float x = -wallSize.x / 2f; x < wallSize.x / 2f; x += BlockSpacing)
            {
                // Random chance on what will spawn
                if (Random.value > SpawnChance)
                {
                    continue;
                }

                Vector3 localOffset = right * x + up * z;
                Vector3 worldPos = basePos + localOffset;

                // Offset this block randomly on the X axis
                float xOffset = 5f;
                worldPos += right * Random.Range(-xOffset, xOffset);
                worldPos += forward * 0.01f;

                // Set the index of the prefab and ensure this face isn't ignoring it (cant spawn there)
                int prefabIndex = Random.Range(0, blockPrefabs.Length);
                do
                {
                    prefabIndex = Random.Range(0, blockPrefabs.Length);
                }
                while (BlocksToIgnore.Contains(prefabIndex));

                // Random rot of each block
                Quaternion rotation = blockPrefabs[prefabIndex].GetRandomRotation();

                // Add to the networked block spawn data so other clients can get this data
                outSpawnList.Add(new BlockSpawnData
                {
                    Position = worldPos,
                    Rotation = rotation,
                    PrefabIndex = prefabIndex
                });
            }
        }
    }
}

[System.Serializable]
public class Block
{
    public GameObject Prefab;
    public bool CanRotate = true;
    public Quaternion[] AvailableRotations;

    public void SpawnBlock(Vector3 pos, Transform parent)
    {
        var rot = AvailableRotations[Random.Range(0, AvailableRotations.Length)];
        GameObject block = GenerateWall.Instantiate(Prefab, pos, rot, parent);
    }

    /// <summary>
    /// Randomly rotate block within the available rotations set
    /// </summary>
    /// <returns>Rotation</returns>
    public Quaternion GetRandomRotation()
    {
        if (AvailableRotations != null && AvailableRotations.Length > 0)
            return AvailableRotations[Random.Range(0, AvailableRotations.Length)];

        return Quaternion.identity;
    }
}
