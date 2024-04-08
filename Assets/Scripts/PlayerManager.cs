using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class PlayerManager : MonoBehaviour
{
    public GameObject hand;
    public float sensitivity = 10;
    private Vector3 handPos = Vector3.zero;
    private Vector3 posUpdate = Vector3.zero;
    public GameObject holding;
    private Vector3 throwForce;

    public GameObject cam;
    public bool debug;

    public int score;
    Rigidbody rb;
    SphereCollider col;

    private void Awake()
    {
        rb = hand.GetComponent<Rigidbody>();
        col = hand.GetComponent<SphereCollider>();
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
        Rigidbody hrb = holding.GetComponent<Rigidbody>();

        // clear holding if object has been collected
        if (holding == null)
        {
            holding = gameObject;
        }

        // apply cursor movement to hand position
        posUpdate.x += Input.GetAxis("Mouse X") * sensitivity / 100;
        posUpdate.z += Input.GetAxis("Mouse Y") * sensitivity / 100;

        handPos = WallCollision(handPos, posUpdate);

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
            // move holding object and hand
            hrb.MovePosition(handPos);
            rb.MovePosition(handPos);

            // update handPos with new vector
            handPos.x = hrb.position.x;
            handPos.z = hrb.position.z;
        } else
        {
            rb.MovePosition(handPos);
            handPos = rb.position;
        }

        if (debug)
        {
            cam.transform.position = new Vector3(handPos.x, cam.transform.position.y, handPos.z);
        }

        // left click detection
        if (Input.GetMouseButton(0) && holding == gameObject)
        {
            Grab();
        }
        else if (Input.GetMouseButton(0) && holding != gameObject)
        {
            // why is rigidbody so buggyyyyyy
            // please it just causes more and more problems
            throwForce = (handPos - holding.transform.position) * 50;
            hrb.useGravity = false;

            // disable hand collision while holding
            col.enabled = false;
        }
        else
        {
            if (holding != null && holding != gameObject)
            {
                hrb.useGravity = true;

                hrb.AddForce(throwForce, ForceMode.Impulse);
                Debug.Log("threw object with force of " + throwForce);
            }
            holding = gameObject;

            // enable hand collisions when not holding
            Invoke("ThrowCooldown", 1.0f);
        }
    }

    void ThrowCooldown()
    {
        col.enabled = true;
    }

    private Vector3 WallCollision(Vector3 pos1, Vector3 pos2)
    {
        col.enabled = false;

        RaycastHit hit;

        if ( Physics.Raycast( pos1, pos2 - pos1, out hit, Vector3.Distance(pos1, pos2) ) && hit.collider.CompareTag("Walls") )
        {
            col.enabled = true;
            Debug.Log("hand collided with wall at position " + hit.point + " with distance of " + Vector3.Distance(pos1, pos2));
            return hit.point;
        }

        col.enabled = true;
        return pos2;
    }

    public void SetCursor()
    {
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    public void Grab()
    {
        col.enabled = false;

        RaycastHit hit;
        if (Physics.Raycast(HandPos(10), hand.transform.TransformDirection(Vector3.down), out hit, 15) && hit.collider.CompareTag("Collectable"))
        {
            holding = hit.collider.GameObject();
            holding.transform.position = hand.transform.position;
        }

        col.enabled = true;
    }

    public Vector3 HandPos(float offset = 0)
    {
        return new Vector3(handPos.x, handPos.y + offset, handPos.z);
    }

    public void Move()
    {

    }
}
