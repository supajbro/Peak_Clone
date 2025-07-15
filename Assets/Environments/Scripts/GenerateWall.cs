using Mirror;
using System.Collections.Generic;
using UnityEngine;
using static GenerateWall;

public class GenerateWall : NetworkBehaviour
{
    [SerializeField] private WallFace[] _wallFaces;
    [SerializeField] private Block[] _climbBlockPrefabs;
    [SerializeField] private int _generationSeed = 42;
    private List<BlockSpawnData> _spawnDataList = new List<BlockSpawnData>();

    public struct BlockSpawnData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public int PrefabIndex;
    }

    private void Start()
    {
        //GenerateAllWalls();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        GenerateAndSpawn();
    }

    [Server]
    private void GenerateAndSpawn()
    {
        Random.InitState(1234); // Make it reproducible

        foreach (var wall in _wallFaces)
        {
            wall.Generate(transform, _climbBlockPrefabs, _spawnDataList);
        }

        // Actually spawn
        foreach (var data in _spawnDataList)
        {
            Block block = _climbBlockPrefabs[data.PrefabIndex];
            GameObject obj = Instantiate(block.Prefab, data.Position, data.Rotation);
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
    [SerializeField] private Block[] _climbBlockPrefabs;

    public void Generate(Transform parent, Block[] blockPrefabs, List<BlockSpawnData> outSpawnList)
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
                if (Random.value > SpawnChance)
                    continue;

                Vector3 localOffset = right * x + up * z;
                Vector3 worldPos = basePos + localOffset;

                float xOffset = 5f;
                worldPos += right * Random.Range(-xOffset, xOffset);
                worldPos += forward * 0.01f;

                int prefabIndex = Random.Range(0, blockPrefabs.Length);
                Quaternion rotation = blockPrefabs[prefabIndex].GetRandomRotation();

                outSpawnList.Add(new BlockSpawnData
                {
                    Position = worldPos,
                    Rotation = rotation,
                    PrefabIndex = prefabIndex
                });
            }
        }
    }

    Block GetRandomBlock()
    {
        return _climbBlockPrefabs[Random.Range(0, _climbBlockPrefabs.Length)];
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

    public Quaternion GetRandomRotation()
    {
        if (AvailableRotations != null && AvailableRotations.Length > 0)
            return AvailableRotations[Random.Range(0, AvailableRotations.Length)];

        return Quaternion.identity;
    }
}
