using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Idle,
    Walking,
    Crouching,
    Running,
    Hiding,
}

public class Player : MonoBehaviour
{
    public Camera camera;
    public Rigidbody rb;
    public PlayerState currentState = PlayerState.Idle;

    public bool isDead;
    public bool hasAwokenMonster;

    public Stretch stretch;

    public Light flashLight;

    [SerializeField] public float sensitivity;
    [SerializeField] float crouchSpeed;
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] float lerpSpeed = 5f;

    [SerializeField] public GameObject lookAtThis;

    [SerializeField] public Canvas continueScreen;

    float movementSpeed;
    float noiseAmplifier;

    public GameObject currentTarget;

    public bool isPaused;
    
    //Camera
    private float verticalRotation = 0f;

    //Inventory
    public List<Item> inventory = new List<Item>();

    // Start is called before the first frame update
    void Start()
    {
        try{
            stretch = GameObject.Find("Stretch").GetComponent<Stretch>();
        }
        catch{

        }
        
        rb = gameObject.GetComponent<Rigidbody>();
        camera = gameObject.transform.Find("Camera").GetComponent<Camera>();
        flashLight = camera.transform.Find("Flashlight").GetComponent<Light>();

        continueScreen.enabled = false;
        continueScreen.GetComponent<AudioSource>().enabled = false;

        movementSpeed = walkSpeed;
    }

    void Update()
    {
        if(!isPaused && !isDead){
            
    
            if(currentState != PlayerState.Hiding){
                MovementSpeed();
                InteractionRay();
                InteractionHandler();
            }
            
        }


        PauseHandler();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        HandleCursor();

        if(!isDead){
            if(!isPaused){
                Movement();
                LookMovement();
            }
        }
        else{
            try{
                camera.transform.LookAt(lookAtThis.transform);
            }
            catch{
                
            }
            
            flashLight.intensity = 2f;
        }

        if(stretch != null)
        {
            UpdateProximityValue();
        }
        
        
    }

    void UpdateProximityValue()
    {
        // Calculate the distance to the target
        float distance = Vector3.Distance(transform.position, stretch.transform.position);

        // Clamp distance to be between 0 and maxDistance
        distance = Mathf.Clamp(distance, 0, 25f);

        // Calculate the value based on the distance (0 at maxDistance, 10 at 0)
        float value = Mathf.Lerp(7.5f, 0, distance / 25f);

        GetComponent<VisualEffects>().stress = value;

        // Optionally, you can print or use the value in your logic
        //Debug.Log("Proximity Value: " + value);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            other.GetComponent<Item>().canInteract = true;
            currentTarget = other.gameObject;
        }
        else if (other.CompareTag("Door"))
        {
            other.transform.parent.GetComponent<Door>().canInteract = true; 
            currentTarget = other.gameObject;
        }
        else if (other.CompareTag("HidingPlace"))
        {
            other.GetComponent<HidingPlace>().canInteract = true;
            currentTarget = other.gameObject;
        }

        
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            other.GetComponent<Item>().canInteract = false;
            currentTarget = null;
        }
        else if (other.CompareTag("Door"))
        {
            other.GetComponent<Door>().canInteract = false;
            currentTarget = null;
        }
        else if (other.CompareTag("HidingPlace"))
        {
            other.GetComponent<HidingPlace>().canInteract = false;
            currentTarget = null;
        }
    }
    void InteractionRay()
    {
        Vector3 rayOrigin = transform.position;
        
        Vector3 rayDirection = camera.transform.forward;

        Ray ray = new Ray(rayOrigin, rayDirection);
        RaycastHit[] hits = Physics.RaycastAll(ray, 2f);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Item"))
            {   
                currentTarget = hit.collider.gameObject;
                break;
            }
            else if (hit.collider.CompareTag("Door"))
            {
                currentTarget = hit.collider.gameObject;
                break;
            }
            else if (hit.collider.CompareTag("HidingPlace"))
            {
                currentTarget = hit.collider.gameObject;
                break;
            }
            currentTarget = null;
        }

        Debug.DrawRay(ray.origin, ray.direction * 2f, Color.green);
    }

    void InteractionHandler()
    {
        if(currentTarget != null){
            if(currentTarget.CompareTag("Item")){
                currentTarget.GetComponent<Item>().Interact();
            }
            else if(currentTarget.CompareTag("Door")){
                currentTarget.transform.parent.GetComponent<Door>().Interact();
            }
            else if(currentTarget.CompareTag("HidingPlace")){
                currentTarget.GetComponent<HidingPlace>().Hide();
            }
        }
    }

    void Movement()
    {
        if(currentState != PlayerState.Hiding){
            if(Input.GetKey(KeyCode.A)) {
                rb.position += -transform.right * Time.deltaTime * movementSpeed;
            }
            else if(Input.GetKey(KeyCode.D)) {
                rb.position += transform.right * Time.deltaTime * movementSpeed;
            }

            if(Input.GetKey(KeyCode.W)) {
                rb.position += transform.forward * Time.deltaTime * movementSpeed;
            }
            else if(Input.GetKey(KeyCode.S)) {
                rb.position += -transform.forward * Time.deltaTime * movementSpeed;
            }
        }
    }

    void MovementSpeed()
    {
        // Smoothly interpolate between walkSpeed and runSpeed based on the player's state
        if (currentState == PlayerState.Running)
        {
            movementSpeed = Mathf.Lerp(movementSpeed, runSpeed, lerpSpeed * Time.deltaTime);
        }
        else if (currentState == PlayerState.Walking)
        {
            movementSpeed = Mathf.Lerp(movementSpeed, walkSpeed, lerpSpeed * Time.deltaTime);
        }
        else if(currentState == PlayerState.Crouching)
        {
            movementSpeed = Mathf.Lerp(movementSpeed, crouchSpeed, lerpSpeed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.LeftControl)){
            currentState = PlayerState.Crouching;
        }
        if (Input.GetKeyUp(KeyCode.LeftControl)){
            currentState = PlayerState.Walking;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            currentState = PlayerState.Running;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            currentState = PlayerState.Walking;
        }
    }

    void NoiseHandler()
    {
        if(currentState == PlayerState.Running){
            noiseAmplifier = 2;
        }
        else if(currentState == PlayerState.Walking){
            noiseAmplifier = 1;
        }
        else{
            noiseAmplifier = 0;
        }
    }

    void LookMovement()
    {  
        float h = sensitivity * Input.GetAxis("Mouse X");
        float v = sensitivity * -Input.GetAxis("Mouse Y");

        verticalRotation += v;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        transform.Rotate(0, h, 0);
        camera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    void HandleCursor()
    {
        if(isPaused || isDead){
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else{
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    //Inventory    

    public void AddItem(Item item)
    {
        inventory.Add(item);

        item.gameObject.SetActive(false);
        item.GetComponent<Collider>().enabled = false;
        item.transform.position = transform.position;
    }

    public void RemoveItem(Item item)
    {
        inventory.Remove(item);
        Destroy(item.gameObject);
    }

    public Item SearchInventoryForItemCode(int code)
    {
        foreach(Item item in inventory){
            if(item.code == code){
                return item;
            }
        }
        return null;
    }

    //System

    public void PauseHandler()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && isDead == false){
            isPaused = !isPaused;
        }

        if(isPaused){
            Time.timeScale = 0;
        }
        else{
            Time.timeScale = 1;
        }
    }
}
