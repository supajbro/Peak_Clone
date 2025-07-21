using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindAudioSetter : MonoBehaviour
{
    private Player _target;
    [SerializeField] private GameObject _endGoal;

    [SerializeField] private float _minVol = .1f;
    [SerializeField] private float _maxVol = 1f;

    [SerializeField] private AudioSource _source;

    private void Start()
    {
        _source.volume = _minVol;
        _source.Play();
    }

    private void Update()
    {
        if (_target == null)
        {
            _target = GameManager.Instance.LocalPlayer;
            return;
        }

        if (_endGoal == null)
        {
            return;
        }

        float height = _target.transform.position.y;
        float highestPoint = _endGoal.transform.position.y;

        float t = Mathf.Clamp01(height / highestPoint);
        float volume = Mathf.Lerp(_minVol, _maxVol, t);

        _source.volume = volume;
    }
}
