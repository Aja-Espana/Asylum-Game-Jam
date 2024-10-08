using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : Item
{
    [SerializeField] AudioClip keyJingle;


    public override void PlaySound(){
        AudioSource.PlayClipAtPoint(keyJingle, transform.position);
    }
}
