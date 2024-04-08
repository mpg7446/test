using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
    public int score = 15;
    public bool objective = false;

    void Start()
    {
        int rnd = Random.Range(0, 10);
        if (rnd == 0)
        {
            objective = true;
        }
    }
}
