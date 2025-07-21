using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class EnableNextSkyscraper : NetworkBehaviour
{
    [SerializeField] private GameObject _next;
    [SerializeField] private MeshRenderer _mesh;
    [SyncVar(hook = nameof(OnNextSkyscraperStateChanged))]
    private bool _isNextActive = false;
    public bool IsNextActive => _isNextActive;
    [SerializeField] private List<GameObject> _objsToEnable;

    [Header("Dissolve")]
    [SerializeField] private MeshRenderer _dissolveMesh;
    private Material _dissolveMat;

    private void Start()
    {
        if (_dissolveMesh != null)
        {
            _dissolveMat = _dissolveMesh?.material;
            _dissolveMesh.material = new Material(_dissolveMat);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer) return;

        if (other.CompareTag("Player") && !_isNextActive)
        {
            _isNextActive = true;
        }
    }

    private void OnNextSkyscraperStateChanged(bool oldValue, bool newValue)
    {
        //_next.SetActive(newValue);
        _mesh.enabled = newValue;

        if (newValue)
        {
            foreach (var _obj in _objsToEnable)
            {
                _obj.SetActive(true);
            }

            StartCoroutine(Dissolve());
        }
    }

    public override void OnStartClient()
    {
        // Late joiners will have the SyncVar's current value
        //_next.SetActive(_isNextActive);
        _mesh.enabled = _isNextActive;

        if (_isNextActive)
        {
            foreach (var _obj in _objsToEnable)
            {
                _obj.SetActive(true);
            }

            StartCoroutine(Dissolve());
        }
    }

    private IEnumerator Dissolve()
    {
        while(_dissolveMesh.material.GetFloat("_Dissolve") < 1f)
        {
            _dissolveMesh.material.SetFloat("_Dissolve", _dissolveMesh.material.GetFloat("_Dissolve") + Time.deltaTime * .5f);
            yield return null;
        }
        _dissolveMesh.enabled = false;
    }
}
