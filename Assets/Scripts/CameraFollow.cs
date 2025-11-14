using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform player;
    
    [Header("Camera Boundaries")]
    public Vector2 minCameraBounds;
    public Vector2 maxCameraBounds;
    
    [Header("Smoothing")]
    public float smoothSpeed = 0.125f;
    public bool useSmoothing = true;
    
    private Camera cam;
    private float cameraHalfWidth;
    private float cameraHalfHeight;
    
    void Start()
    {
        cam = GetComponent<Camera>();
        
        // Set aspect ratio to 16:9
        cam.aspect = 16f / 9f;
        
        // Calculate camera size in world units
        cameraHalfHeight = cam.orthographicSize;
        cameraHalfWidth = cameraHalfHeight * cam.aspect;
    }
    
    void LateUpdate()
    {
        if (player == null)
            return;
        
        // Get desired position
        Vector3 desiredPosition = new Vector3(player.position.x, player.position.y, transform.position.z);
        
        // Clamp camera position to boundaries
        float clampedX = Mathf.Clamp(desiredPosition.x, 
            minCameraBounds.x + cameraHalfWidth, 
            maxCameraBounds.x - cameraHalfWidth);
            
        float clampedY = Mathf.Clamp(desiredPosition.y, 
            minCameraBounds.y + cameraHalfHeight, 
            maxCameraBounds.y - cameraHalfHeight);
        
        Vector3 clampedPosition = new Vector3(clampedX, clampedY, transform.position.z);
        
        // Apply smoothing if enabled
        if (useSmoothing)
        {
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, clampedPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
        else
        {
            transform.position = clampedPosition;
        }
    }
}