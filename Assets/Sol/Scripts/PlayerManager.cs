using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class PlayerManager : MonoBehaviour
{
    [Header("Hand Settings")]
    public GameObject hand;
    public float sensitivity = 75;
    private Vector3 handPos = Vector3.zero;
    [SerializeField] private Vector3 handStartPos = Vector3.zero;
    private Vector3 posUpdate = Vector3.zero;
    public GameObject holding;
    private Vector3 throwForce;
    private bool movementCooldown;
    [Space(13)]

    //Izzy
    [Header("Arm Settings")]
    public GameObject arm;
    public S_Arm armScript;
    public IzzyHand IzzyHandCollision;

    [Header("Debug")]
    public GameObject cam;
    public bool debug;
    [Space(13)]

    [Header("Scoring & Text Display")]
    public int score;
    public TextMeshProUGUI currentScore;

    SphereCollider col;

    private void Awake()
    {
        col = hand.GetComponent<SphereCollider>();
    }

    // set cursor and hand settings on game begin
    private void OnEnable()
    {
        EnableHand();
        handPos = hand.transform.localPosition;
        posUpdate = hand.transform.localPosition;
        holding = gameObject;
        movementCooldown = true;
        Invoke("MovementCooldown", 0.4f);

        //Izzy
        armScript.enabled = true;
    }
    // return cursor to regular state when game ends
    private void OnDisable()
    {

        //Izzy
        armScript.enabled = false;

        DisableHand();


    }

    private void Update()
    {
        //Izzy
        // move hand around
        Move();
    }

    void FixedUpdate()
    {
        // clear holding if object has been collected
        if (holding == null)
        {
            holding = gameObject;
        }

        //Izzy
        // move hand around
        //Move();

        // left click detection
        if (Input.GetMouseButton(0) && holding == gameObject)
        {
            Grab();
        }
        else if (Input.GetMouseButton(0) && holding != gameObject)
        {
            Rigidbody hrb = holding.GetComponent<Rigidbody>();
            throwForce = (handPos - holding.transform.position) * 50;
            hrb.useGravity = false;

            // disable hand collision while holding
            col.enabled = false;
        }
        else
        {
            if (holding != null && holding != gameObject)
            {
                Rigidbody hrb = holding.GetComponent<Rigidbody>();
                hrb.useGravity = true;

                hrb.AddForce(throwForce, ForceMode.Impulse);
                Debug.Log("threw object with force of " + throwForce);
            }
            holding = gameObject;

            // enable hand collisions when not holding
            Invoke("ThrowCooldown", 2.0f);
        }

        // update score display
        currentScore.text = "Score: " + score;
    }

    void ThrowCooldown()
    {
        col.enabled = true;
    }

    void MovementCooldown()
    {
        movementCooldown = false;
    }

    private Vector3 WallCollision(Vector3 pos1, Vector3 pos2)
    {
        col.enabled = false;

        RaycastHit hit;

        if ( Physics.Raycast( pos1, pos2 - pos1, out hit, Vector3.Distance(pos1, pos2) ) && hit.collider.CompareTag("Walls") )
        {
            col.enabled = true;
            return pos1;
        }

        col.enabled = true;
        return pos2;
    }

    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int X, int Y);

    private void EnableHand()
    {
        // cursor
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;

        SetCursorPos((Screen.width / 2) + Screen.mainWindowPosition.x, (Screen.height / 2) + Screen.mainWindowPosition.y);

        //hand object
        hand.transform.localPosition = handStartPos;
    }

    private void DisableHand()
    {
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;

        SetCursorPos((Screen.width / 2) + Screen.mainWindowPosition.x, (Screen.height / 2) + Screen.mainWindowPosition.y);
    }

    public void Grab()
    {

        //Izzy
        //col.enabled = false;
        IzzyHandCollision.Enable();


        RaycastHit hit;
        if (Physics.Raycast(HandPos(10), hand.transform.TransformDirection(Vector3.down), out hit, 15) && hit.collider.CompareTag("Collectable"))
        {
            holding = hit.collider.GameObject();
            holding.transform.position = hand.transform.position;
        }

        //Izzy
        //col.enabled = true;
        IzzyHandCollision.Disable();
        
    }

    public Vector3 HandPos(float offset = 0)
    {
        return new Vector3(handPos.x, handPos.y + offset, handPos.z);
    }

    public void Move()
    {
        // apply cursor movement to hand position
        if (!movementCooldown)
        {
            posUpdate.x += (Input.GetAxis("Mouse X") * sensitivity) * Time.deltaTime;
            posUpdate.z += (Input.GetAxis("Mouse Y") * sensitivity) * Time.deltaTime;

            handPos = WallCollision(handPos, posUpdate);
            posUpdate = handPos;
        }

        // apply hand motion
        if (holding != gameObject)
        {
            Rigidbody hrb = holding.GetComponent<Rigidbody>();

            // move holding object and hand
            hrb.MovePosition(handPos);
            hand.transform.position = handPos;

            // update handPos with new vector
            handPos.x = hrb.position.x;
            handPos.z = hrb.position.z;
        }
        else
        {
            hand.transform.position = handPos;
            handPos = hand.transform.position;
        }

        if (debug)
        {
            cam.transform.position = new Vector3(handPos.x, cam.transform.position.y, handPos.z);
        }
    }

    public void SetSensitivity(float  sensitivity)
    {
        this.sensitivity = sensitivity;
    }
}
