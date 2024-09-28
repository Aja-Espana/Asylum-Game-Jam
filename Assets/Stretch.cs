using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StretchState
{
    Idle,
    Patrolling,
    Stalking,
    Sprinting
}

public enum StalkDecision
{
    NotStalking,
    Follow,
    Peek,
    Feign,
    Hide
}

public class Stretch : MonoBehaviour
{
    int aggro = 0;

    Vector3 targetedLocation;
    bool canSeePlayer;
    StretchState currentState;

    Corner currentCorner;

    // Start is called before the first frame update
    void Start()
    {
        currentState = StretchState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AlertStretch(Vector3 presumedLocation)
    {
        targetedLocation = presumedLocation;

        if(currentState == StretchState.Patrolling){
            aggro = 10;
            currentState = StretchState.Stalking;
        }


        
    }

    public void GoToNearestCorner(Vector3 location){
        GameObject[] corners = GameObject.FindGameObjectsWithTag("Corner");

        if (corners.Length == 0)
        {
            Debug.LogWarning("No objects named 'Corner' found in the scene.");
        }
        GameObject nearestCorner = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject corner in corners)
        {
            float distance = Vector3.Distance(location, corner.transform.position);

            // Update the nearest corner if this one is closer
            if (distance < shortestDistance && corner.GetComponent<Corner>().isInVision == false)
            {
                shortestDistance = distance;
                nearestCorner = corner;
            }
        }

        currentCorner = nearestCorner.GetComponent<Corner>();
        gameObject.transform.position = currentCorner.transform.position;
        currentCorner.isInhabited = true;  
    }

    public void LeaveCurrentCorner()
    {
        currentCorner.isInhabited = false;
    }
}
