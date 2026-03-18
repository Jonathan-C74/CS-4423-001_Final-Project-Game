using System.Collections;
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
    public LayerMask groundMask; // The mask associated with walkable objects
    public float gravityAccel; // How fast the player accelerates to the ground
    Vector3 gravityVector;
    CharacterController cc; // Helps move the player

    // Manages sounds the player makes
    [Header("Audio")]
    AudioSource audioSource; // Plays the audio
    public AudioClip jumpClip;

    // Manages pistol
    [Header("Pistol")]
    public Pistol pistol;
    bool isGrappling = false; // Used to prevent player from moving while grappling

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
        // Doesn't simulate gravity while grappling
        if(isGrappling)
        {
            return;
        }
        
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
        // Doesn't continue if the player is not moving or is grappling
        if(direction == Vector3.zero || isGrappling)
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

    // Fires pistol
    public void FirePistol()
    {
        pistol.Shoot();
    }

    // Moves player to shot target
    public void Grapple(GameObject enemy)
    {
        // Prevents player from grappling more than once
        if(isGrappling)
        {
            return;
        }

        StartCoroutine(GrappleRoutine(enemy)); // Starts coroutine for grapple
    }

    // Helper function for Grapple() to move player to the target position
    void GrappleTowards(Vector3 target)
    {
        Vector3 moveVector = target - transform.position;
        moveVector = moveVector.normalized;
        cc.Move(moveVector * movementSpeed * Time.deltaTime);
    }

    IEnumerator GrappleRoutine(GameObject enemy)
    {
        // Disables player from moving with keyboard and doubles speed
        isGrappling = true;
        movementSpeed *= 2;

        // While there is distance between the player and target...
        while(Vector3.Distance(transform.position, enemy.transform.position) > 1)
        {
            GrappleTowards(enemy.transform.position); // Move towards the target
            yield return null;
        }

        Destroy(enemy); // Destroy the target when you come in contact
        // Reenable keyboard movement and reset speed
        isGrappling = false;
        movementSpeed /= 2;

        yield return null;
    }
}
