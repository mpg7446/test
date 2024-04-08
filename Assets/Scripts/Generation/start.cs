using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class start : MonoBehaviour
{
    // room stats
    public enum genTypes
    {
        CUBIC,
        DYNAMIC
    }

    [Header("Room Settings")]
    public int roomCount = -1;
    [HideInInspector] public int maxLargeRooms;
    public float roomSize = 10;

    [Space(13)]
    public genTypes genType;
    public int roomCoordsLength = 3;
    [HideInInspector] public bool[] roomCoords;
    [HideInInspector] public bool[] roomWalls;

    [Space(13)]

    // debug
    [Header("Debug")]
    public bool debug;
    public GameObject debugRoom;
    [Space(13)]

    // rooms
    [Header("Rooms")]
    public List<GameObject> rooms = new List<GameObject>();
    public List<GameObject> placedRooms = new List<GameObject>();
    void OnEnable()
    {
        // destroy old rooms
        DestroyRooms();

        // initiate room generation settings
        if (roomCount == -1)
        {
            roomCount = RNG(1, 9);
            maxLargeRooms = RNG(roomCount);

            Debug.Log("Room generation:\n Room Count: " + roomCount
                + "\n Max Large Rooms: " + maxLargeRooms);
        }

        // begin room generation
        if (genType == genTypes.CUBIC) // cubic room generation
        {
            roomCoords = new bool[roomCoordsLength * roomCoordsLength];
            roomWalls = new bool[roomCoords.Length * 4];
            CubicRoomGeneration();
        } 
        else if (genType == genTypes.DYNAMIC) // dynamic room generation
        {

        }

        // end room generation
        roomCount = -1;
    }

    private void OnDisable()
    {
        DestroyRooms();
    }

    private void CubicRoomGeneration()
    {
        // set randomly placed rooms
        for (int i = 0; i < roomCoords.Length; i++)
        {
            if (i == 1)
            {
                roomCoords[i] = true;
                Debug.Log("1: starting room");
                roomCount--;
            } else
            {
                SetRoom(roomCoords, i, 2);
            }
        }

        // remove floating rooms
        for (int p = 0; p < 3; p++)
        {
            for (int i = roomCoords.Length - 1; i > -0; i--)
            {
                if (i != 1)
                {
                    CheckRoom(roomCoords, i, roomCoordsLength);
                }
            }
        }

        // place rooms
        for (int i = roomCoords.Length - 1; i >= 0; i--)
        {
            PlaceRoom(roomCoords, i, roomSize, roomCoordsLength);
        }
    }

    // randomly choose rooms to place
    private void SetRoom(bool[] array, int index, int chance = 2)
    {
        if (roomCount > 0)
        {
            array[index] = RandomBool(chance);
            Debug.Log(index + ": " + array[index]);
        }
        else
        {
            array[index] = false;
            Debug.Log(index + ": value exceeded room count");
        }

        // lower room count
        if (array[index])
        {
            roomCount--;
        }
    }

    // check to see if the room is connected to another room
    private void CheckRoom(bool[] array, int index, int loop = 3)
    {
        // declare variables
        bool left = false;
        bool right = false;
        bool up = false;
        bool down = false;

        // check if index is true (placed room)
        if (array[index] == true)
        {
            // left
            try
            {
                if (array[index -1])
                {
                    left = true;
                }
            }
            catch { }

            // right
            try
            {
                if (array[index + 1])
                {
                    right = true;
                }
            }
            catch { }

            // up
            try
            {
                if (array[index - loop])
                {
                    up = true;
                }
            } catch { }

            // down
            try
            {
                if (array[index + loop])
                {
                    down = true;
                }
            }
            catch { }

            // change value if nothing happens
            if (!up && !down && !left && !right)
            {
                array[index] = false;
                Debug.Log(index + ": culled due to no connections");
            }
        }
    }

    // place rooms in unity space
    private void PlaceRoom(bool[] array, int index, float spacing, int loop = 3)
    {
        // convert from array to Vector3
        int z = index / loop;
        int x = index - (z * loop) - 1;

        Vector3 coords = new Vector3(x, 0, z) * spacing;
        coords.z += spacing / 2;

        // place based on array

        if (array[index])
        {
            GameObject room = Instantiate(debugRoom, coords, Quaternion.identity);
            room.name = "room " + index;

            placedRooms.Add(room);
            Debug.Log(index + ": Room Placed at X/Z: " + coords.x + "/" + coords.z);
        }
    }

    public void DestroyRooms()
    {
        if (placedRooms.Count != 0)
        {
            foreach (GameObject room in placedRooms)
            {
                Destroy(room);
            }
            placedRooms.Clear();
        }
    }

    private bool RandomBool(int range = 2)
    {
        int chance = RNG(range);

        if (chance == 0)
        {
            roomCount--;
            return true;
        }
        return false;
    }

    // random number generator (so that i dont have to type UnityEngine.Random.Range() every time
    private int RNG(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }
    private int RNG(int max)
    {
        return UnityEngine.Random.Range(0, max);
    }
    private float RNG(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }
    private float RNG(float max)
    {
        return UnityEngine.Random.Range(0, max);
    }
}