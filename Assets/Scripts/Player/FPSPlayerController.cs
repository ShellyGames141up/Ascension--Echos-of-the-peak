using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSPlayerController : Movement
{
    public float mouseSensitivity = 2f;
    [SerializeField] private Transform playerCamera;
    public float maxLookAngle = 90f;
    public bool invertY = false;
    private float xRotation = 0f;
    private bool inputEnabled = true;
    protected override void Awake()
    {
        base.Awake();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        if (playerCamera == null)
        {
            Debug.LogError("Player Camera is not assigned in FPSPlayerController!");
        }
    }
    void Update()
    {
        if (!inputEnabled) return;
        
        float horizontalInput = Input.GetKey(KeyCode.D) ? 1 : (Input.GetKey(KeyCode.A) ? -1 : 0);
        float verticalInput = Input.GetKey(KeyCode.W) ? 1 : (Input.GetKey(KeyCode.S) ? -1 : 0);
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);
        bool sprintPressed = Input.GetKey(KeyCode.LeftShift);
        
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * (invertY ? -1 : 1);
        
        base.Update();
        
        HandleMovement(new Vector2(horizontalInput, verticalInput));
        HandleLook(mouseX, mouseY);
        HandleJump(jumpPressed);
        HandleSprint(sprintPressed);
    }
    private void HandleLook(float mouseX, float mouseY)
    {
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
        
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
        Cursor.lockState = enabled ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !enabled;
    }
    public void SetSensitivity(float sensitivity)
    {
        mouseSensitivity = sensitivity;
    }
    
    public void SetInvertY(bool invert)
    {
        invertY = invert;
    }
    private void OnDrawGizmosSelected()
    {
        if (controller != null)
        {
            Vector3 groundCheckPos = transform.position + Vector3.down * (controller.height / 2 - controller.radius);
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheckPos, groundDistance);
        }
    }
}