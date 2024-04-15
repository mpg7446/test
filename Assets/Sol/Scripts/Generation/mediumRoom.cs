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
        Debug.Log("Placed Medium Room, Script is runnig!");

        // define root script
        root = GameObject.FindGameObjectWithTag("root").GetComponent<start>();
        root.placedRooms.Add(gameObject);

        // randomize the order of spawners in list - PLEASE FIX THIS - TODO
        GameObject[] array = spawners.ToArray();
        //spawners.Clear();
        for (int i = 0; i < array.Length; i++)
        {
            int rand = root.RNG(array.Length);
            GameObject temp = array[rand];
            array[rand] = temp;
            //Debug.Log(temp.name + ": moved to array pos " + rand + " from pos " + i);
        }

        // generate small rooms
        foreach (GameObject spawner in spawners)
        {
            PlaceRoom(spawner, root.debug, 2);
        }

    }

    private void PlaceRoom(GameObject spawner, bool debug = false,int chance = 5)
    {
        chance = root.RNG(chance);
        bool bounding = spawner.transform.position.z > 0 && spawner.transform.position.z <= 20
            && spawner.transform.position.x >= -15 && spawner.transform.position.x <= 15;

        if (root.roomCount > 0 && chance == 0 && bounding)
        {
            root.roomCount--;
            // place room here at game object locations

            GameObject roomToPlace;
            // place medium / small room here
            chance = root.RNG(5);
            if (chance == 0)
            {
                int rand = root.RNG(root.smallRooms.Count);
                // place small room
                if (debug)
                {
                    roomToPlace = root.smallDebugRoom;
                }
                else
                {
                    roomToPlace = root.smallRooms[rand];
                }
            }
            else
            {
                int rand = root.RNG(root.mediumRooms.Count);
                // place medium room
                if (debug)
                {
                    roomToPlace = root.mediumDebugRoom;
                }
                else
                {
                    roomToPlace = root.mediumRooms[rand];
                }
            }

            GameObject room = Instantiate(roomToPlace, Vector3.zero, new Quaternion(), spawner.transform);
            room.transform.localPosition = Vector3.zero;

        }
        else
        {
            // place blank wall
            if (root.debug)
            {
                GameObject wall = Instantiate(root.debugWall, Vector3.zero, new Quaternion(), spawner.transform);
                wall.transform.localPosition = Vector3.zero;
            }
        }
    }
}
