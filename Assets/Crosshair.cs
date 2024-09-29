using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    Player player;
    Image image;
    [SerializeField] Sprite normalIcon;
    [SerializeField] Sprite interactIcon;
    [SerializeField] Sprite lockedIcon;
    [SerializeField] Sprite hideIcon;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        image = gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleCrosshairChange();
    }

    void HandleCrosshairChange(){
        if(player.isPaused){
            image.sprite = null;
        }
        else if(player.currentTarget == null){
           image.sprite = normalIcon; 
           image.rectTransform.sizeDelta = new Vector2(10f,10f);
        }
        else if(player.currentTarget.GetComponent<Item>() != null){
            image.sprite = interactIcon;
            image.rectTransform.sizeDelta = new Vector2(50f,50f);
        }
        else if(player.currentTarget.GetComponent<Door>() != null){
            if(player.currentTarget.GetComponent<Door>().isDoorLocked){
                image.sprite = lockedIcon;
                image.rectTransform.sizeDelta = new Vector2(50f,50f);
            }
            else{
                image.sprite = interactIcon;
                image.rectTransform.sizeDelta = new Vector2(50f,50f);
            }
        }
        else if(player.currentTarget.GetComponent<HidingPlace>() != null){
            image.sprite = hideIcon;
            image.rectTransform.sizeDelta = new Vector2(50f,50f);
        }
    }
}
