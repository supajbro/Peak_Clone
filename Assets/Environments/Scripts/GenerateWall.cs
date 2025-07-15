using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class GenerateWall : MonoBehaviour
{
    [SerializeField] private WallFace[] _wallFaces;
    [SerializeField] private Block[] _climbBlockPrefabs;

    private void Start()
    {
        GenerateAllWalls();
    }

    private void GenerateAllWalls()
    {
        foreach (var wall in _wallFaces)
        {
            wall.Generate(transform);
        }
    }

    //private void Generate(WallFace wall)
    //{
    //    Vector3 up = wall.Face.forward;         // Climbing direction
    //    Vector3 right = wall.Face.right;        // Horizontal
    //    Vector3 forward = wall.Face.up;         // Outward-facing normal

    //    Renderer rend = wall.Face.GetComponent<Renderer>();
    //    Vector3 wallSize = rend.bounds.size;

    //    Vector3 basePos = wall.Face.position - up * (wallSize.y / 2f); // Start from bottom

    //    for (float z = 0; z < wallSize.y; z += wall.BlockSpacing)
    //    {
    //        for (float x = -wallSize.x / 2f; x < wallSize.x / 2f; x += wall.BlockSpacing)
    //        {
    //            if (Random.value > wall.SpawnChance)
    //            {
    //                continue;
    //            }

    //            Vector3 localOffset = right * x + up * z;
    //            Vector3 worldPos = basePos + localOffset;

    //            // Offset X position of the block randomly
    //            const float xOffset = 5f;
    //            var xRand = Random.Range(-xOffset, xOffset);
    //            worldPos.x += xRand;

    //            //Quaternion rot = Quaternion.LookRotation(forward) * Quaternion.Euler(90f, 0f, 0f);

    //            float surfaceOffset = 0.01f;
    //            worldPos += forward * surfaceOffset;

    //            GetRandomBlock().SpawnBlock(worldPos, transform);
    //        }
    //    }
    //}

    //Block GetRandomBlock()
    //{
    //    return _climbBlockPrefabs[Random.Range(0, _climbBlockPrefabs.Length)];
    //}
}

[System.Serializable]
public class WallFace
{
    public Transform Face;
    public float BlockSpacing = 15f;
    public float SpawnChance = .6f;
    [SerializeField] private Block[] _climbBlockPrefabs;

    public void Generate(Transform parent)
    {
        Vector3 up = Face.forward;         // Climbing direction
        Vector3 right = Face.right;        // Horizontal
        Vector3 forward = Face.up;         // Outward-facing normal

        Renderer rend = Face.GetComponent<Renderer>();
        Vector3 wallSize = rend.bounds.size;

        Vector3 basePos = Face.position - up * (wallSize.y / 2f); // Start from bottom

        for (float z = 0; z < wallSize.y; z += BlockSpacing)
        {
            for (float x = -wallSize.x / 2f; x < wallSize.x / 2f; x += BlockSpacing)
            {
                if (Random.value > SpawnChance)
                {
                    continue;
                }

                Vector3 localOffset = right * x + up * z;
                Vector3 worldPos = basePos + localOffset;

                // Offset X position of the block randomly
                const float xOffset = 5f;
                var xRand = Random.Range(-xOffset, xOffset);
                worldPos.x += xRand;

                //Quaternion rot = Quaternion.LookRotation(forward) * Quaternion.Euler(90f, 0f, 0f);

                float surfaceOffset = 0.01f;
                worldPos += forward * surfaceOffset;

                GetRandomBlock().SpawnBlock(worldPos, parent);
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
}
