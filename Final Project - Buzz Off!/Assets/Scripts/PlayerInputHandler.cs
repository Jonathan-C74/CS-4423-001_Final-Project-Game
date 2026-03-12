using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInputHandler : MonoBehaviour
{
    
    public Player player;

    public ThirdPersonCamera playerCamera;
    
    // Update is called once per frame
    void Update()
    {
        // Movement
        Vector3 direction = new Vector3(0, 0, 0);
        if(Keyboard.current.wKey.isPressed)
        {
            direction.z += 1;
        }
        if(Keyboard.current.aKey.isPressed)
        {
            direction.x -= 1;
        }
        if(Keyboard.current.sKey.isPressed)
        {
            direction.z -= 1;
        }
        if(Keyboard.current.dKey.isPressed)
        {
            direction.x += 1;
        }
        if(Keyboard.current.spaceKey.isPressed)
        {
            player.Jump();
        }

        // Changes the direction based on the camera's rotation
        direction = playerCamera.transform.TransformDirection(direction);
        // Prevents the player from moving up and down
        direction.y = 0;
        
        player.Move(direction);

        // Reset game for testing purposes
        if(Keyboard.current.rKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // Camera Movement
        playerCamera.AdjustRotation(Mouse.current.delta.x.value, Mouse.current.delta.y.value); // Based on player's mouse movement
    }
}