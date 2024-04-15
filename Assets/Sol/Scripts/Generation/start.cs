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
    [SerializeField] private int maxRooms = 9;
    public int roomChance = 5;
    public int largeRoomChance = 2;
    [Space(13)]

    public float cubicRoomSize = 10;
    public Vector3 smallRoomSize = new Vector3(10, 0, 5);
    public Vector3 mediumRoomSize = new Vector3(10, 0, 10);
    public Vector3 largeRoomSize = new Vector3(10, 0, 20);

    [Space(13)]
    public genTypes genType;
    public int cubicLoop = 3;
    [HideInInspector] public bool[] roomCoords;
    [HideInInspector] public bool[] roomWalls;

    [Space(13)]

    // debug
    [Header("Debug")]
    public bool debug;
    public GameObject cubicDebugRoom;
    public GameObject smallDebugRoom;
    public GameObject mediumDebugRoom;
    public GameObject largeDebugRoom;
    public GameObject debugWall;
    public GameObject debugDoorWall;
    [Space(13)]

    // rooms
    [Header("Rooms")]
    public List<GameObject> smallRooms = new List<GameObject>();
    public List<GameObject> mediumRooms = new List<GameObject>();
    public List<GameObject> largeRooms = new List<GameObject>();
    public List<GameObject> Walls = new List<GameObject>();
    public List<GameObject> Doors = new List<GameObject>();

    // placed rooms for room generation
     public List<GameObject> placedRooms = new List<GameObject>();

    void OnEnable()
    {
        // destroy old rooms
        DestroyRooms();

        // initiate room generation settings
        if (roomCount == -1)
        {
            roomCount = RNG(1, maxRooms);
            maxLargeRooms = RNG(roomCount - 1);

            Debug.Log("Room generation:\n Room Count: " + roomCount
                + "\n Max Large Rooms: " + maxLargeRooms);
        }

        // begin room generation
        if (genType == genTypes.CUBIC) // cubic room generation
        {
            Debug.Log("roomgen: Starting Cubic Generation");

            roomCoords = new bool[cubicLoop * cubicLoop];
            roomWalls = new bool[roomCoords.Length * 4];
            CubicRoomGeneration();

            Debug.Log("roomgen: Cubic Generation Complete!");
        } 
        else if (genType == genTypes.DYNAMIC) // dynamic room generation
        {
            Debug.Log("roomgen: Starting Dynamic Generation");

            DynamicRoomGeneration(new Vector3(0, 0, largeRoomSize.x/2));

            Debug.Log("goofy ahhh looking rooms generated");
        }

        // end room generation
        roomCount = -1;
    }

    private void OnDisable()
    {
        DestroyRooms();
    }

    private void DynamicRoomGeneration(Vector3 offset = new Vector3()) // TODO possibly merge large and medium roomgen to allow for large rooms to spawn off of medium rooms??
    {
        int chance = RNG(2);
        GameObject firstRoom;
        if (chance == 0)
        {
            if (debug)
            {
                firstRoom = largeDebugRoom;
            } else
            {
                int sel = RNG(largeRooms.Count);
                firstRoom = largeRooms[sel];
            }
        } else if (chance == 1)
        {
            if (debug)
            {
                firstRoom = mediumDebugRoom;
            } else
            {
                int sel = RNG(mediumRooms.Count);
                firstRoom = mediumRooms[sel];
            }
        } else
        {
            if (debug)
            {
                firstRoom = smallDebugRoom;
            } else
            {
                int sel = RNG(smallRooms.Count);
                firstRoom = smallRooms[sel];
            }
        }
        firstRoom = Instantiate(firstRoom, transform.position + offset, Quaternion.identity, transform);
        firstRoom.name = "entry room";
    }

    // cubic room generation on a 3x3 grid
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
                    CheckRoom(roomCoords, i, cubicLoop);
                }
            }
        }

        // place rooms
        for (int i = roomCoords.Length - 1; i >= 0; i--)
        {
            PlaceRoom(roomCoords, i, cubicRoomSize, cubicLoop);
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
            GameObject room = Instantiate(cubicDebugRoom, coords, Quaternion.identity, transform);
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
            Debug.Log("rooms cleared!");
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

    // random number generator (so that i dont have to type UnityEngine.Random.Range() every. single. time.
    public int RNG(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }
    public int RNG(int max)
    {
        return UnityEngine.Random.Range(0, max);
    }
}
