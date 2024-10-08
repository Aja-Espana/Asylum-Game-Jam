using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum StretchState
{
    Idle,
    Stalking,
    Hunting,
    Chasing
}

public enum StalkDecision
{
    NotStalking,
    Follow,
    Peek,
    Feign,
    Hide
}

public class Stretch : Entity
{
    Vector3 targetedLocation;
    bool canSeePlayer;
    public StretchState currentState;

    Corner currentCorner;

    Player player;
    NavMeshAgent agent;

    Corner somePoint;

    public Animator anim;
    
    [SerializeField] AudioClip jumpscareSound;

    [SerializeField] string runningAnimation;
    [SerializeField] string jumpscareAnimation;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        anim = gameObject.GetComponent<Animator>();

        anim.Play(runningAnimation);
        //currentState = StretchState.Idle;

        /*
        stateMachine = gameObject.AddComponent<StateMachine>();
        stateMachine.SetState(new IdleState(stateMachine, this));  
        */
    }

    // Update is called once per frame
    void Update()
    {
        if(player.isDead == false && player.currentState != PlayerState.Hiding){
            agent.SetDestination(player.gameObject.transform.position);
        }
        else if(player.currentState == PlayerState.Hiding){
            agent.SetDestination(somePoint.gameObject.transform.position);
        }
        else if(player.isDead == true){
            agent.SetDestination(player.gameObject.transform.position - Vector3.forward * 2f);
        }
        
    }

    public void OnCollisionEnter(Collision col){
        if(col.gameObject.tag == "Player"){
            player.isDead = true;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
            anim.Play(jumpscareAnimation);
            Kill();
        }
    }

    public void Kill(){
        gameObject.transform.position = player.transform.position - Vector3.forward * 2f;
        agent.speed = 0;
        AudioSource.PlayClipAtPoint(jumpscareSound, transform.position);
    }

    public void AlertStretch(Vector3 presumedLocation)
    {
        targetedLocation = presumedLocation;

        if(aggro < 20){
            currentState = StretchState.Idle;
        }
        else if(aggro >= 20 && aggro < 80){
            if(aggro >= 60 && player.currentState == PlayerState.Hiding){
                currentState = StretchState.Hunting;
            }
            else{
                currentState = StretchState.Stalking;
            }
        }
        else if(aggro >= 80){
            currentState = StretchState.Chasing;
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

        if(nearestCorner != null){
            currentCorner = nearestCorner.GetComponent<Corner>();
            gameObject.transform.position = currentCorner.transform.position;
            currentCorner.isInhabited = true;  
        }

    }

    public void LeaveCurrentCorner()
    {
        currentCorner.isInhabited = false;
    }
}
