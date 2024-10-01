using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualEffects : MonoBehaviour
{
    [SerializeField] public float brightness;
    [SerializeField] public float stress;
    [SerializeField] public Material fullscreenMaterial;
    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        fullscreenMaterial.SetFloat("_Stress", stress);
        fullscreenMaterial.SetFloat("_Brightness", brightness);
        
    }
}
