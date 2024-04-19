using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;
using System.Linq;

public class S_Arm : MonoBehaviour
{
    Vector3 pivotPoint;
    Vector3 lastpivotPoint;
    Vector3 secondtolastpivotpoint;
    public GameObject gameHand;
    // list 
    private List<Vector3> collisionPoints = new List<Vector3>();
    int rayTestCounter = 0;

    public GameObject player;

    int layerMask = 1 << 10;

    public bool liveGame = false;

    // setting up bone points (data)
    public List<Vector3> bonePoints = new List<Vector3>(30);
    public int boneAmount = 30;
    float totalDistance;
    float pointDistance;
    public Transform[] bones;

    [Header("Debug")]
    public bool debug;




    // Start is called before the first frame update
    void Start()
    {
        collisionPoints.Add(player.transform.position);

        
    }

    // Update is called once per frame
    void Update()
    {

        // making raycast line - debug
        Vector3 vectorLastToHand;
        Vector3 vectorSecondLastToHand;
        vectorLastToHand = gameHand.transform.position - lastpivotPoint;
        vectorSecondLastToHand = gameHand.transform.position - secondtolastpivotpoint;

        // Raycast detecting collision+

        Ray firstArmRay = new Ray(lastpivotPoint, vectorLastToHand);



        RaycastHit secondpivotHit;

        if (collisionPoints.Count > 1 ) 
        {
          

            // for second ray
            if (Physics.Raycast(secondtolastpivotpoint, vectorSecondLastToHand, out secondpivotHit, 100f,layerMask))
            {
                if (secondpivotHit.collider.tag == "Hand")
                {
                    UpdateRayPoints(2, pivotPoint);
                    Debug.DrawRay(secondtolastpivotpoint, vectorSecondLastToHand, Color.blue);
                }
                else
                {
                    Debug.DrawRay(secondtolastpivotpoint, vectorSecondLastToHand, Color.blue);
                }

            }
            else
            {
                Debug.DrawRay(secondtolastpivotpoint, vectorSecondLastToHand, Color.blue);
            }
        }

        for (int i = 0; i < collisionPoints.Count - 1; i++)
        {
            Debug.DrawLine(collisionPoints[i], collisionPoints[i+1]);
        }
        // Checking for if hit object is a wall first ray
        FirstRayCast();

        BonePointsSet();

    }

    void BonePointsSet()
    {
        totalDistance = 0;
        float totalStepDistance = 0;
        float remainder = 0;
        int counter = 0;

        Vector3 currentPoint;

        for (int i = 0; i < collisionPoints.Count; i++)
        {
            float dist;
            if (i < collisionPoints.Count - 1)
            {

                dist = Vector3.Distance(collisionPoints[i], collisionPoints[i + 1]);
                totalDistance += dist;
            }
            else
            {
                dist = Vector3.Distance(collisionPoints[i], gameHand.transform.position);
                totalDistance += dist;
            }
        }

        pointDistance = totalDistance / boneAmount;

        for (int i = 0; i < collisionPoints.Count; i++)
        {
            currentPoint = collisionPoints[i];


            Vector3 direction;
            float dist;
            if (i < collisionPoints.Count - 1)
            {
                direction = (collisionPoints[i + 1] - collisionPoints[i]).normalized;
                dist = Vector3.Distance(collisionPoints[i], collisionPoints[i + 1]);
            }
            else
            {
                direction = (gameHand.transform.position - collisionPoints[i]).normalized;
                dist = Vector3.Distance(collisionPoints[i], gameHand.transform.position);
            }
            
           
            


            for (int j = counter; j < boneAmount; j++)
            {
                if (j < boneAmount)
                {
                    if (dist > totalStepDistance + pointDistance - remainder)
                    {
                        if (j <= bonePoints.Count - 1)
                        {
                            bonePoints[j] = currentPoint + direction * (totalStepDistance + pointDistance - remainder);
                            counter++;

                        }
                        else
                        {
                            bonePoints.Add(currentPoint + direction * (totalStepDistance + pointDistance - remainder));
                            counter++;
                        }

                        totalStepDistance += pointDistance - remainder;
                        remainder = 0;

                        if (bonePoints.Count > boneAmount)
                        {
                            bonePoints.RemoveAt (boneAmount);
                        }
                    }
                    else { break; }
                }
                else
                {
                    break;
                }

               
            }

            totalStepDistance = 0;

        }
        counter = 0;
        BoneMove();
    }

    void BoneMove()
    {
        Vector3 rotation;
        for (int i = 0; i < bonePoints.Count; i++)
        {
            if (i != bonePoints.Count - 1)
            {
                rotation = (bonePoints[i + 1] - bonePoints[i]).normalized;

                bones[i].transform.position = bonePoints[i];
                bones[i].transform.up = (rotation);
            }
            else
            {
                bones[i].transform.position = gameHand.transform.position;
            }

            
        }

    }

    //gizmo
    private void OnDrawGizmos()
    {
        if (liveGame == true && debug == true )
        {

            int b = 0;
            Gizmos.color = Color.cyan;

            foreach (Vector3 x in bonePoints)
            {
                Gizmos.DrawWireCube(x, new Vector3(1, 1, 1));
                b++;
            }
        }

    }

    bool FirstRayCast()
    {
        RaycastHit lastpivotHit;

    Vector3 vectorLastToHand;
        vectorLastToHand = gameHand.transform.position - lastpivotPoint;
        rayTestCounter++;
        if (rayTestCounter >= 200)
        {
            rayTestCounter = 0;
            return false;
        }
        else
        {

            // Checking for if hit object is a wall first ray
            if (Physics.Raycast(lastpivotPoint, vectorLastToHand, out lastpivotHit, 1000f, layerMask))
            {
                if (lastpivotHit.collider.tag == "Walls")
                {
                    Vector3 d = (lastpivotHit.point - collisionPoints[^1]).normalized;
                    Vector3 n = lastpivotHit.normal;
                    Vector3 r = d - 2 * (Vector3.Dot(d, n) * n);
                    Vector3 jitter = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1) * 0.1f);
                    r += jitter;

                    // correct position of nextpoint so it can be solved
                    Vector3 correctedPoint = lastpivotHit.point + r * 0.4f;


                    UpdateRayPoints(1, correctedPoint);
                    Debug.DrawRay(lastpivotPoint, vectorLastToHand, Color.green);

                    FirstRayCast();
                    return false;
                }
                else if (lastpivotHit.collider.tag == "Hand")
                {
                    rayTestCounter = 0;
                    Debug.DrawRay(lastpivotPoint, vectorLastToHand, Color.red);
                    return true;
                    
                }

            }
            else
            {
                // should never happen 
                Debug.Log(("This should never happen"));
                return false;
            }

            return false;

        }

    }


    void UpdateRayPoints(int a, Vector3 b)
    {

        if (a == 1)
        {
            if (b != collisionPoints.Last())
            {
                collisionPoints.Add(b);
                Debug.Log(collisionPoints.Count);
            }
            
        }

        if (a == 2)
        {
            collisionPoints.RemoveAt(collisionPoints.Count - 1);
        }

        lastpivotPoint = collisionPoints.Last();
        Debug.Log(collisionPoints.Last());

        if (collisionPoints.Count > 1)
        {
            secondtolastpivotpoint = collisionPoints[^2];
        }
        
    }

    public void Begin()
    {
        collisionPoints.Clear();
        bonePoints.Clear();
        liveGame = true;
        collisionPoints.Add(player.transform.position);

    }
    public  void End()
    {
        liveGame = false;
        collisionPoints.Clear();
        bonePoints.Clear();
    }



}
