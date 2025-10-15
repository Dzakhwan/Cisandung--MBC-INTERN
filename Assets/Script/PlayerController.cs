using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sprintSpeed = 10f;
    public float jumpForce = 5f;
    public bool IsGrounded = false;
    public bool canMove = true;
    private AudioManager audioManager;
    private Vector2 moveInput;
    private Rigidbody rb;
    private Animator anim;
    private float currentSpeed;
    private IInteractable interactableInRange;
    public GameManager GameManager;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        currentSpeed = moveSpeed;
        GameObject audioObject = GameObject.FindGameObjectWithTag("Audio");
        if (audioObject != null)
        {
            audioManager = audioObject.GetComponent<AudioManager>();
        }
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump()
    {
        Jump();
        Debug.Log("Jump");
    }

    public void OnSprint(InputValue value)
    {
        if (value.isPressed)
        {
            currentSpeed = sprintSpeed;
            anim.SetBool("IsRunning",true);
            Debug.Log("Sprint");
        }
        else
        {
            anim.SetBool("IsRunning",false);
            Debug.Log("Stop Sprint");
            currentSpeed = moveSpeed;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            IsGrounded = true;
        }
        if (audioManager != null)
        {
            audioManager.PlayLandSound();
        }
    }
    void OnCollisionExit(Collision collision)
    {

        if (collision.gameObject.CompareTag("Ground"))
        {
            IsGrounded = false;
        }
    }


private void OnTriggerEnter(Collider other)
{
    if (other.TryGetComponent<IInteractable>(out var interactable))
    {
        interactableInRange = interactable;
    }
    if (other.CompareTag("Obstacle"))
    {
            Die();
            if (audioManager != null)
            {
                audioManager.PlayDeathSound();
            }
    }
    if (other.CompareTag("CheckpointDog"))
    {
        Vector3 checkpointPosition = other.transform.position;
        UpdateCheckpoint(checkpointPosition);
        Debug.Log("Checkpoint Dog reached at: " + checkpointPosition);
        if (audioManager != null)
        {
            audioManager.PlayDogBarking();
        }
    }
    else if (other.CompareTag("Checkpoint"))
    {
        Vector3 checkpointPosition = other.transform.position;
        UpdateCheckpoint(checkpointPosition);
        Debug.Log("Checkpoint reached at: " + checkpointPosition);
        if (audioManager != null)
        {
            audioManager.StopDogBarking();
        }
    }
        if (other.gameObject.name == "Vcam2")
        {
            if (audioManager != null)
            {
                audioManager.ChangeBGM();
                Debug.Log("BGM changed to VCam BGM.");
            }
        }
    if(other.CompareTag("Ending"))
        {
           GameManager.TriggerOutroCutscene();   
        }
    
}

private void OnTriggerExit(Collider other)
{
        if (other.TryGetComponent<IInteractable>(out var interactable) && interactable == interactableInRange)
        {
            interactableInRange = null;
        }
}

public void OnInteract()
{
    Debug.Log("OnInteract called");
    if (interactableInRange != null)
    {
        interactableInRange.OnInteract();
    }
}

    public void Jump()
    {
        if (IsGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            IsGrounded = false;
            anim.SetTrigger("Jumping");
        }
    }

    

    void FixedUpdate()
    {
       if (!canMove) return; 

        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        rb.MovePosition(rb.position + moveDirection * currentSpeed * Time.fixedDeltaTime);
        if (moveDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            rb.rotation = Quaternion.RotateTowards(rb.rotation, toRotation, 720 * Time.fixedDeltaTime);
            anim.SetBool("IsWalking", true);
        }
        else
        {
            anim.SetBool("IsWalking", false);
        }
    }
    Vector3 CheckpointPost;
    private void Start()
    {
        CheckpointPost = transform.position;
    }

    void Die()
    {
        Debug.Log("Player Died");
        StartCoroutine(Respawn(1.0f));
    }
    public void UpdateCheckpoint(Vector3 newCheckpoint)
    {
        CheckpointPost = newCheckpoint;
        Debug.Log("Checkpoint Updated");
    }
    IEnumerator Respawn(float delay)
    {
        yield return new WaitForSeconds(delay);
        transform.position = CheckpointPost;
        rb.linearVelocity = Vector3.zero;
        Debug.Log("Player Respawned");
    }

}