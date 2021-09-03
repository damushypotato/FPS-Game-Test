using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallrun : MonoBehaviour
{
    public Transform orientation;
    PlayerMovement movement;

    public float wallDistance = 0.6f;
    public float minJumpHeight = 1.5f;

    public float wallRunGravityScale;
    public float wallJumpForce;

    public bool stickyEnabled = true;

    bool wallLeft;
    bool wallRight;

    Rigidbody rb;
    public CameraEffects camEffects;

    RaycastHit leftWallHit;
    RaycastHit rightWallHit;

    KeyCode jumpKey;

    public bool wallrunning;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<PlayerMovement>();
        jumpKey = movement.jumpKey;
    }

    bool CanWallRun()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight);
    }

    void CheckWall()
    {
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallDistance);
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallDistance);
    }

    private void Update()
    {
        if (PauseMenu.GameIsPaused) return;

        CheckWall();

        if (CanWallRun())
        {
            if (wallLeft)
            {
                StartWallRun();
            }
            else if (wallRight)
            {
                StartWallRun();
            }
            else
            {
                StopWallRun();
            }
        }
        else
        {
            StopWallRun();
        }
    }

    void StartWallRun()
    {
        wallrunning = true;

        rb.useGravity = false;

        float wrGravScale;
        if (movement.crouching && stickyEnabled)
        {
            wrGravScale = 0f;
            movement.physMat.dynamicFriction = 1f;
        }
        else
        {
            wrGravScale = wallRunGravityScale;
            movement.physMat.dynamicFriction = 0f;
        }

        rb.AddForce(Vector3.up * Physics.gravity.y * wrGravScale, ForceMode.Acceleration);

        camEffects.LerpFovWallrun(true);

        if (wallLeft)
        {
            camEffects.LerpTiltWallrun(-1);
        }
        else if (wallRight)
        {
            camEffects.LerpTiltWallrun(1);
        }

        if (Input.GetKeyDown(jumpKey)  && movement.canJump)
        {
            if (wallLeft)
            {
                Vector3 wallrunJumpDir = transform.up + leftWallHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallrunJumpDir * wallJumpForce * 100, ForceMode.Force);
                movement.JumpCooldown();
            }
            else if (wallRight)
            {
                Vector3 wallrunJumpDir = transform.up + rightWallHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallrunJumpDir * wallJumpForce * 100, ForceMode.Force);
                movement.JumpCooldown();
            }
        }
    }

    void StopWallRun()
    {
        wallrunning = false;

        rb.useGravity = true;

        camEffects.LerpFovWallrun(false);
        camEffects.LerpTiltWallrun(0);
    }

}
