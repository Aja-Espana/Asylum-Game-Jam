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
    
    [SerializeField] AudioClip monsterSpawn;
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

        AudioSource.PlayClipAtPoint(monsterSpawn, transform.position);
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
            gameObject.transform.position = player.transform.position + Vector3.forward * 1.4f - Vector3.up * 0.2f;
            gameObject.transform.LookAt(player.transform);
            //agent.SetDestination(player.gameObject.transform.position - Vector3.forward * 2f);
        }
        
    }

    public void OnCollisionEnter(Collision col){
        if(col.gameObject.tag == "Player" && player.isDead == false){
            player.isDead = true;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
            //GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            anim.Play(jumpscareAnimation);
            Kill();
        }
        if(col.gameObject.tag == "Door" && col.transform.parent.GetComponent<Door>() != null){
            if(!col.transform.parent.GetComponent<Door>().isDoorLocked){
                col.transform.parent.GetComponent<Door>().OpenDoor();
            }
        }
    }

    public void Kill(){
        gameObject.transform.position = player.transform.position - Vector3.forward * 2f;
        agent.speed = 0f;
        AudioSource.PlayClipAtPoint(jumpscareSound, transform.position);
    }

    public void FlashContinueScreen(){
        player.GetComponent<VisualEffects>().stress = 0f;
        player.continueScreen.enabled = true;
        player.continueScreen.GetComponent<AudioSource>().enabled = true;

        Invoke("DestroySelf",1.0f);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
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
