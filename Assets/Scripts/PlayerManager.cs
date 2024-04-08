using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class PlayerManager : MonoBehaviour
{
    public GameObject cam;
    public GameObject hand;
    public float sensitivity = 10;
    private Vector3 handPos = Vector3.zero;
    public GameObject holding;
    private Vector3 throwForce;

    public int score;
    Rigidbody rb;

    private void Awake()
    {
        rb = hand.GetComponent<Rigidbody>();
    }

    // set cursor and hand settings on game begin
    private void OnEnable()
    {
        SetCursor();
        handPos = hand.transform.position;
        holding = gameObject;
    }

    void Update()
    {
        // clear holding if object has been collected
        if (holding == null)
        {
            holding = gameObject;
        }

        // apply cursor movement to hand position
        handPos.x += Input.GetAxis("Mouse X") * sensitivity / 100;
        handPos.z += Input.GetAxis("Mouse Y") * sensitivity / 100;

        // restrict hand from going out of bounds
        //if (handPos.x > 15)
        //{
        //    handPos.x = 15;
        //}
        //if (handPos.x < -15)
        //{
        //    handPos.x = -15;
        //}
        //if (handPos.z > 20)
        //{
        //    handPos.z = 20;
        //}
        //if (handPos.z < -2)
        //{
        //    handPos.z = -2;
        //}

        // apply hand motion
        if (holding != gameObject)
        {
            holding.GetComponent<Rigidbody>().MovePosition(handPos);
            handPos = new Vector3(holding.transform.position.x, handPos.y, holding.transform.position.z);
            hand.transform.position = holding.transform.position;
        } else
        {
            Vector3 newPosition = hand.transform.position;
            newPosition.x += handPos.x;
            newPosition.z += handPos.z;
            rb.MovePosition(handPos);
            handPos = rb.position;
        }

        cam.transform.position = new Vector3(handPos.x, cam.transform.position.y, handPos.z);

        // left click detection
        if (Input.GetMouseButton(0) && holding == gameObject)
        {
            Grab();
        } else if (Input.GetMouseButton(0) && holding != gameObject)
        {
            // why is rigidbody so buggyyyyyy
            // please it just causes more and more problems
            throwForce = (handPos - holding.transform.position) * 50;
            //holding.GetComponent<Rigidbody>().MovePosition(handPos);
            holding.GetComponent<Rigidbody>().useGravity = false;

            // disable hand collision while holding
            hand.GetComponent<SphereCollider>().enabled = false;
        } else
        {
            if (holding != null && holding != gameObject)
            {
                holding.GetComponent<Rigidbody>().useGravity = true;

                holding.GetComponent<Rigidbody>().AddForce(throwForce, ForceMode.Impulse);
                Debug.Log("threw object with force of " + throwForce);
            }
            holding = gameObject;

            // enable hand collisions when not holding
            hand.GetComponent<SphereCollider>().enabled = true;
        }
    }

    public void SetCursor()
    {
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    public void Grab()
    {
        hand.GetComponent<SphereCollider>().enabled = false;

        RaycastHit hit;
        if (Physics.Raycast(HandPos(10), hand.transform.TransformDirection(Vector3.down), out hit, 15) && hit.collider.CompareTag("Collectable"))
        {
            holding = hit.collider.GameObject();
            holding.transform.position = hand.transform.position;
        }

        hand.GetComponent<SphereCollider>().enabled = true;
    }

    public Vector3 HandPos(float offset = 0)
    {
        return new Vector3(handPos.x, handPos.y + offset, handPos.z);
    }

    public void Move()
    {

    }
}
