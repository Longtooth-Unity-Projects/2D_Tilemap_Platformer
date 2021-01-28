using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO see if we can do cross platform without this library
// TODO better yet, implement the new Unity Input system
//using UnityStandardAssets.CrossPlatformInput;


//TODO adjust movement so there is less movement with one keypress


public class Player : MonoBehaviour
{
    // Configuration Parameters
    [SerializeField] private float runVelocity = 5f;
    [SerializeField] private float jumpVelocity = 28f;
    [SerializeField] private float climbVelocity = 5f;
    [SerializeField] private float deathFlyVelocity = 10f;

    // State
    private enum PlayerState
    {
        groundIdle = 0,
        running = 1,
        jumping = 2,
        climbing = 3,
        climbIdle = 4,
        dead = 5
    }
    private PlayerState currentState = PlayerState.groundIdle;
    private PlayerState lastState = PlayerState.groundIdle;
    [SerializeField] private bool bIsAlive = true;  //TODO serialized for debugging
    private bool bIsOnGroundLayer = false;
    private bool bIsOnClimbLayer = false;
    private bool bIsMovingHorizontal = false;
    private bool bIsMovingVertical = false;


    // Cached References
    private Rigidbody2D playerRigidBody;
    private BoxCollider2D playerBodyCollider2D;
    private CapsuleCollider2D playerFeetCollider2D;
    private Animator playerAnimator;
    private float startingGravity;
    private float horizontalControlThrow = 0;
    private float verticalControlThrow = 0;
    private Vector2 jumpVelocityToAdd;

    // Message and Methods

    // Start is called before the first frame update
    void Start()
    {
        // setup references
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerBodyCollider2D = GetComponent<BoxCollider2D>();
        playerFeetCollider2D = GetComponent<CapsuleCollider2D>();
        playerAnimator = GetComponent<Animator>();
        startingGravity = playerRigidBody.gravityScale;
        jumpVelocityToAdd = new Vector2(0f, jumpVelocity);
    }


    // Update is called once per frame
    void Update()
    {
        if (bIsAlive)
        {
            ProcessMovement();
            DeathCheck();
            DetermineAnimation();
        }
        else
        {
            playerRigidBody.velocity = new Vector2(0, playerRigidBody.velocity.y);
        }

    }// end of Update method


    private void ProcessMovement()
    {
        // values are between -1 and +1
        horizontalControlThrow = Input.GetAxis("Horizontal");
        verticalControlThrow = Input.GetAxis("Vertical");

        bIsOnGroundLayer = playerFeetCollider2D.IsTouchingLayers(LayerMask.GetMask("L_Ground"));
        bIsOnClimbLayer = playerFeetCollider2D.IsTouchingLayers(LayerMask.GetMask("L_Climb"));

        // process movement
        if (bIsOnClimbLayer)
        {
            //we only want to stop gravity if player is already up ladder
            if (!bIsOnGroundLayer)
                playerRigidBody.gravityScale = 0;

            if (verticalControlThrow != 0)
                playerRigidBody.velocity = new Vector2(horizontalControlThrow * runVelocity, verticalControlThrow * climbVelocity);
            else
                //if we set it to 0, player sticks to ladder when jumps, if we use playerRigidBody.velocity.y, player drifts slowly
                playerRigidBody.velocity = new Vector2(horizontalControlThrow * runVelocity, 0);
        }
        else if (bIsOnGroundLayer)
        {
            playerRigidBody.gravityScale = startingGravity;
            playerRigidBody.velocity = new Vector2(horizontalControlThrow * runVelocity, playerRigidBody.velocity.y);
        }
        else
        { //let gravity handle it
            playerRigidBody.gravityScale = startingGravity;
        }


        if (Input.GetButtonDown("Jump") && bIsOnGroundLayer)
        {
            playerRigidBody.velocity += jumpVelocityToAdd;
        }
    }// end of method ProcessMovement

    private void DeathCheck()
    {
        if (playerBodyCollider2D.IsTouchingLayers(LayerMask.GetMask("L_Enemy", "L_Hazard")) ||
            playerFeetCollider2D.IsTouchingLayers(LayerMask.GetMask("L_Enemy", "L_Hazard"))
            )
        {
            bIsAlive = false;
            playerRigidBody.gravityScale = startingGravity;
            playerRigidBody.velocity = new Vector2(0f, deathFlyVelocity);
            FindObjectOfType<GameManagerSingleton>().processPlayerDeath();
        }
    }


    private void DetermineAnimation()
    {
        // velocity checks
        bIsMovingHorizontal = Mathf.Abs(playerRigidBody.velocity.x) > Mathf.Epsilon;
        bIsMovingVertical = Mathf.Abs(playerRigidBody.velocity.y) > Mathf.Epsilon;

        // keep animation state assignemnt seperate from movement calculations for readability
        if (!bIsAlive)
        {
            currentState = PlayerState.dead;
        }
        else if (bIsOnClimbLayer && !bIsOnGroundLayer)
        {
            if (!bIsMovingHorizontal && !bIsMovingVertical)
                currentState = PlayerState.climbIdle;
            else if (verticalControlThrow != 0)
                currentState = PlayerState.climbing;
            else
                currentState = PlayerState.jumping;
        }
        else if (bIsOnGroundLayer)
        {
            if (bIsMovingHorizontal)
            {
                currentState = PlayerState.running;
            }
            else
                currentState = PlayerState.groundIdle;
        }
        else if (!bIsOnClimbLayer && !bIsOnGroundLayer)
        {
            currentState = PlayerState.jumping;
        }

        //TODO see if this is more efficient than setting value each iteration
        // only set animation state if there is a change
        if (currentState != lastState)
        {
            playerAnimator.SetInteger("PlayerState", (int)currentState);
            lastState = currentState;
        }

        //flip sprite to movement direction
        if (bIsMovingHorizontal)
            transform.localScale = new Vector2(Mathf.Sign(playerRigidBody.velocity.x), 1f);
    }// end of method DetermineAnimation


    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Colliding: " + collision.gameObject.name);
    }




}// end of class Player





