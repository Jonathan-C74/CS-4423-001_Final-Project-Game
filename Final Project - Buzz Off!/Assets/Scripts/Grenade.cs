using UnityEngine;

public class Grenade : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Checks to see if the object can be bounced
        CanBounce bounce = other.GetComponent<CanBounce>();
        if(bounce != null)
        {
            bounce.Bounce(); // Bounces the object up

            // Freezes the object if it's an enemy
            EnemyCreature enemy = other.GetComponent<EnemyCreature>();
            if(enemy != null)
            {
                enemy.interupted = true; // Freezes enemy
            }
            Destroy(this.gameObject); // Destroy the grenade
        }
        
        // Destroys the object if it is destructible
        if(other.CompareTag("Destructible"))
        {
            Destroy(other.gameObject);
            Destroy(this.gameObject); // Destroy the grenade
        }
    }
}
