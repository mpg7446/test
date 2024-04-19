using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class IzzyHand : MonoBehaviour
{
    public bool HandCollision;
    public Collider handcol;

    //izzy
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "collectables" !& HandCollision)
        {
            Physics.IgnoreCollision(collision.collider,handcol);
        }
    }
    public void Enable()
    {
        HandCollision = true;
    }
    public void Disable()
    {
        HandCollision = false;
    }

}
