using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sprintSpeed = 10f;
    public float jumpForce = 5f;
    public bool IsGrounded = false;
    private AudioManager audioManager;
    private Vector2 moveInput;
    private Rigidbody rb;
    private Animator anim;
    private float currentSpeed;
    private IInteractable interactableInRange;

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

    public void PlayFootstep()
    {
        if (audioManager != null )
        {
            audioManager.PlayFootstep();
        }
    }

    void FixedUpdate()
    {
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
}