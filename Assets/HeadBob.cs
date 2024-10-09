using UnityEngine;

public class HeadBob : MonoBehaviour
{
    public Transform cameraTransform;          // Reference to the camera transform
    public float bobSpeed = 10f;   
    private float runSpeed = 20f;
    private float walkSpeed = 10f;     
    private float crouchSpeed = 5f;     
    public float bobAmount = 0.05f;            // Amount of bobbing

    private float defaultYPos;                 // Default y-position of the camera
    private float timer = 0f;                  // Timer to track bobbing motion

    private Player player;                     // Reference to the Player script

    void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform; // Assign the main camera if not set
        }

        runSpeed = 2*bobSpeed;
        crouchSpeed = (1/2)*bobSpeed;

        // Store the default camera y-position
        defaultYPos = cameraTransform.localPosition.y;

        // Get reference to the player's Player script
        player = gameObject.transform.parent.GetComponent<Player>();
    }

    void Update()
    {
        // Check if the player is moving based on velocity
        if (player != null && player.velocity.magnitude > 0.1f) // Adjust threshold as needed
        {
            if(player.currentState == PlayerState.Running){
                bobSpeed = runSpeed;
            }
            else if(player.currentState == PlayerState.Crouching){
                bobSpeed = crouchSpeed;
            }
            else{
                bobSpeed = walkSpeed;
            }
            // Calculate head bobbing motion
            timer += Time.deltaTime * bobSpeed;
            float newYPos = defaultYPos + Mathf.Sin(timer) * bobAmount;

            // Apply the new y-position to the camera
            cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, newYPos, cameraTransform.localPosition.z);
        }
        else
        {
            // Reset the timer and set the camera to the default y-position when the player is not moving
            timer = 0f;
            cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, defaultYPos, cameraTransform.localPosition.z);
        }
    }
}
