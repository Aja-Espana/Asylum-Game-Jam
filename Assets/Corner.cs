using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corner : MonoBehaviour
{
    public bool isInVision = false;
    public bool isInhabited = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col){
        if(col.tag == "Player"){
            isInVision = true;
        }
    }

    void OnTriggerExit(Collider col){
        if(col.tag == "Player"){
            isInVision = false;
        }
    }

}
