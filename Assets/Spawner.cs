using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] bool isOnEvent;
    [SerializeField] GameObject monster;
    [SerializeField] float time;
    [SerializeField] Player player;

    [SerializeField] AudioClip monsterSpawn;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawn(monster));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Spawn(GameObject monster)
    {
        if(isOnEvent){
            while(!player.hasAwokenMonster){
                yield return null;
            }
        }
        else{
            yield return new WaitForSeconds(time);
        }

        GameObject stretch = Instantiate(monster, gameObject.transform.position, Quaternion.identity);
        
        player.stretch = stretch.GetComponent<Stretch>();
        player.lookAtThis = stretch.transform.Find("QuickRigCharacter_Ctrl_Reference/QuickRigCharacter_Ctrl_Hips/QuickRigCharacter_Ctrl_Spine/QuickRigCharacter_Ctrl_Spine1/QuickRigCharacter_Ctrl_Spine2/QuickRigCharacter_Ctrl_Neck/QuickRigCharacter_Ctrl_Head").gameObject;

        

    }
}
