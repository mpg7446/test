using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class room : MonoBehaviour
{
    private start root;
    [SerializeField] private List<GameObject> spawners = new List<GameObject>();
    private void OnEnable()
    {
        // define root script
        root = GameObject.FindGameObjectWithTag("root").GetComponent<start>();
        root.placedRooms.Add(gameObject);

        // set name and update roomID
        gameObject.name = "room " + root.roomID;
        root.roomID++;

        Debug.Log("Placed Room " + gameObject.name + ".");

        // randomize the order of spawners in list
        for (int i = spawners.Count - 1; i >= 0; i--)
        {
            int rand = root.RNG(i + 1);
            GameObject temp = spawners[rand];
            spawners[rand] = spawners[i];
            spawners[i] = temp;
        }

        // place rooms on spawners
        foreach (GameObject spawner in spawners)
        {
            int chance = root.RNG(2);
            if (root.maxLargeRooms > 0 && chance == 0 && CheckEmpty(spawner, true))
            {
                // generate large rooms
                PlaceRoom(spawner, true, root.debug, root.largeRoomChance);
            }
            else if (CheckEmpty(spawner, false))
            {
                // generate smaller rooms
                PlaceRoom(spawner, false, root.debug);
            }
        }
    }

    private void PlaceRoom(GameObject spawner, bool largeRoom = false, bool debug = false, int chance = 2) // TODO if possible, stop wall doubling from rooms spawning next to each other
    {
        chance = root.RNG(chance);
        bool bounding = spawner.transform.position.z > 0 && spawner.transform.position.z <= 20
            && spawner.transform.position.x >= -15 && spawner.transform.position.x <= 15;

        if (root.roomCount > 0 && chance == 0 && bounding)
        {
            // reduce roomCount and increase roomID
            root.roomCount--;

            GameObject roomToPlace;
            if (largeRoom && spawner.transform.position.z > 5)
            {
                root.maxLargeRooms--;
                int rand = root.RNG(root.largeRooms.Count);

                if (debug)
                {
                    roomToPlace = root.largeDebugRoom;
                }
                else
                {
                    roomToPlace = root.largeRooms[rand];
                }
            }
            else
            {
                // set medium / small room here
                chance = root.RNG(root.roomChance);
                if (chance == 0)
                {
                    int rand = root.RNG(root.smallRooms.Count);
                    // set small room
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
                    // set medium room
                    if (debug)
                    {
                        roomToPlace = root.mediumDebugRoom;
                    }
                    else
                    {
                        roomToPlace = root.mediumRooms[rand];
                    }
                }
            }

            // final place room
            GameObject room = Instantiate(roomToPlace, Vector3.zero, new Quaternion(), spawner.transform);
            room.transform.localPosition = Vector3.zero; // reset local position due to annoying instantiate stuff

        }
        else
        {
            // place blank wall
            if (root.debug)
            {
                GameObject wall = Instantiate(root.debugWall, Vector3.zero, new Quaternion(), spawner.transform);
                wall.transform.localPosition = Vector3.zero;
            } else
            {
                int rand = root.RNG(root.Walls.Count);
                GameObject wall = Instantiate(root.Walls[rand], Vector3.zero, new Quaternion(), spawner.transform);
                wall.transform.localPosition = Vector3.zero;
            }
        }
    }

    private bool CheckEmpty(GameObject spawner, bool largeRoom = false)
    {
        GameObject[] blockers;
        blockers = GameObject.FindGameObjectsWithTag("spawned");

        // if spawner is trying to spawn a large room, check to see if the space next to it (where the large room will be occupying) is empty too
        if (largeRoom) 
        {
            Vector3 loc = spawner.transform.position;
            float rotation = spawner.transform.rotation.y;

            switch (rotation)
            {
                case 0:
                    loc.x -= 10;
                    break;
                case 90:
                    loc.z -= 10;
                    break;
                case 180:
                    loc.x += 10;
                    break;
                case 270:
                    loc.z += 10;
                    break;
            }

            // check distance from next spawn location for large room
            for (int i = 0; i < blockers.Length; i++)
            {
                if (Vector3.Distance(loc, blockers[i].transform.position) <= 1)
                {
                    Debug.Log("blocker " + i + " is intersecting with " + gameObject.name + spawner.name + ". blocker is " + Vector3.Distance(loc, blockers[i].transform.position) + " units away for large room");
                    return false;
                }
            }
        }

        // check distance from spawner and each blocker
        for (int i = 0; i < blockers.Length; i++)
        {
            if (Vector3.Distance(spawner.transform.position, blockers[i].transform.position) <= 1)
            {
                Debug.Log("blocker " + i + " is intersecting with " + gameObject.name + spawner.name + ". blocker is " + Vector3.Distance(spawner.transform.position, blockers[i].transform.position) + " units away");
                return false;
            }
        }

        return true;
    }

}
