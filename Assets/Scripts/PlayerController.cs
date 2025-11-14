using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    
    [Header("World Boundaries")]
    public Vector2 minBounds;
    public Vector2 maxBounds;
    
    [Header("Collision Settings")]
    public float recoilForce = 2f;
    public float recoilDuration = 0.2f;
    
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 movement;
    private bool isRecoiling = false;
    private float recoilTimer = 0f;
    
    // Player stats
    public int cropsCollected = 0;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    void Update()
    {
        if (!isRecoiling)
        {
            // Get input using new Input System
            movement = Vector2.zero;
            
            // Check keyboard using new Input System
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
                    movement.y = 1f;
                if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
                    movement.y = -1f;
                if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                    movement.x = -1f;
                if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                    movement.x = 1f;
            }
            
            // Normalize diagonal movement
            movement = movement.normalized;
            
            // Update animator parameters
            UpdateAnimations();
        }
        else
        {
            // Handle recoil timer
            recoilTimer -= Time.deltaTime;
            if (recoilTimer <= 0)
            {
                isRecoiling = false;
            }
        }
    }
    
    void FixedUpdate()
    {
        if (!isRecoiling)
        {
            // Move the player
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
            
            // Clamp position within boundaries
            Vector2 clampedPosition = rb.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, minBounds.x, maxBounds.x);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, minBounds.y, maxBounds.y);
            rb.position = clampedPosition;
        }
    }
    
    void UpdateAnimations()
    {
        // Handle sprite flipping for left/right
        if (movement.x > 0)
        {
            spriteRenderer.flipX = false; // Face right
        }
        else if (movement.x < 0)
        {
            spriteRenderer.flipX = true; // Face left (flip)
        }
        
        // Set animator parameters - just Speed to switch between Idle and Walk
        animator.SetFloat("Speed", movement.sqrMagnitude);
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Monster collision
        if (collision.gameObject.CompareTag("Monster"))
        {
            Debug.Log("Player is hurt");
            
            // Calculate recoil direction
            Vector2 recoilDirection = (transform.position - collision.transform.position).normalized;
            
            // Apply recoil
            rb.linearVelocity = recoilDirection * recoilForce;
            isRecoiling = true;
            recoilTimer = recoilDuration;
            
            // Play hurt animation (if you have it)
            if (animator != null)
            {
                animator.SetTrigger("Hurt");
            }
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Crop collection
        if (other.CompareTag("Crop"))
        {
            cropsCollected++;
            Debug.Log("Crop harvested: " + cropsCollected);
            Destroy(other.gameObject);
        }
        
        // Animal interaction
        if (other.CompareTag("Animal"))
        {
            AnimalSound animalSound = other.GetComponent<AnimalSound>();
            if (animalSound != null)
            {
                Debug.Log(animalSound.soundText);
            }
        }
    }
}