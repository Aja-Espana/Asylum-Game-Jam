using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    public CapsuleCollider col;
    public Camera camera;
    public Rigidbody rb;
    public PlayerState currentState = PlayerState.Idle;

    public bool isDead;
    public bool hasWon;
    public bool hasAwokenMonster;

    public Stretch stretch;

    public Light flashLight;

    private Vector3 previousPosition;
    private Vector3 currentPosition;
    public Vector3 velocity;

    public Vector3 lateralVelocity;

    public float crouchedHeight = 1f;
    public float standingHeight = 2f; 

    [SerializeField] public float sensitivity;
    [SerializeField] float crouchSpeed;
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] float lerpSpeed = 5f;

    [SerializeField] public GameObject lookAtThis;

    [SerializeField] public Canvas pauseMenu;
    [SerializeField] public Canvas continueScreen;
    [SerializeField] public Canvas winScreen;

    [SerializeField] AudioSource audioSource;

    [SerializeField] AudioClip walkingSound;
    [SerializeField] AudioClip runningSound;

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
        col = gameObject.GetComponent<CapsuleCollider>();
        camera = gameObject.transform.Find("Camera").GetComponent<Camera>();
        flashLight = camera.transform.Find("Flashlight").GetComponent<Light>();

        previousPosition = transform.position;

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
            if(stretch != null)
            {
                UpdateProximityValue();
            }
        }
        else{
            try{
                camera.transform.LookAt(lookAtThis.transform);
            }
            catch{
                
            }
            audioSource.Stop();
            
            flashLight.intensity = 2f;
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
        else{
            currentTarget = null;
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
            other.transform.parent.GetComponent<Door>().canInteract = false;
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
        RaycastHit[] rayhits = Physics.RaycastAll(ray, 2f);

        currentTarget = null;

        foreach(RaycastHit hit in rayhits){  
                      
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
        CalculateVelocity();

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

        CrouchHandler();
    }

    void CrouchHandler()
    {
        if(currentState == PlayerState.Crouching){
            col.height = Mathf.Lerp(col.height, crouchedHeight, Time.deltaTime * lerpSpeed);
        }
        else{
            col.height = Mathf.Lerp(col.height, standingHeight, Time.deltaTime * lerpSpeed);
        }
    }

    void MovementSpeed()
    {
        if(!audioSource.isPlaying){
            audioSource.Play();
        }
        
        // Smoothly interpolate between walkSpeed and runSpeed based on the player's state
        if (currentState == PlayerState.Running)
        {
            audioSource.clip = runningSound;
            
            movementSpeed = Mathf.Lerp(movementSpeed, runSpeed, lerpSpeed * Time.deltaTime);
        }
        else if (currentState == PlayerState.Walking)
        {
            audioSource.clip = walkingSound;

            movementSpeed = Mathf.Lerp(movementSpeed, walkSpeed, lerpSpeed * Time.deltaTime);
        }
        else if(currentState == PlayerState.Crouching)
        {
            audioSource.clip = null;
            movementSpeed = Mathf.Lerp(movementSpeed, crouchSpeed, lerpSpeed * Time.deltaTime);
        }
        else{
            audioSource.clip = null;
        }



        if (Input.GetKey(KeyCode.LeftShift) && velocity.magnitude > 1f)
        {
            currentState = PlayerState.Running;
        }
        else if(Input.GetKey(KeyCode.LeftControl)){
            currentState = PlayerState.Crouching;
        }
        else if (velocity.magnitude > 1f)
        {
            currentState = PlayerState.Walking;
        }
        else{
            currentState = PlayerState.Idle;
        }
    }

    void CalculateVelocity()
    {
        // Store the current position
        currentPosition = transform.position;

        // Calculate the velocity: (new position - old position) / Time.deltaTime
        velocity = (currentPosition - previousPosition) / Time.deltaTime;

        // Update the previous position to be the current position for the next frame
        previousPosition = currentPosition;

        // Optional: Print the velocity to the console
        //Debug.Log("Manual Velocity: " + velocity);
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
        if(isPaused || isDead || hasWon){
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
        if(Input.GetKeyDown(KeyCode.Escape) && isDead == false && hasWon == false){
            isPaused = !isPaused;
        }

        if(isPaused){
            pauseMenu.enabled = true;
            Time.timeScale = 0;
        }
        else{
            pauseMenu.enabled = false;
            Time.timeScale = 1;
        }
    }
}
