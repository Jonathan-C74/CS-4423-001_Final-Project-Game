using UnityEngine;
using System.Collections;
public class CanBounce : MonoBehaviour
{
    // Manages gravity of the object
    [Header("Gravity")]
    public float bouncePower = 10f;
    public Transform groundCheck; // Position below the object to check if on ground
    public LayerMask groundMask; // The mask associated with walkable objects
    public float gravityAccel = -14.2f; // How fast the object accelerates to the ground
    public float maxGravityAccel = -19.8f; // The fastest the object is allowed to fall
    Vector3 gravityVector;

    // Manages the movement of the object if it's an enemy
    EnemyCreature enemy;
    
    // Stops the object from falling if it's being grappled to or if it's not bouncing
    public bool isGrappled = false;
    public bool isBouncing = false;

    void Awake() // Access the object's Enemy component if available
    {
        enemy = GetComponent<EnemyCreature>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Set the gravity of the object
        Bounce();
        gravityVector = new Vector3(0, -2, 0);
    }

    // Update is called once per frame
    void Update()
    {
        // If the object is an enemy, unfreeze it when it's on the ground
        if(OnGround() && enemy != null)
        {
            enemy.interupted = false;
            enemy.patrolRange = -enemy.patrolRange;
        }
    }

    // Checks if the object is on the ground
    public bool OnGround()
    {
        // Creates a collider below the object and checks if it connects with a walkable object
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
        // Resets the objects's acceleration if it's on the ground
        if(OnGround() && gravityVector.y <= 0)
        {
            gravityVector.y = 0;
        }

        gravityVector.y += gravityAccel * Time.deltaTime; // Accelerates the object the longer it's in the air
        
        // Prevents the object from falling past max acceleration
        if(gravityVector.y <= maxGravityAccel)
        {
            gravityVector.y = maxGravityAccel;
        }

        // Move the object over time
        transform.position += gravityVector * Time.deltaTime;
    }
    public void Bounce()
    {
        if(isBouncing)
        {
            return;
        }
        isBouncing = true;
        gravityVector = new Vector3(0, bouncePower, 0);
        StartCoroutine(BounceRoutine());
    }

    IEnumerator BounceRoutine()
    {
        // Simulate gravity for half a second
        float cooldown = 0.5f;
        while(cooldown > 0)
        {
            if(isGrappled) // Immediately stop if grappled
            {
                yield break;
            }

            SimulateGravity();
            cooldown -= Time.deltaTime;
            yield return null;
        }

        // Continue simulating gravity while not on the ground
        while(!OnGround())
        {
            if(isGrappled) // Immediately stop if grappled
            {
                yield break;
            }

            SimulateGravity();
            yield return null;
        }
        isBouncing = false;

        yield return null;
    }
}
