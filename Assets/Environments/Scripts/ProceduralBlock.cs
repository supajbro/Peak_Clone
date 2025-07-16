using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralBlock : NetworkBehaviour
{
    [SyncVar] private NetworkIdentity _parentIdentity;
    private MeshRenderer _skyscraperMesh;
    [SerializeField] private List<MeshRenderer> _meshs;
    public void Init(NetworkIdentity parent)
    {
        _parentIdentity = parent;
    }

    private void Update()
    {
        if (_skyscraperMesh == null)
        {
            _skyscraperMesh = _parentIdentity?.gameObject.GetComponent<MeshRenderer>();
        }

        //if (_parentIdentity == null || _meshs == null || _meshs.Count == 0)
        //{
        //    return;
        //}

        bool shouldEnable = _skyscraperMesh.enabled;

        for (int i = 0; i < _meshs.Count; i++)
        {
            _meshs[i].enabled = shouldEnable;
        }

    }
}
