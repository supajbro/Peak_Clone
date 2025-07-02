using UnityEngine;

public class Spinning : MonoBehaviour
{
    [SerializeField] private Vector3 _rotationAxis = Vector3.forward;
    [SerializeField] private float _rotationSpeed = 100f;

    void Update()
    {
        transform.Rotate(_rotationAxis, _rotationSpeed * Time.deltaTime);
    }
}
