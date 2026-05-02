using UnityEngine;
using System.Collections;
// This script manages basic movement for enemies
public class EnemyCreature : MonoBehaviour
{
    // This boolean helps with stopping a coroutine
    public bool interupted = false;

    // Manages movement
    [Header("Stats")]
    public float movementSpeed = 3;
    public float rotateSpeed = 10;
    public float patrolRange = 7;
    IEnumerator currentState; // Changes state of the enemy

    void Start() // Starts the enemy in a patrolling state
    {
        ChangeState(PatrolStateRoutine());
    }

    // Helps change between states
    void ChangeState(IEnumerator newState)
    {
        if(currentState != null)
        {
            StopCoroutine(currentState);
        }

        currentState = newState;
        StartCoroutine(currentState);
    }

    IEnumerator PatrolStateRoutine()
    {
        while(true)
        {
            Vector3 newPosition = transform.position + new Vector3(patrolRange, 0, 0); // Set new position and move towards it
            while(Vector3.Distance(transform.position, newPosition) > 1)
            {
                MoveTowards(newPosition);

                if(interupted) // Stop moving if interupted
                {
                    ChangeState(FreezeRoutine());
                    yield break;
                }

                yield return null;
            }
            
            patrolRange = -patrolRange; // Switch directions and wait two seconds
            yield return new WaitForSeconds(2);
        }
    }

    // Stops the enemy from moving
    IEnumerator FreezeRoutine()
    {
        yield return null;

        while(interupted)
        {
            yield return null;
        }

        // Will only get to this point if the enemy is uninterupted
        ChangeState(PatrolStateRoutine());
    }
    
    // Enemy moves towards a certain position
    void MoveTowards(Vector3 target)
    {
        // Moves enemy towards the position
        Vector3 moveVector = target - transform.position;
        moveVector = moveVector.normalized;
        transform.position += moveVector * movementSpeed * Time.deltaTime;

        // Rotates the enemy to look where they're moving to
        transform.rotation = Quaternion.LookRotation(moveVector);
    }
}
