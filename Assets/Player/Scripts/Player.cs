using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private Camera _camPrefab;
    private Camera _cam;

    private void Start()
    {
        if (isLocalPlayer)
        {
            _cam = Instantiate(_camPrefab);
        }
    }

    private void Update()
    {
        Movement();
    }

    private void Movement()
    {
        if (isLocalPlayer)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            Vector3 playerMovement = new Vector3(h * .25f, 0, v * .25f);

            transform.position += playerMovement;
        }
    }
}
