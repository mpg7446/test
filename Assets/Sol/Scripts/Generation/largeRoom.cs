using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UIElements;

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
        
        if (root.roomCount > 0)
        {
            foreach (GameObject spawner in spawners)
            {
                if (root.maxLargeRooms > 0)
                {
                    // generate large rooms
                    PlaceRoom(spawner, true, root.debug);
                } else
                {
                    // generate smaller rooms
                    PlaceRoom(spawner, false, root.debug);
                }
            }
        }

    }

    private void PlaceRoom(GameObject spawner, bool largeRoom = false, bool debug = false, int chance = 2) // TODO please make wall placements seperate to room count, move room count check into PlaceRoom function
    {
        chance = root.RNG(chance);
        bool bounding = spawner.transform.position.z > 0 && spawner.transform.position.z <= 20
            && spawner.transform.position.x >= -15 && spawner.transform.position.x <= 15;
        Debug.Log(spawner.name + " bounding: " + bounding);
        if (chance == 0 && bounding)
        {
            root.roomCount--;
            // place room here at game object locations

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
                // place medium / small room here
                chance = root.RNG(5);
                if (chance == 0)
                {
                    int rand = root.RNG(root.smallRooms.Count);
                    // place small room
                    if (debug)
                    {
                        roomToPlace = root.smallDebugRoom;
                    } else
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
                    } else
                    {
                        roomToPlace = root.mediumRooms[rand];
                    }
                }
            }

            GameObject room = Instantiate(roomToPlace, Vector3.zero, new Quaternion(), spawner.transform);
            room.transform.localPosition = Vector3.zero;

        } else
        {
            // place blank wall
            if(root.debug)
            {
                GameObject wall = Instantiate(root.debugWall, Vector3.zero, new Quaternion(), spawner.transform);
                wall.transform.localPosition = Vector3.zero;
            }
        }
        //Destroy(spawner);
    }
}
