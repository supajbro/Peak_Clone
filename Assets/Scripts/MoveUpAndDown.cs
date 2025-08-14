using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUpAndDown : MonoBehaviour
{
    [Header("Float Settings")]
    [SerializeField, Tooltip("Height of movement")] private float _amplitude = 1f;
    [SerializeField, Tooltip("Speed of movement")] private float _frequency = 1f;

    private Vector3 _startPos;

    void Start()
    {
        _startPos = transform.localPosition;
    }

    void Update()
    {
        float yOffset = Mathf.Sin(Time.time * _frequency) * _amplitude;
        transform.localPosition = _startPos + new Vector3(0f, yOffset, 0f);
    }
}
