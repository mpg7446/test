using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class largeRoom : MonoBehaviour
{
    private Vector3 roomSize;
    private start root;
    [SerializeField] private List<GameObject> spawners = new List<GameObject>();
    private void OnEnable()
    {
        Debug.Log("Placed Large Room, Script is runnig!");

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
            foreach (GameObject spawner in spawners)
            {
                if (root.maxLargeRooms > 0)
                {
                    // generate large rooms
                    PlaceRoom(spawner, true);
                } else
                {
                    // generate smaller rooms
                    PlaceRoom(spawner, false, 2);
                }
            }
        }

    }

    private void PlaceRoom(GameObject spawner, bool largeRoom = false, int chance = 5)
    {
        chance = root.RNG(chance);
        if (chance == 0)
        {
            root.roomCount--;
            // place room here at game object locations
            if (largeRoom)
            {
                root.maxLargeRooms--;
                int rand = root.RNG(root.largeRooms.Count);
                Vector3 dir = spawner.transform.position - transform.position;
                Vector3 pos = spawner.transform.position;

                if (dir.x > 0)
                {
                    pos.x += root.largeRoomSize.x / 2;
                }
                else
                {
                    pos.x -= root.largeRoomSize.x / 2;
                }
                if (dir.z > 0)
                {
                    pos.z += root.largeRoomSize.z / 2;
                }
                else
                {
                    pos.z -= root.largeRoomSize.z / 2;
                }

                Instantiate(root.largeRooms[rand], pos, Quaternion.identity, transform);
            }
            else
            {
                // place medium / small room here
                chance = root.RNG(5);
                if (chance == 0)
                {
                    // place small room
                } else
                {
                    // place medium room
                }
            }

            // place wall with door
            if(root.debug)
            {
                Instantiate(root.debugDoorWall, spawner.transform, transform);
            }

        } else
        {
            // place blank wall
            if(root.debug)
            {
                Instantiate(root.debugWall, spawner.transform, transform);
            }
        }
    }
}
