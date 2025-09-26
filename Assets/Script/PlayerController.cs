using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public bool IsGrounded = false;
    private Vector2 moveInput;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Method ini akan dipanggil otomatis oleh Send Messages
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    public void OnJump()
    {
        Jump();
        Debug.Log("Jump");
    }
    public void Walk()
    {
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.deltaTime);
    }
    public void Jump()
    {
        if (IsGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            IsGrounded = false; // Set false agar tidak lompat terus
        }
    }

    void Update()
    {
        Walk();
        
    }
}