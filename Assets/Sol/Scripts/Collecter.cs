using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collecter : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Collectable") && !Input.GetMouseButton(0))
        {
            Debug.Log("Collected Object");
            // collision is collectable
            GameObject[] player = GameObject.FindGameObjectsWithTag("Player");

            if (other.gameObject.GetComponent<Object>().objective)
            {
                player[0].GetComponent<PlayerManager>().score += other.gameObject.GetComponent<Object>().score * 3;
            } else
            {
                player[0].GetComponent<PlayerManager>().score += other.gameObject.GetComponent<Object>().score;
            }

            Destroy(other.gameObject);
        }
    }
}
