using UnityEngine;

public class Bullet : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Checks to see if the object can be grappled to
        CanGrapple grapple = other.GetComponent<CanGrapple>();
        if(grapple == null)
        {
            return;
        }
        
        grapple.TriggerGrapple(); // Triggers the grapple function
        
        // Freezes the enemy in place
        EnemyCreature enemy = other.GetComponent<EnemyCreature>();
        if(enemy != null)
        {
            enemy.interupted = true;
        }

        // Stops the object from falling if it can bounce
        CanBounce bounce = other.GetComponent<CanBounce>();
        if(bounce != null)
        {
            bounce.isGrappled = true;
        }

        Destroy(this.gameObject); // Destroy the bullet
    }
}