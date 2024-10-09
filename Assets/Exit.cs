using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
    Player player;
    GameObject monster;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Player"){
            player = col.gameObject.GetComponent<Player>();
            player.winScreen.enabled = true;
            player.hasWon = true;

            monster = GameObject.Find("Greg(Clone)");

            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;

            Destroy(monster);

        }
    }
}
