using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * TODO
 * 
 * sprint fov like wr
 * 
 * downforce
 * 
*/

public class PlayerMovement : MonoBehaviour
{
    #region
    public Transform orientation;
    public MoveCamera camHolder;
    PlayerController player;

    [Header("Movement")]
    public float moveSpeed = 6f;
    public float movementMultiplier = 10f;
    public float airMultipler = 0.4f;
    public float crouchMultipler = 0.5f;
    public float wallrunMultipler = 1.2f;
    public PhysicMaterial physMat;
    public float maxSlopeAngle;

    [Header("Crouch/Slide")]
    Vector3 playerScale;
    public Vector3 crouchScale = new(1, 0.5f, 1);
    public float slideForce = 400f;
    public bool crouching;

    [Header("Sprinting")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 6f;
    public float acceleration = 10f;

    [Header("Jumping")]
    public float jumpForce = 15;
    public bool canJump = true;
    public bool jumpingEnbled = true;
    public float jumpCooldown = 0.1f;
    public float crouchJumpMultipler = 0.5f;
    public bool doubleJump = true;
    public bool doubleJumped;
    public float doubleJumpMultipler = 0.3f;


    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Drag")]
    public float groundDrag = 6f;
    public float airDrag = 1f;
    public float crouchDrag = 1f;
    public float wallrunDrag = 1f;

    Vector2 movement;

    Vector3 moveDirection;

    Rigidbody rb;
    Wallrun wr;

    [Header("Ground detection")]
    public bool isGrounded;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public Transform groundCheck;

    #endregion

    private void Start()
    {
        playerScale = transform.localScale;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        wr = GetComponent<Wallrun>();
        player = GetComponent<PlayerController>();
    }

    private void Update()
    {
        CheckGrounded();

        MyInput();
        ControlDrag();
        ControlSpeed();
    }

    void CheckGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && doubleJumped)
        {
            doubleJumped = false;
        }
    }

    void MyInput()
    {
        if (PauseMenu.GameIsPaused) return;

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * movement.y + orientation.right * movement.x;

        if (Input.GetKeyDown(jumpKey) && canJump && jumpingEnbled)
        {
            Jump();
        }

        if (Input.GetKeyDown(crouchKey))
            StartCrouch();
        if (Input.GetKeyUp(crouchKey))
            StopCrouch();
    }

    void Jump()
    {
        if (!canJump || !jumpingEnbled || wr.wallrunning) return;

        Vector3 force = transform.up * jumpForce;

        if (isGrounded)
        {
            rb.drag = airDrag;

            if (crouching)
            {
                force *= crouchJumpMultipler;
            }
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }
        else if (doubleJump && !doubleJumped)
        {
            doubleJumped = true;

            force *= doubleJumpMultipler;

            if (rb.velocity.y < 0)
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }
        else return;

        rb.AddForce(force, ForceMode.Impulse);

        JumpCooldown();
    }

    public void JumpCooldown()
    {
        canJump = false;
        Invoke(nameof(EnableJump), jumpCooldown);
    }

    private void EnableJump()
    {
        canJump = true;
    }

    private void StartCrouch()
    {
        crouching = true;

        transform.localScale = crouchScale;
        transform.position = new Vector3(transform.position.x, transform.position.y - (playerScale.y - crouchScale.y), transform.position.z);
        camHolder.UpdatePos();
        //sliding
        if (rb.velocity.magnitude > 0.5f && isGrounded && movement.y == 1 && player.TrySlide())
        {
            Physics.Raycast(groundCheck.position, Vector3.down, out RaycastHit slopeHit, groundDistance, groundMask);
            float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
            rb.AddForce((slopeAngle != 0 ? Vector3.ProjectOnPlane(orientation.transform.forward, slopeHit.normal) : orientation.transform.forward) * slideForce, ForceMode.Impulse);
        }
    }

    private void StopCrouch()
    {
        crouching = false;

        transform.localScale = playerScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + (playerScale.y - crouchScale.y), transform.position.z);
        camHolder.UpdatePos();
    }

    private void ControlSpeed()
    {
        if (PauseMenu.GameIsPaused) return;

        float to;

        if (Input.GetKey(sprintKey))
        {
            if (movement.sqrMagnitude != 0 && player.TrySprint())
            {
                to = sprintSpeed;
            }
            else
            {
                to = walkSpeed;
            }
        }
        else
        {
            to = walkSpeed;
        }

        moveSpeed = Mathf.Lerp(moveSpeed, to, acceleration * Time.deltaTime);
    }

    private void ControlDrag()
    {
        if (isGrounded)
        {
            if (crouching)
                rb.drag = crouchDrag;
            else
            rb.drag = groundDrag;
        }
        else if (wr.wallrunning)
        {
            rb.drag = wallrunDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        //slope shit
        Physics.Raycast(groundCheck.position, Vector3.down, out RaycastHit slopeHit, groundDistance, groundMask);
        float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
        bool onslope = slopeAngle != 0;
        if (onslope)
        {
            if (slopeAngle < maxSlopeAngle)
            {
                Vector3 gravityForce = Physics.gravity - Vector3.Project(Physics.gravity, slopeHit.normal);
                rb.AddForce(-gravityForce, ForceMode.Acceleration);
            }
        }

        //move dir
        Vector3 movedir = Vector3.zero;

        Vector3 slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);

        if (wr.wallrunning)
        {
            movedir = movement.y * movementMultiplier * moveSpeed * wallrunMultipler * orientation.forward;
        }
        if (crouching && isGrounded)
        {
            movedir = crouchMultipler * movementMultiplier * moveSpeed * (onslope ? slopeMoveDirection : moveDirection).normalized;
        }
        if (isGrounded && !crouching)
        {
            movedir = movementMultiplier * moveSpeed * (onslope ? slopeMoveDirection : moveDirection).normalized;
        }
        if (!isGrounded)
        {
            movedir = airMultipler * movementMultiplier * moveSpeed * moveDirection.normalized;
        }

        rb.AddForce(movedir, ForceMode.Acceleration);
    }
}
