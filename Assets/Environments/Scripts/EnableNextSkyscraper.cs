using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class EnableNextSkyscraper : NetworkBehaviour
{
    [SerializeField] private GameObject _next;
    [SerializeField] private MeshRenderer _mesh;

    [Header("Check to see if any user has reached top")]
    [SyncVar(hook = nameof(OnNextSkyscraperStateChanged))]
    [SerializeField] private bool _reachedTopSkyscraper = false;
    public bool ReachedTopSkyscraper => _reachedTopSkyscraper;

    [Header("Check to enable next skyscraper")]
    [SyncVar(hook = nameof(OnNextSkyscraperStateChanged))]
    [SerializeField] private bool _activateNextSkyscraper = false;
    public bool ActivateNextSkyscraper => _activateNextSkyscraper;

    [Header("Enable these objects when skyscraper is enabled")]
    [SerializeField] private List<GameObject> _objsToEnable;

    [Header("Dissolve Material")]
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

        if (other.CompareTag("Player") && !_activateNextSkyscraper)
        {
            _activateNextSkyscraper = true;
            _reachedTopSkyscraper = true;
        }
    }

    private void OnNextSkyscraperStateChanged(bool oldValue, bool newValue)
    {
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
        _mesh.enabled = _activateNextSkyscraper;

        if (_activateNextSkyscraper)
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
