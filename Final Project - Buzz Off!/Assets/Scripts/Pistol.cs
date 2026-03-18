using UnityEngine;

public class Pistol : MonoBehaviour
{
    [Header("Bullet Stats")]
    public float bulletSpeed = 10;
    public float lifetime = 10;
    public GameObject bulletPrefab; // Reference to bullet object
    public Transform spawnTransform; // Reference to where the bullet starts
    
    public void Shoot()
    {
        GameObject newBullet = Instantiate(bulletPrefab, spawnTransform.position, transform.rotation); // Create a new bullet
        newBullet.GetComponent<Rigidbody>().linearVelocity = transform.forward * bulletSpeed; // Access the bullet's Rigidbody to make it move forward
        Destroy(newBullet, lifetime); // Destroy the bullet after a certain amount of time
    }
}
