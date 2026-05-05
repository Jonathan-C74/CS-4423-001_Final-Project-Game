using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{   
    // Manages general player movement
    [Header("Stats")]
    public float movementSpeed = 10;
    public float rotateSpeed = 10;
    public float jumpPower = 10f;
    // How long you can hover / how many times you can dash before needing to rest
    public float maxWingMeter = 3;
    float curWingMeter;
    // Player model
    public GameObject playerModel;

    // Manages gravity of the player
    [Header("Gravity")]
    public Transform groundCheck; // Position below the player to check if on ground
    public LayerMask groundMask; // The mask associated with walkable objects
    public float gravityAccel = -19.8f; // How fast the player accelerates to the ground
    public float maxGravityAccel = -19.8f; // The fastest the player is allowed to fall
    public float maxHoverAccel = -3; // The fastest the player is allowed to fall while hovering
    Vector3 gravityVector;
    CharacterController cc; // Helps move the player

    // Manages sounds the player makes
    [Header("Audio")]
    AudioSource audioSource; // Plays the audio
    public AudioClip jumpClip;
    public AudioClip shootClip;
    public AudioClip dashClip;
    public AudioClip grappleClip;

    // Manages pistol
    [Header("Pistol")]
    public Pistol pistol;

    // Booleans for movement options
    bool isGrappling = false; // Used to prevent player from moving while grappling
    bool isDashing = false; // Used to prevent player from moving while dashing

    // Accesses all the components of the player
    void Awake()
    {
        cc =  GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Set the gravity of the player and current wing meter
        gravityVector = new Vector3(0, -2, 0);
        curWingMeter = maxWingMeter;
    }

    // Update is called once per frame
    void Update()
    {
        SimulateGravity();

        // Sets the wing meter exactly to 0 or maxWingMeter
        if(curWingMeter < 0)
        {
            curWingMeter = 0;
        }
        else if(curWingMeter > maxWingMeter)
        {
            curWingMeter = maxWingMeter;
        }

        // Refreshes the wing meter while on the ground and less than the maximum wing meter
        if(OnGround() && curWingMeter < maxWingMeter)
        {
            curWingMeter += 2 * Time.deltaTime;
        }
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
        // Doesn't simulate gravity while grappling or dashing
        if(isGrappling || isDashing)
        {
            return;
        }
        
        // Resets the player's acceleration if they are on the ground
        if(OnGround() && gravityVector.y <= 0)
        {
            gravityVector.y = -2;
        }

        gravityVector.y += gravityAccel * Time.deltaTime; // Accelerates the player the longer they are in the air
        
        // Prevents the player from falling past max acceleration
        if(gravityVector.y <= maxGravityAccel)
        {
            gravityVector.y = maxGravityAccel;
        }

        cc.Move(gravityVector * Time.deltaTime); // Moves the player down over time
    }

    // Moves the player in a direction
    public void Move(Vector3 direction)
    {
        // Doesn't continue if the player is not moving, is grappling, or is dashing
        if(direction == Vector3.zero || isGrappling || isDashing)
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
        // Calls the hover method if you're off the ground
        if(!OnGround())
        {
            Hover();
            return;
        }
        audioSource.PlayOneShot(jumpClip);
        gravityVector = new Vector3(0, jumpPower, 0);
    }

    // Allows the player to hover when trying to jump while not on the ground
    void Hover()
    {
        // Doesn't continue if the player is grappling, their wing meter is depleted, or if they are moving upwards from jumping
        if(isGrappling || curWingMeter <= 0 || gravityVector.y > 0)
        {
            return;
        }

        // Decrements the wing meter
        curWingMeter -= Time.deltaTime;
        
        // Decelarates the player while their acceleration is faster than the maximum hover acceleration
        if(gravityVector.y < maxHoverAccel)
        {
            gravityVector.y -= 4 * gravityAccel * Time.deltaTime;
        }
        
        // Keeps the player's accelaration from going faster than max hover acceleration
        Mathf.Clamp(gravityVector.y, maxHoverAccel, 0);
        
        cc.Move(gravityVector * Time.deltaTime); // Moves the player down over time
    }

    // Rotates the player model but not the entire object
    public void RotateForCamera(Transform cameraTransform)
    {
        playerModel.transform.rotation = cameraTransform.rotation;
    }

    // Fires pistol
    public void FirePistol()
    {
        audioSource.PlayOneShot(shootClip);
        pistol.Shoot();
    }

    // Throws a grenade
    public void ThrowGrenade()
    {
        audioSource.PlayOneShot(shootClip);
        pistol.Grenade();
    }

    // Moves player to shot target
    public void Grapple(GameObject enemy, float cutOffRange)
    {
        // Prevents player from grappling more than once
        if(isGrappling)
        {
            return;
        }

        StartCoroutine(GrappleRoutine(enemy, cutOffRange)); // Starts coroutine for grapple
    }

    IEnumerator GrappleRoutine(GameObject target, float cutOffRange)
    {
        // Plays grapple sound
        audioSource.PlayOneShot(grappleClip);
        
        // Disables player from moving with keyboard, doubles speed, and resets acceleration
        isGrappling = true;
        movementSpeed *= 2;
        gravityVector.y = -2;


        // While there is distance between the player and target...
        while(Vector3.Distance(transform.position, target.transform.position) > cutOffRange)
        {
            MoveTowards(target.transform.position); // Move towards the target
            yield return null;
        }

        Destroy(target); // Destroy the target when you come in contact
        // Reenable keyboard movement and reset speed
        isGrappling = false;
        movementSpeed /= 2;

        yield return null;
    }

    // Moves player forward for a short time
    public void Dash()
    {
        // Prevents player from dashing more than once
        if(isDashing || curWingMeter <= 0)
        {
            return;
        }
        curWingMeter -= 1;
        audioSource.PlayOneShot(dashClip);
        StartCoroutine(DashRoutine()); // Starts coroutine for dashing
    }

    IEnumerator DashRoutine()
    {   
        // Disables player from moving with keyboard, doubles speed, and resets gravity acceleration
        isDashing = true;
        movementSpeed *= 2;
        gravityVector.y = -2;


        // Dash for half a second
        float dashTimer = 0.5f;
        while(dashTimer > 0)
        {
            // Takes the pistols position, which is in front of them 
            Vector3 forward = pistol.transform.position;
            MoveTowards(forward); // Move forwards

            dashTimer -= Time.deltaTime; // Decrement timer
            yield return null;
        }

        // Reenable keyboard movement and reset speed
        isDashing = false;
        movementSpeed /= 2;

        yield return null;
    }

    // Helper function for Grapple() and Dash() to move player to the target position
    void MoveTowards(Vector3 target)
    {
        Vector3 moveVector = target - transform.position;
        moveVector = moveVector.normalized;
        cc.Move(moveVector * movementSpeed * Time.deltaTime);
    }

    // Ends the level when the goal is reached
    void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Goal"))
        {
            return;
        }

        SceneManager.LoadScene("WinScene");
    }
}
