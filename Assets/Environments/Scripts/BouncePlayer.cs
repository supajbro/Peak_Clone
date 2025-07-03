using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePlayer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player")
        {
            return;
        }

        Debug.Log("[Bounce] Player bounce");
        Player player = other.gameObject.GetComponent<Player>();
        player.BouncePlayer();
    }
}
