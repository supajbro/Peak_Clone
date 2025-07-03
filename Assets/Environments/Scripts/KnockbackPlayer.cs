using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackPlayer : MonoBehaviour
{
    [SerializeField] private float _force = 50f;
    [SerializeField] private float _upwardForce = 25f;
    [SerializeField] private float _dur = 0.5f;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag != "Player")
        {
            return;
        }

        Player player = other.GetComponent<Player>();
        if (player == null)
        {
            return;
        }

        Vector3 knockbackDirection = -player.transform.forward;
        player.Knockback(knockbackDirection, _force, _dur, _upwardForce);
    }
}
