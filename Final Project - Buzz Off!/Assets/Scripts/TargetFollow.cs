//using Unity.VisualScripting.FullSerializer.Internal;
using UnityEngine;

// This script helps the camera follow the player
public class TargetFollow : MonoBehaviour
{
    public Transform followTransform;
    
    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = followTransform.position;
    }
}