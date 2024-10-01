using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingPlace : MonoBehaviour
{
    Player player;

    Vector3 playerOldPosition;
    [SerializeField] float playerRotation;

    public bool canInteract = false;
    public bool isOccupied;

    [SerializeField] int direction;

    [SerializeField] string hideAnimationName;
    [SerializeField] string unhideAnimationName;

    [SerializeField] AudioClip hideSound;
    [SerializeField] AudioClip unhideSound;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        canInteract = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(isOccupied){
            Unhide();
        }

        if(!isOccupied){
            canInteract = true;
        }
        else{
            canInteract = false;
            player.transform.position = gameObject.transform.position;
        }
            
        
        
    }

    public void Hide()
    {
        if(canInteract && Input.GetKeyDown(KeyCode.E)){
            canInteract = false;
            player.currentState = PlayerState.Hiding;
            Animator anim = player.GetComponent<Animator>();
            anim.Play(hideAnimationName, 0, 0.0f);

            AudioSource.PlayClipAtPoint(hideSound, transform.position);
            isOccupied = true;

            player.GetComponent<Rigidbody>().isKinematic = true;

            player.transform.rotation = Quaternion.Euler(0,direction,0);
        }
    }

    void Unhide()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            canInteract = false;
            player.currentState = PlayerState.Idle;
            Animator anim = player.GetComponent<Animator>();
            anim.Play(unhideAnimationName, 0, 0.0f);

            AudioSource.PlayClipAtPoint(unhideSound, transform.position);


            player.transform.position = gameObject.transform.position + Vector3.forward * 5 * direction;

            //player.GetComponent<Rigidbody>().isKinematic = false;
            Invoke("SetKinematicFalse", 0.01f);

            isOccupied = false;
        }
    }

    void SetKinematicFalse()
    {
        player.GetComponent<Rigidbody>().isKinematic = false;
    }
}
