using UnityEngine;

public class CanGrapple : MonoBehaviour
{
    // This script helps with the player's grapple ability
    public Player player;
    public void TriggerGrapple()
    {
        player.Grapple(this.gameObject);
    }
}
