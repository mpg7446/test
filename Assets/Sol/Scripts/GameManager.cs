using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private bool debugStartGame = false;
    public bool playing = false;
    [Space(5)]
    public int gameTimer = 0;
    public TextMeshProUGUI timer;
    public int highScore;
    public TextMeshProUGUI highScoreText;
    public Texture2D img;

    //Izzy
    [Header("Arm")]
    public S_Arm armScript;

    void Start()
    {
        SetCamera(menuPos, menuRot);
        player.SetActive(false);
        playerManager = player.GetComponent<PlayerManager>();
        roomGeneration.SetActive(false);

        if (img == null)
        {
            // this should close the game / stop on runtime
        }
    }

    void FixedUpdate()
    {
        if (debugStartGame)
        {
            StartGame();
        }

        if (playing)
        {
            gameTimer--;
            timer.text = "Time: " + (gameTimer / 50 + 1) + "s!";

            //if (collectables.Length <= 0 || gameTimer <= 0)
            if (gameTimer <= 0)
            {
                EndGame();
            }
        }
    }

    // user input / keybinds
    private void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            EndGame();
        }
    }

    public void StartGame()
    {
        SetCamera(playingPos, playingRot);
        roomGeneration.SetActive(true);
        player.SetActive(true);
        playerManager.score = 0;

        gameTimer = 500;
        debugStartGame = false;
        playing = true;

        //Izzy
        int counter = 0;
        int armLayer = LayerMask.NameToLayer("ArmRayCast");
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Walls").Length; i++)
        {
            GameObject.FindGameObjectsWithTag("Walls")[i].layer = armLayer;
            counter++;
        }
        Debug.Log("GameWallCounter" + counter);

        armScript.Begin();

        Debug.Log("Game started");
    }

    public void EndGame()
    {
        if (highScore < playerManager.score)
        {
            highScore = playerManager.score;
            highScoreText.text = "High Score: " + highScore;
        }

        SetCamera(menuPos, menuRot);
        roomGeneration.SetActive(false);
        player.SetActive(false);

        playing = false;
        

        //Izzy
        armScript.End();

        Debug.Log("Game ended");

        // THIS IS FOR TESTING PURPOSES ONLY
        // PLEASE REMOVE THE FOLLOWING LINE FOR RELEASE
        //StartGame();

    }

    public void SettingsMenu()
    {
        SetCamera(settingsPos, settingsRot);
    }

    public void CharacterMenu()
    {
        SetCamera(custPos, custRot);
    }

    public void MainMenu()
    {
        SetCamera(menuPos, menuRot);
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
