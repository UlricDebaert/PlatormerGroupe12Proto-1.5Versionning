using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float movementInputDirection;
    public float movementSpeed = 10f;
    public float jumpForce = 16.0f;
    public float groundCheckRadius=0.4f;
    public float movementForceInAir=50f;
    public float airDragMultiplier = 0.95f;
    public float variableJumpHeightMultiplier = 0.4f;
    private float jumpTimer;
    public float jumpTimerSet = 0.15f;
    public float wallCheckDistance=-0.5f;
    public float wallSlideSpeed=4;
    public float wallHopForce=10f;
    public float wallJumpForce=35f;
    private float turnTimer = 0.15f;
    public float turnTimerSet = 0.05f;
    private float wallJumpTimer;
    public float wallJumpTimerSet = 0.5f;
    public float fallingGravity = 20f;
    public float fastFallingGravity = 30f;
    public float jumpFall=500f;
    public float obstacleCheckHeight=1.2f;
    public float obstacleCheckDistance=-0.5f;
    public float accelerationTime = 0.5f;
    public float deccelerationTime = 0.5f;
    private float timeSinceAccelerated;
    private float timeSinceDeccelerated;

    private int facingDirection = -1;
   // public int amontOfJumps = 2;
   //private int amontOfJumpsLeft=2;
    private int lastWallJumpDirection;

    private bool isFacingRight = false;
    private bool canMove;
    private bool canFlip;
    private bool isGrounded;
    private bool canNormalJump;
    private bool canWallJump;
    private bool isAttemptingToJump;
    private bool checkJumpMultiplier;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool hasWallJumped;
    public bool isJumping;
    private bool falled=false;
    private bool fastFall = false;
    public bool isMoving = false;
    public bool isTouchingObstacle;
    public bool isTouchingObstacleDown;

    private bool haveJump;
    private bool haveDoubleJump;
    //private bool haveWallJump;


    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;
    
    private Rigidbody2D rb;
    private Animator anim;
    public Transform groundCheck;
    public Transform wallCheck;
    public Transform obstacleCheck;
    public Transform obstacleCheckDown;

    public LayerMask whatIsGround;

    public AnimationCurve acceleration = AnimationCurve.EaseInOut(0, 0, 0.75f, 1);
    public AnimationCurve decceleration = AnimationCurve.EaseInOut(0, 1, 2, 0);

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        //amontOfJumpsLeft = amontOfJumps;
        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();
    }

    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        UpdateAnimation();
        CheckIfCanJump();
        CheckIfWallSliding();
        CheckJump();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSurroundings();
        CheckWall();
    }

    private void CheckIfWallSliding()
    {
        if(isTouchingWall && movementInputDirection != facingDirection && rb.velocity.y < 0)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false; 
        }
    }

    private void CheckMovementDirection()
    {
        if(isFacingRight && movementInputDirection < 0)
        {
            Flip();
        }
        else if (!isFacingRight && movementInputDirection > 0)
        {
            Flip();
        }
    }

    private void UpdateAnimation()
    {
        anim.SetBool("isWalking", isMoving);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isTouchingWall", isTouchingWall);
    }

    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            if (isTouchingWall && movementInputDirection!=0 && movementInputDirection != facingDirection)
            {
                WallJump();
            }
            else if (isGrounded || haveJump || haveDoubleJump) //(amontOfJumpsLeft > 0 && isTouchingWall) || amontOfJumpsLeft>0)
            {
                NormalJump();
            }
            else
            {
                Debug.Log("JumpTimer");
                jumpTimer = jumpTimerSet;
                isAttemptingToJump = true;
            }
        }

        if(Input.GetButtonDown("Horizontal") && isTouchingWall)
        {
            if(/*!isGrounded &&*/ movementInputDirection != facingDirection)
            {
                Debug.Log("wallFlip");
                canMove = false;
                canFlip = false;

                turnTimer = turnTimerSet;
            }
        }

        if (!canMove)
        {
            turnTimer -= Time.deltaTime;

            if (turnTimer <= 0)
            {
                canMove = true;
                canFlip = true;
            }
        }

        if (checkJumpMultiplier && !Input.GetButton("Jump"))
        {
            checkJumpMultiplier = false;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier);
        }
    }

    private void ApplyMovement()
    {
        if (movementInputDirection==0 && isGrounded && !isTouchingWall)
        {
            timeSinceAccelerated = 0;
            timeSinceDeccelerated += Time.deltaTime;
        }
        else if(movementInputDirection!=0 && isGrounded && !isTouchingWall)
        {
            timeSinceAccelerated += Time.deltaTime;
            timeSinceDeccelerated = 0;
        }

        float accelerationMultiplier = 1;
        if (accelerationTime > 0)
            accelerationMultiplier = acceleration.Evaluate(timeSinceAccelerated / accelerationTime);

        float deccelerationMultiplier = 1;
        if (deccelerationTime > 0)
            deccelerationMultiplier = decceleration.Evaluate(timeSinceDeccelerated / deccelerationTime);

        if (!isGrounded && !isWallSliding && movementInputDirection == 0)
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
            isMoving = false;
        }
        if (isGrounded && !isWallSliding && movementInputDirection == 0)
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier * deccelerationMultiplier,  rb.velocity.y);
            isMoving = false;
        }
        else if(canMove && movementInputDirection!=0)
        {
            rb.velocity = new Vector2(movementSpeed * movementInputDirection * accelerationMultiplier, rb.velocity.y);
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        
        if (isWallSliding)
        {
            if(rb.velocity.y < wallSlideSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            }
        }

        if (isJumping || (!isGrounded && hasWallJumped))
        {
            if (rb.velocity.y <= 0 && !fastFall)
            {
                //IE Enumerator pour augmenter la vitesse de descente avec le temps ????
                /*rb.velocity = new Vector2(rb.velocity.x, 0.0f);
                Vector2 forceToAdd = new Vector2(rb.velocity.x, jumpFall * -1);
                rb.AddForce(forceToAdd, ForceMode2D.Force);*/
                // StartCoroutine("Falled");
                rb.AddForce(Vector2.down * fallingGravity * rb.mass);
            }

            if(rb.velocity.y <=0 && movementInputDirection == 0)
            {
                fastFall = true;
                rb.AddForce(Vector2.down * fastFallingGravity * rb.mass);
            }
        }
        else
        {
            fastFall = false;
        }
    }

    private void Flip()
    {
        if (!isWallSliding && canFlip)
        {
            facingDirection *= -1;
            isFacingRight = !isFacingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }

    private void CheckJump()
    {
        if (jumpTimer > 0)
        {
            if (isGrounded)
            {
                NormalJump();
            }
        }

        if(isAttemptingToJump)
        {
            jumpTimer -= Time.deltaTime;
        }

        if (wallJumpTimer > 0)
        {
            if(hasWallJumped && movementInputDirection == -lastWallJumpDirection)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0f);
                hasWallJumped = false;
            }
            else if (wallJumpTimer <= 0)
            {
                hasWallJumped = false;
            }
            else
            {
                wallJumpTimer -= Time.deltaTime;
            }
        }
        else if(isGrounded)
        {
            hasWallJumped = false;
        }
    }

    private void NormalJump()
    {
        if (canNormalJump) // && !(isTouchingWall&&facingDirection==movementInputDirection) && !(isTouchingWall&&movementInputDirection==0)) //la partie !(isTouchingWall&&movementInputDirection==0) vient empêcher le joueur de spammer le saut vertical quand sur mur mais rend plus difficil le wall jump
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            if (haveJump == true)
            {
                haveJump = false;
            }
            else if (haveDoubleJump == true)
            {
                haveDoubleJump = false;
            }
            //amontOfJumpsLeft--;
            jumpTimer = 0;
            isAttemptingToJump = false;
            checkJumpMultiplier = true;
            isJumping = true;
        }
    }

    private void WallJump()
    {
        Debug.Log("WallJump");
        if (canWallJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0.0f);
            isWallSliding = false;
            //amontOfJumpsLeft = amontOfJumps;
            //amontOfJumpsLeft--;

            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * movementInputDirection, wallJumpForce * wallJumpDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
            jumpTimer = 0;
            isAttemptingToJump = false;
            checkJumpMultiplier = true;
            turnTimer = 0;
            canMove = true;
            canFlip = true;
            hasWallJumped = true;
            wallJumpTimer = wallJumpTimerSet;
            lastWallJumpDirection = -facingDirection;
        }
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);

        isTouchingObstacle = Physics2D.Raycast(obstacleCheck.position, transform.right, obstacleCheckDistance, whatIsGround);

        isTouchingObstacleDown = Physics2D.Raycast(obstacleCheckDown.position, transform.right, obstacleCheckDistance, whatIsGround);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));

        Gizmos.DrawLine(obstacleCheck.position, new Vector3(obstacleCheck.position.x + obstacleCheckDistance, obstacleCheck.position.y, obstacleCheck.position.z));

        Gizmos.DrawLine(obstacleCheckDown.position, new Vector3(obstacleCheckDown.position.x + obstacleCheckDistance, obstacleCheckDown.position.y, obstacleCheckDown.position.z));
    }

    private void CheckIfCanJump()
    {
        if (isGrounded && rb.velocity.y <= 0.01f)
        {
            //amontOfJumpsLeft = amontOfJumps;
            haveJump = true;
            haveDoubleJump = true;
            isJumping = false;
            falled = false;
        }

        if(isTouchingWall && isGrounded && rb.velocity.y==0 && movementInputDirection == 0)
        {
            canWallJump = false;
            isJumping = false;
            falled = false;
            haveDoubleJump = true;
            haveJump = true;
        }
        else if(isTouchingWall)
        {
            canWallJump = true;
            isJumping = false;
            falled = false;
            haveDoubleJump = false;
            haveJump = false;
        }

        //Patch pour régler le fait qu'on pouvait jump depuis un mur en vertical mais ne règle absolument pas le problème du wallJump //problème maintenant réglé mais je souhaite garder cette aprtie au ca où
        /*if(isTouchingWall && movementInputDirection == 0) 
        {
            haveJump = false;
        }*/

        if (haveDoubleJump==false && haveJump==false) //amontOfJumpsLeft <= 0)
        {
            canNormalJump = false;
        }
        else
        {
            canNormalJump = true;
        }
    }

    /*IEnumerator Falled()
    {
        if (isJumping)
        {
            if (rb.velocity.y <= 0)
            {
                falled = true;
                //IE Enumerator pour augmenter la vitesse de descente avec le temps ????
                rb.velocity = new Vector2(rb.velocity.x, 0.0f);
                Vector2 forceToAdd = new Vector2(rb.velocity.x, jumpFall * -1 - fallingGravity);
                rb.AddForce(forceToAdd, ForceMode2D.Force);
            }
        }
        yield return new WaitForSeconds(5f);
        if (!isGrounded || !isTouchingWall || !isWallSliding)
        {
            StartCoroutine("Falled");
        }
    } */

    private void CheckWall() 
    {
        if (isTouchingObstacle || isTouchingWall || isTouchingObstacleDown)
        {
            isMoving=false;
        }
    }

}
