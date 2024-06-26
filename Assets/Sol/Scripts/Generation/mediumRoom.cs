using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mediumRoom : MonoBehaviour
{
    private start root;
    [SerializeField] private List<GameObject> spawners = new List<GameObject>();
    private void OnEnable()
    {
        Debug.Log("Placed Medium Room, Script is running!");

        // define root script
        root = GameObject.FindGameObjectWithTag("root").GetComponent<start>();
        root.placedRooms.Add(gameObject);

        // randomize the order of spawners in list
        for (int i = spawners.Count - 1; i >= 0; i--)
        {
            int rand = root.RNG(i + 1);
            GameObject temp = spawners[rand];
            spawners[rand] = spawners[i];
            spawners[i] = temp;
        }

        // generate small rooms
        foreach (GameObject spawner in spawners)
        {
            PlaceRoom(spawner, root.debug, root.roomChance);
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
            chance = root.RNG(2);
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
