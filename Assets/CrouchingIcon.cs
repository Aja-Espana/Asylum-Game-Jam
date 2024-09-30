using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Ensure this is included for Image handling
using System;

public class CrouchingIcon : MonoBehaviour
{
    Player player;
    Image image;
    float targetAlpha;
    float lerpSpeed = 1f;
    Color currentColor;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        image = gameObject.GetComponent<Image>();
        currentColor = image.color; 
        currentColor.a = 0;        
        image.color = currentColor;
    }

    // Update is called once per frame
    void Update()
    {
        if(!player.isPaused){
            if (player.currentState == PlayerState.Crouching)
            {
                targetAlpha = 1f;
            }
            else
            {
                targetAlpha = 0f;
            }

            currentColor.a = Mathf.Lerp(currentColor.a, targetAlpha, Time.unscaledDeltaTime * lerpSpeed * 1/Math.Abs(currentColor.a - targetAlpha));
            image.color = currentColor;
        }
        else{
            currentColor.a = 0;
            image.color = currentColor;
        }

    }
}
