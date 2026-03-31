using UnityEngine;

public class Pistol : MonoBehaviour
{
    [Header("Bullet Stats")]
    public float bulletSpeed = 10;
    public float lifetime = 10;
    public GameObject bulletPrefab; // Reference to bullet object
    public Transform spawnTransform; // Reference to where the bullet starts

    [Header("Aiming")]
    public ThirdPersonCamera aim; // Reference to camera to adjust bullet's trajectory
    
    public void Shoot()
    {
        GameObject newBullet = Instantiate(bulletPrefab, spawnTransform.position, transform.rotation); // Create a new bullet
        newBullet.GetComponent<Rigidbody>().linearVelocity = aim.transform.forward * bulletSpeed; // Access the bullet's Rigidbody to make it move forward based on camera rotation
        Destroy(newBullet, lifetime); // Destroy the bullet after a certain amount of time
    }
}
