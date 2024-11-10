using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    public Transform orientation;


    [Header("Movements")]
    public float walkSpeed;
    public float sprintSpeed;
    private float moveSpeed;
    public float slideSpeed;
    public float wallrunSpeed;

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    public float groundDrag;

    Vector2 inputMovement;
    private bool sprinting;

    [Header("Jump")]
    bool readyToJump = true;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;

    [Header("Crouch")]
    public float crouchYScale;
    float startYScale;
    public float crouchSpeed;

    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    public float slideCooldown;
    private bool readyToSlide = true;
    private float slideTimer;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;


    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        wallrunning,
        crouching,
        sliding,
        air
    }

    public bool crouching;
    public bool sliding;
    public bool wallrunning;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        //ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MovePlayer();
        SpeedControl();
        StateHandler();

        //apply drag
        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    private void FixedUpdate()
    {
        //MovePlayer();
        if (sliding)
        {
            SlidingMovement();
        }
    }

    private void StateHandler()
    {
        //Mode - wallrunning
        if (wallrunning)
        {
            state = MovementState.wallrunning;
            desiredMoveSpeed = wallrunSpeed;
        }
        // Mode - sliding
        else if (sliding)
        {
            state = MovementState.sliding;

            if (OnSlope() && rb.velocity.y < 0.1f)
            {
                desiredMoveSpeed = slideSpeed;
            }
            else
            {
                desiredMoveSpeed = sprintSpeed;
            }
        }
        // Mode - crouching
        else if (crouching)
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }
        // Mode - sprinting
        else if (sprinting)
        {
            if (grounded)
            {
                state = MovementState.sprinting;
                desiredMoveSpeed = sprintSpeed;
            }
        }
        // Mode - walking
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }
        // Mode - Air
        else
        {
            state = MovementState.air;
        }


        // check if desiredMoveSpeed has changed drasticlly
        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 10f && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
        {
            moveSpeed = desiredMoveSpeed;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    /*private IEnumerator SmoothlyLerpMoveSpeed()
    {
        // smoothly lerp movementSpeed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time/difference);
            time += Time.deltaTime;
            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }*/

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
            {
                time += Time.deltaTime * speedIncreaseMultiplier; 
            }
            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    public void OnMovePlayer(InputAction.CallbackContext context)
    {
        inputMovement = context.ReadValue<Vector2>();
    }
    private void MovePlayer()
    {
        moveDirection = orientation.forward * inputMovement.y + orientation.right * inputMovement.x;

        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 40f, ForceMode.Force);
            }
        }
        else if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
        if (!wallrunning) 
        { 
            rb.useGravity = !OnSlope(); 
        }
    }

    private void SpeedControl()
    {
        if (OnSlope() && !exitingSlope)
        {
            if(rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (readyToJump && grounded && context.performed)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }
    private void Jump()
    {
        exitingSlope = true;

        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (GameManager.instance.maintainCrouch)
        {
            if (context.started)
            {
                if ((inputMovement != new Vector2(0, 0)) && state == PlayerMovement.MovementState.sprinting && readyToSlide)
                {
                    StartSlide();
                }
            }

            if (context.performed)
            {
                if (!crouching && state != MovementState.wallrunning && (state != MovementState.sprinting || (!readyToSlide && !sliding)))
                {
                    transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
                    rb.AddForce(Vector3.down * 10f, ForceMode.Impulse);
                    crouching = true;
                }
            }

            if (context.canceled)
            {
                transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
                crouching = false;

                if (sliding)
                {
                    StopSlide();
                }
            }
        }
        else
        {
            if (context.started)
            {
                if (!crouching && state != MovementState.wallrunning && (state != MovementState.sprinting || (!readyToSlide && !sliding)))
                {
                    transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
                    rb.AddForce(Vector3.down * 10f, ForceMode.Impulse);
                    crouching = true;
                }

                if (crouching)
                {
                    transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
                    crouching = false;
                }

                if ((inputMovement != new Vector2(0, 0)) && state == PlayerMovement.MovementState.sprinting && readyToSlide)
                {
                    StartSlide();
                }
            }

            if (context.canceled)
            {
                if (sliding)
                {
                    StopSlide();
                }
            }
        }
    }

    private void StartSlide()
    {
        sliding = true;
        readyToSlide = false;

        transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
        rb.AddForce(Vector3.down * 10f, ForceMode.Impulse);

        slideTimer = maxSlideTime;

        Invoke(nameof(ResetSlide), slideCooldown);
    }

    private void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // sliding normal
        if (!OnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

            slideTimer -= Time.deltaTime;
        }
        else
        {
            rb.AddForce(GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
        }

        if (slideTimer <= 0)
        {
            StopSlide();
        }
    }

    private void StopSlide()
    {
        sliding = false;

        //transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
    }

    private void ResetSlide()
    {
        readyToSlide = true;
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.started) sprinting = true; else if (context.canceled) sprinting = false;
    }
}
