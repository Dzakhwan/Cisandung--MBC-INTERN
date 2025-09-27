using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sprintSpeed = 10f;
    public float jumpForce = 5f;
    public bool IsGrounded = false;
    private Vector2 moveInput;
    private Rigidbody rb;
    private Animator anim;
    private float currentSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        currentSpeed = moveSpeed;
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
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        rb.MovePosition(rb.position + moveDirection * currentSpeed * Time.fixedDeltaTime);
    }
}