using UnityEngine;
using UnityEngine.InputSystem;

public class WallRunning : MonoBehaviour
{
    [Header("WallRunning")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallRunForce;
    public float wallJumpUpForce;
    public float wallJumpSideForce;
    public float wallClimbSpeed;
    public float maxWallRunTime;
    private float wallRunTimer;
    private bool readyToJump;

    [Header("Input")]
    private float horizontalInput;
    private float verticalInput;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallhit;
    private RaycastHit rightWallhit;
    private Collider lastWallhit;
    private bool wallLeft;
    private bool wallRight;
    private bool jumping;

    [Header("Exiting")]
    private bool exitingWall;
    public float exitWallTime;
    private float exitWallTimer;

    [Header("Gracity")]
    public bool useGravity;
    public float gravityCounterForce;

    [Header("References")]
    public Transform CamHolderOrientation;
    public Transform orientation;
    public PlayerCam cam;
    private PlayerMovement pm;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        CheckForWall();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if (pm.wallrunning)
        {
            WallRunningMovement();
        }
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallhit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallhit, wallCheckDistance, whatIsWall);
        if (pm.grounded) 
        {
            lastWallhit = null;
        }

    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void StateMachine()
    {

        // State 1 - Wallrunning
        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingWall)
        {
            // start wallrun
            if (!pm.wallrunning && ((lastWallhit != leftWallhit.collider && lastWallhit != rightWallhit.collider) || lastWallhit == null))
            {
                StartWallRun();
            }

            // wallrun timer
            if (wallRunTimer > 0)
            {
                wallRunTimer -= Time.deltaTime;
            }

            if (wallRunTimer <= 0 && pm.wallrunning)
            {
                exitingWall = true;
                exitWallTimer = exitWallTime;
            }

            // wall jump
            if (jumping && readyToJump)
            {
                jumping = false;
                WallJump();

            }
        }
        // State 2 - Exiting
        else if (exitingWall)
        {
            if (pm.wallrunning)
            {
                StopWallRun();
            }

            if (exitWallTimer > 0)
            {
                exitWallTimer -= Time.deltaTime;
            }

            if (exitWallTimer <=0)
            {
                exitingWall = false;
            }
        }
        // State 3 - None
        else
        {
            if (pm.wallrunning)
            {
                StopWallRun();
            }
        }
    }

    private void StartWallRun()
    {
        readyToJump = true;

        if (wallLeft)
        {
            lastWallhit = leftWallhit.collider;
        }
        else if (wallRight)
        {
            lastWallhit = rightWallhit.collider;
        }
        pm.wallrunning = true;

        wallRunTimer = maxWallRunTime;

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        // apply camera effects
        if (wallLeft) 
        {
            cam.StopAllCoroutines();
            cam.StartCoroutine(cam.SmoothTiltChange(-5f));
        }
        if (wallRight)
        {
            cam.StopAllCoroutines();
            cam.StartCoroutine(cam.SmoothTiltChange(5f));
        }
    }

    private void WallRunningMovement()
    {
        rb.useGravity = useGravity;

        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((orientation.forward - wallForward).magnitude > (orientation.forward - - wallForward).magnitude) 
        {
            wallForward = -wallForward;
        }

        // forward force
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        // upwards/downwards force
        if (!useGravity)
        {
            rb.velocity = new Vector3(rb.velocity.x, wallClimbSpeed * CamHolderOrientation.forward.y, rb.velocity.z);
        }
        // push to wall force
        if (!(wallLeft && horizontalInput > 0) || !(wallRight && horizontalInput < 0))
        {
            rb.AddForce(-wallNormal * 100, ForceMode.Force);
        }

        // weaken gravity
        if (useGravity)
        {
            rb.AddForce(transform.up * gravityCounterForce, ForceMode.Force);
        }
    }

    private void StopWallRun()
    {
        pm.wallrunning = false;

        // reset camera effects
        cam.StopAllCoroutines();
        cam.StartCoroutine(cam.SmoothTiltChange(0f));
    }

    private void WallJump()
    {
        readyToJump = false;
        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;

        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        // reset y velocity and add force
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
    }

    public void OnMovePlayer(InputAction.CallbackContext context)
    {
        Vector2 inputMovement = context.ReadValue<Vector2>();
        horizontalInput = inputMovement.x;
        verticalInput = inputMovement.y;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started) jumping = true; else if (context.canceled) jumping = false;
    }
}
