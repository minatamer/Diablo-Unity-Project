using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //public Transform target;  // The target to follow (Barbarian)
    //public Vector3 offset = new Vector3(0f, 10f, -10f);  // Camera's offset from the target
    //public float smoothSpeed = 0.125f;  // Smooth camera follow speed

    //private Vector3 desiredPosition;  // The position the camera should move to

    //void LateUpdate()
    //{
    //    if (target == null) return;

    //    // Only update the camera's position in the X and Z axes based on the Barbarian's position
    //    // Maintain the same Y offset, keeping the camera above the character
    //    desiredPosition = new Vector3(target.position.x + offset.x, target.position.y + offset.y, target.position.z + offset.z);

    //    // Smooth the camera movement
    //    Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

    //    // Apply the smoothed position to the camera
    //    transform.position = smoothedPosition;

    //    // Ensure the camera does NOT rotate, and always looks at the target (Barbarian)
    //    transform.LookAt(target);
    //}

    public Transform target;  // The object the camera will follow
    public Vector3 offset;    // Offset between the camera and the object
    public Vector3 rotationAngle;

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
            transform.rotation = Quaternion.Euler(rotationAngle);
        }
    }
}
