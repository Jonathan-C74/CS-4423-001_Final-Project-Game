using UnityEngine;

public class Player : MonoBehaviour
{
    // Manages general player movement
    [Header("Stats")]
    public float movementSpeed = 5;
    public float rotateSpeed = 10;
    public float jumpPower = 10f;

    // Manages gravity of the player
    [Header("Gravity")]
    public Transform groundCheck; // Position below the player to check if on ground
    public LayerMask groundMask; // The mask associated walkable objects
    public float gravityAccel; // How fast the player accelerates to the ground
    Vector3 gravityVector;
    CharacterController cc;

    // Manages sounds the player makes
    [Header("Audio")]
    AudioSource audioSource; // Plays the audio
    public AudioClip jumpClip;

    // Accesses all the components of the player
    void Awake()
    {
        cc =  GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Set the gravity of the player
        gravityVector = new Vector3(0, -2, 0);
    }

    // Update is called once per frame
    void Update()
    {
        SimulateGravity();
    }

    // Checks if the player is on the ground
    public bool OnGround()
    {
        // Creates a collider below the player and checks if it connects with a walkable object
        Collider[] colliders = Physics.OverlapSphere(groundCheck.position, 0.25f, groundMask);
        if(colliders.Length > 0)
        {
            return true;
        }

        return false;
    }

    // Simulates gravity
    public void SimulateGravity()
    {
        // Resets the player's acceleration if they are on the ground
        if(OnGround() && gravityVector.y <= 0)
        {
            gravityVector.y = -2;
        }

        gravityVector.y += gravityAccel * Time.deltaTime; // Accelerates the player the longer they are in the air
        cc.Move(gravityVector * Time.deltaTime); // Moves the player down
    }

    // Moves the player in a direction
    public void Move(Vector3 direction)
    {
        // Doesn't continue if the player is not moving
        if(direction == Vector3.zero)
        {
            return;
        }

        // Normalizes the vector so that the rotation of the camera doesn't affect the vector
        direction = direction.normalized;
        cc.Move(direction * movementSpeed * Time.deltaTime);
    }

    // Allows the player to jump
    public void Jump()
    {
        // You can't jump if you're off the ground
        if(!OnGround())
        {
            return;
        }
        audioSource.PlayOneShot(jumpClip);
        gravityVector = new Vector3(0, jumpPower, 0);
    }
}
