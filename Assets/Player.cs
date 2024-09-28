using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Idle,
    Walking,
    Running,
    Hiding,
    Paused
}

public class Player : MonoBehaviour
{
    public Camera camera;
    public Rigidbody rb;
    public PlayerState currentState = PlayerState.Idle;

    [SerializeField] public float sensitivity;
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] float lerpSpeed = 5f;

    float movementSpeed;
    
    //Camera
    private float verticalRotation = 0f;

    //Inventory
    public List<Item> inventory = new List<Item>();

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        camera = gameObject.transform.Find("Camera").GetComponent<Camera>();

        movementSpeed = walkSpeed;
    }

    void Update()
    {
        LookMovement();
        MovementSpeed();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        Movement();
        HandleCursor();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            other.GetComponent<Item>().canInteract = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            other.GetComponent<Item>().canInteract = false;
        }
    }

    void Movement()
    {
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

        // Switch between states based on input
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            currentState = PlayerState.Running;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            currentState = PlayerState.Walking;
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
        if(currentState == (PlayerState.Paused)){
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
}
