using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    #region Attributes
    //Components
    private CharacterController characterController;
    private Transform cameraTransform;
    private WeaponController weaponController;

    //Movement and jump configuration parameters
    [Header("Movement and jump parameters")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float multiplier = 2f;
    [SerializeField] private float jumpForce = 1.5f;
    [SerializeField] private float gravity = Physics.gravity.y;

    //Input fields for movement and look actions
    private Vector2 moveInput; //Left, right, front and back
    private Vector2 lookInput;

    //Velocity and rotation variables
    private Vector2 velocity;
    private float verticalVelocity;
    private float verticalRotation;

    //Is sprinting state
    private bool isSprinting;
    private bool isMoving;
    private bool isDrowned;

    //Stamina quantity
    [Header("Stamina Settings")]
    [SerializeField] private int maxStamina = 130;
    public int MaxStamina
    {
        get => maxStamina;
        set => maxStamina = Mathf.Max(0, value); //Max stamina will not be lower than 0
    }

    private float currentStamina;
    public float CurrentStamina
    {
        get => currentStamina;
        set => currentStamina = Mathf.Clamp(value, 0, maxStamina);
    }

    [Tooltip("Quantity of stamina to be reduced when using it")]
    [SerializeField] private float staminaReduction = 30f;
    [Tooltip("Quantity of stamina to be restored when not using it")]
    [SerializeField] private float staminaRestoration = 10f;


    //Camera look sensitivity and max angle to limit vertical rotation
    [Header("Camera Settings")]
    [Tooltip("Speed of camera rotation when looking")]
    [SerializeField] private float lookSensitivity = 1f;
    [Tooltip("Max angle of camera rotation when looking up and down")]
    [SerializeField] private float maxLookAngle = 80f;
    #endregion

    #region Event Functions
    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
        weaponController = GetComponent<WeaponController>();

        //Hide Cursor
        Cursor.lockState = CursorLockMode.Locked;

        //Stamina quantity at start of game
        currentStamina = maxStamina;
    }

    private void Update()
    {
        //Manage Player Movement
        MovePlayer();
        //Manage Camera Rotation
        LookAround();
        //Use stamina
        UseStamina();
    }
    #endregion

    #region Other Methods
    /// <summary>
    /// This method recieves movement input from Input System
    /// </summary>
    /// <param name="context"></param>
    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        // Check if character is moving based on input
        isMoving = moveInput != Vector2.zero;
    }

    /// <summary>
    /// This method recieves look input from Input System
    /// </summary>
    /// <param name="context"></param>
    public void Look(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
        
    }

    /// <summary>
    /// This method recieves jump input from Input System and activates the jump if grounded
    /// </summary>
    /// <param name="context"></param>
    public void Jump(InputAction.CallbackContext context)
    {
        if (characterController.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    /// <summary>
    /// This method recieves sprint input from Input System and changes isSprinted to true
    /// </summary>
    /// <param name="context"></param>
    public void Sprint(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            // Start sprinting only if stamina is available
            isSprinting = currentStamina > 0 && isMoving && !isDrowned;
        }
        else if (context.canceled)
        {
            // Stop sprinting when the sprint action is canceled
            isSprinting = false;
        }
    }

    /// <summary>
    /// This method recieves shoot input from Input System and shoots if possible
    /// </summary>
    /// <param name="context"></param>
    public void Shoot(InputAction.CallbackContext context)
    {
        
        if (weaponController.CanShoot() && context.started) weaponController.Shoot();
        
    }

    /// <summary>
    /// This method handles player movement and jump based on Input and applies gravity
    /// </summary>
    private void MovePlayer()
    {
        //Fall from jump
        if (characterController.isGrounded)
        {
            //Restart vertical velocity when ground is touched
            verticalVelocity = 0f;
        } else
        {
            //When is falling down increment velocity with gravity and time
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 move = new Vector3 (0, verticalVelocity, 0);
        characterController.Move(move * Time.deltaTime);

        //Movement
        Vector3 moveDirection = new Vector3 (moveInput.x, 0, moveInput.y);
        moveDirection = transform.TransformDirection(moveDirection);

        float targetSpeed = isSprinting ? speed * multiplier : speed;
        characterController.Move(moveDirection * targetSpeed * Time.deltaTime);

        //Apply gravity constantly
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

    }

    /// <summary>
    /// This method manages the movement of the player and camera in order to look around
    /// </summary>
    private void LookAround()
    {
        //Horizontal rotation (Y-axis)
        float horizontalRotation = lookInput.x * lookSensitivity;
        transform.Rotate(Vector3.up * horizontalRotation); //Transforms the player rotation

        //Vertical rotation (X-axis) with clamping to prevent over-rotation
        verticalRotation -= lookInput.y * lookSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f); //Transform degrees; transforms the camera rotation
    }

    /// <summary>
    /// This method manages the quantity of stamina of the player when sprinting and when not sprinting
    /// </summary>
    private void UseStamina()
    {
        if (isSprinting && currentStamina > 0)
        {
            // Reduce stamina gradually over time
            currentStamina -= staminaReduction * Time.deltaTime;

            if (currentStamina <= 0.1f)
            {
                currentStamina = 0;
                isSprinting = false;
                isDrowned = true;
                UIManager.Instance.ChangeStaminaBarColor(isDrowned);
            }
        }
        else if (!isSprinting && currentStamina < maxStamina)
        {
            if (isDrowned)
            {
                currentStamina += staminaRestoration/2 * Time.deltaTime;
            } else
            {
                // Gradually restore stamina when not sprinting
                currentStamina += staminaRestoration * Time.deltaTime;
            }
            
        }

        if(currentStamina == maxStamina)
        {
            isDrowned = false;
            UIManager.Instance.ChangeStaminaBarColor(isDrowned);
        }

        // Clamp stamina to ensure it stays within bounds
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }

    #endregion

}
