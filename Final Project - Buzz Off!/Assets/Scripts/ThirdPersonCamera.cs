using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    // Camera rotation
    float xRotation = 0;
    float yRotation = 0;

    // How fast the camera rotates
    public float xSpeed = 60f;
    public float ySpeed = 60f;

    // Prevents you from flippinng the camera upside down
    public float maxLookDownAngle = 60;
    public float maxLookUpAngle = -55;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Locks the camera inside the game window
    }

    // Update is called once per frame
    public void AdjustRotation(float xDelta, float yDelta)
    {
        // Adjusts the values based on how fast the camera can rotate
        xDelta *= xSpeed * Time.deltaTime;
        yDelta *= ySpeed * Time.deltaTime;

        // Changes the camera's rotation values
        yRotation += xDelta;
        xRotation -= yDelta;

        // Keeps the xRotation (looking up and down) in certain bounds to prevent the camera from flipping before changing the camera's rotation
        xRotation = Mathf.Clamp(xRotation, maxLookDownAngle, maxLookUpAngle);
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
    }
}