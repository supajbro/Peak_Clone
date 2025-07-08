using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableNextSkyscraper : MonoBehaviour
{
    [SerializeField] private GameObject _next;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            _next.SetActive(true);
        }
    }
}
