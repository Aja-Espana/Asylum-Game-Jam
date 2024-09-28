using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    Player player;
    public bool canInteract;

    protected AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
       player = GameObject.Find("Player").GetComponent<Player>(); 
       audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Interact();
    }

    public void Interact()
    {
        if(canInteract && Input.GetKeyDown(KeyCode.E)){
            Debug.Log("item picked up");
            PickUp();
        }
    }

    public void PickUp()
    {
        player.AddItem(this);
        PlaySound();
    }

    public abstract void PlaySound();
}
