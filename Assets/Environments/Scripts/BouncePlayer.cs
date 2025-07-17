using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePlayer : MonoBehaviour
{
    [SerializeField] private GameObject ignoreObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player")
        {
            return;
        }

        Debug.Log("[Bounce] Player bounce");
        Player player = other.gameObject.GetComponent<Player>();

        if(player.gameObject == ignoreObject)
        {
            return;
        }

        player.BouncePlayer();
    }
}
