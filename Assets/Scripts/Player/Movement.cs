using UnityEngine;
using UnityEngine.Events;

public class Movement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public UnityEvent OnJump;
    public UnityEvent OnSprintStart;
    public UnityEvent OnSprintEnd;
    protected CharacterController controller;
    protected Vector3 velocity;
    protected bool isGrounded;
    protected bool isSprinting;
    
    protected virtual void Awake()
    {
        controller = GetComponent<CharacterController>();
    }
    protected virtual void Update()
    {
        HandleGroundCheck();
        HandleGravity();
    }
    protected virtual void HandleGroundCheck()
    {
        Vector3 groundCheckPos = transform.position + Vector3.down * (controller.height / 2 - controller.radius);
        isGrounded = Physics.CheckSphere(groundCheckPos, groundDistance, groundMask);
        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }
    protected virtual void HandleMovement(Vector2 input)
    {
        Vector3 moveDirection = transform.right * input.x + transform.forward * input.y;
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        controller.Move(moveDirection * currentSpeed * Time.deltaTime);
    }
    protected virtual void HandleJump(bool jumpPressed)
    {
        if (jumpPressed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            OnJump?.Invoke();
        }
    }
    protected virtual void HandleSprint(bool sprintPressed)
    {
        if (sprintPressed)
        {
            if (!isSprinting)
            {
                isSprinting = true;
                OnSprintStart?.Invoke();
            }
        }
        else if (isSprinting)
        {
            isSprinting = false;
            OnSprintEnd?.Invoke();
        }
    }
    protected virtual void HandleGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    public bool IsGrounded() => isGrounded;
    public bool IsSprinting() => isSprinting;
    public Vector3 GetVelocity() => velocity;
}
