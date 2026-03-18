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
        Destroy(this.gameObject); // Destroy the bullet
    }
}
