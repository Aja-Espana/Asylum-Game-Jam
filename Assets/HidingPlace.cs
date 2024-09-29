using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingPlace : MonoBehaviour
{
    Player player;

    public bool canInteract = false;
    public bool isOccupied;

    [SerializeField] string hideAnimationName;
    [SerializeField] string unhideAnimationName;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        Unhide();
        Hide();
    }

    void Hide()
    {
        if(canInteract && Input.GetKeyDown(KeyCode.E)){
            canInteract = false;
            player.currentState = PlayerState.Hiding;
            Animator anim = player.GetComponent<Animator>();
            anim.Play(hideAnimationName, 0, 0.0f);

            isOccupied = true;
        }
    }

    void Unhide()
    {
        if(isOccupied && Input.GetKeyDown(KeyCode.Space)){
            canInteract = true;
            player.currentState = PlayerState.Idle;
            Animator anim = player.GetComponent<Animator>();
            anim.Play(unhideAnimationName, 0, 0.0f);

            isOccupied = false;
        }
    }
}
