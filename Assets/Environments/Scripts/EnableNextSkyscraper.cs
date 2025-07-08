using Mirror;
using UnityEngine;

public class EnableNextSkyscraper : NetworkBehaviour
{
    [SerializeField] private GameObject _next;
    [SyncVar(hook = nameof(OnNextSkyscraperStateChanged))]
    private bool _isNextActive = false;

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
        _next.SetActive(newValue);
    }

    public override void OnStartClient()
    {
        // Late joiners will have the SyncVar's current value
        _next.SetActive(_isNextActive);
    }
}
