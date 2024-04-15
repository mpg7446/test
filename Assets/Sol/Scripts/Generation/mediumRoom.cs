using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mediumRoom : MonoBehaviour
{
    private Vector3 roomSize;
    private start root;
    [SerializeField] private List<GameObject> spawners = new List<GameObject>();
    private void OnEnable()
    {

        // define root script
        root = GameObject.FindGameObjectWithTag("root").GetComponent<start>();
        root.placedRooms.Add(gameObject);

        // randomize the order of spawners in list
        GameObject[] array = spawners.ToArray();
        spawners.Clear();
        for (int i = 0; i < array.Length; i++)
        {
            int rand = root.RNG(array.Length);
            GameObject temp = array[rand];
            array[rand] = temp;
            Debug.Log(temp.name + ": moved to array pos " + rand + " from pos " + i);
        }
        
        if (root.roomCount > 0)
        {
            root.roomCount--;

            // generate small rooms
            foreach (GameObject spawner in spawners)
            {
                PlaceRoom(spawner, 2);
            }
        }

    }

    private void PlaceRoom(GameObject spawner,int chance = 5)
    {
        chance = root.RNG(chance);
        if (chance == 0)
        {
            // place medium / small room here
            chance = root.RNG(5);
            if (chance == 0)
            {
                // place small room
            }
            else
            {
                // place medium room
            }

            // place wall with door

        }
        else
        {
            // place blank wall
        }
    }
}
