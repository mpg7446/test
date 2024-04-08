using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Camera Positions
    [Header("Menu Camera Position/Rotation")]
    public Vector3 menuPos;
    public Vector3 menuRot;

    [Header("Settings Camera Position/Rotation")]
    public Vector3 settingsPos;
    public Vector3 settingsRot;

    [Header("Customizing Player Camera Position/Rotation")]
    public Vector3 custPos;
    public Vector3 custRot;

    [Header("During Gameplay Camera Position/Rotation")]
    public Vector3 playingPos;
    public Vector3 playingRot;

    [Space(13)]

    [Header("Game Obects")]
    //public GameObject menuCursor;
    public GameObject player;
    private PlayerManager playerManager;
    public GameObject roomGeneration;

    [Space(13)]

    [Header("Game Stats")]
    public bool startGame = false;
    public bool playing = false;
    [Space(5)]
    public int gameTimer = 0;
    public int highScore;

    void Start()
    {
        SetCamera(menuPos, menuRot);
        player.SetActive(false);
        playerManager = player.GetComponent<PlayerManager>();
        roomGeneration.SetActive(false);
    }

    void FixedUpdate()
    {
        if (startGame)
        {
            StartGame();
        }

        if (playing)
        {
            gameTimer--;
            GameObject[] collectables = GameObject.FindGameObjectsWithTag("Collectable");

            //if (collectables.Length <= 0 || gameTimer <= 0)
            if (gameTimer <= 0)
            {
                EndGame();
            }
        }
    }

    public void StartGame()
    {
        SetCamera(playingPos, playingRot);
        roomGeneration.SetActive(true);
        player.SetActive(true);
        playerManager.score = 0;

        gameTimer = 600000;
        startGame = false;
        playing = true;

        Debug.Log("Game started");
    }

    public void EndGame()
    {
        if (highScore < playerManager.score)
        {
            highScore = playerManager.score;
        }

        SetCamera(menuPos, menuRot);
        roomGeneration.SetActive(false);
        player.SetActive(false);

        playing = false;

        Debug.Log("Game ended");

        // THIS IS FOR TESTING PURPOSES ONLY
        // PLEASE REMOVE THE FOLLOWING LINE FOR RELEASE
        //StartGame();
    }

    public void SetCamera(Vector3 pos, Vector3 rot)
    {
        gameObject.transform.position = pos;
        gameObject.transform.rotation = Quaternion.Euler(rot);
    }

    public void SetCamera(Vector3 pos, Quaternion rot)
    {
        gameObject.transform.position = pos;
        gameObject.transform.rotation = rot;
    }
}
