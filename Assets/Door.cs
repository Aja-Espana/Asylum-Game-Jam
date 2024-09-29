using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    Player player;

    [SerializeField] string openAnimationName;
    [SerializeField] string closeAnimationName;

    GameObject handle;
    Animator doorAnim;

    [SerializeField] AudioClip openSound;
    [SerializeField] AudioClip closeSound;
    [SerializeField] AudioClip unlockSound;

    //[SerializeField] string doorName;
    [SerializeField] int doorCode;

    public bool canInteract = false;

    bool isDoorOpen = false;
    [SerializeField] public bool isDoorLocked = true;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        handle = gameObject.transform.Find("Handle").gameObject;
        doorAnim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Interact();
    }

    void Interact()
    {
        if(canInteract && Input.GetKeyDown(KeyCode.E)){
            if(!isDoorOpen){
                OpenDoor();
            }
            else{
                CloseDoor();
            }
        }
    }

    void OpenDoor()
    {
        if(isDoorLocked){
            if(player.SearchInventoryForItemCode(doorCode) != null){
                UseKey();
                Debug.Log("Opening door");
                doorAnim.Play(openAnimationName, 0, 0.0f);
                AudioSource.PlayClipAtPoint(unlockSound, transform.position);
                AudioSource.PlayClipAtPoint(openSound, transform.position);
                isDoorOpen = true;
            }
            else{
                DisplayLockScreen();
            }
        }
        else{
            Debug.Log("Opening door");
            doorAnim.Play(openAnimationName, 0, 0.0f);
            AudioSource.PlayClipAtPoint(openSound, transform.position);
            isDoorOpen = true;
        }
    }

    void CloseDoor()
    {
        if(canInteract && Input.GetKeyDown(KeyCode.E)){
            Debug.Log("Closing door"); 
            doorAnim.Play(closeAnimationName, 0, 0.0f);
            AudioSource.PlayClipAtPoint(closeSound, transform.position);
            isDoorOpen = false;
        }
    }

    void UseKey(){
        Debug.Log("Found key!");
        isDoorLocked = false;
        Item item = player.SearchInventoryForItemCode(doorCode);
        player.RemoveItem(item);
    }

    void DisplayLockScreen(){
        Debug.Log("This door is locked");
    }
}
