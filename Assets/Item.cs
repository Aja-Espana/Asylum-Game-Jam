using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    Player player;
    public bool canInteract;

    [SerializeField] public int code;

    [SerializeField] bool willAwakenMonster;

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
        
    }

    public void Interact()
    {
        if(Input.GetKeyDown(KeyCode.E)){
            Debug.Log(this.name+"has been picked up");
            PickUp();
        }
    }

    public void PickUp()
    {
        player.AddItem(this);
        player.currentTarget = null;
        PlaySound();

        if(willAwakenMonster){
            player.hasAwokenMonster = true;
        }
    }

    public abstract void PlaySound();
}
