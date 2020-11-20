using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO see if we can do cross platform without this library
// TODO better yet, implement the new Unity Input system
//using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    // Configuration Parameters
    [SerializeField] private float runVelocity = 5f;
    [SerializeField] private float jumpVelocity = 28f;
    [SerializeField] private float climbVelocity = 5f;

    // State
    private enum PlayerState
    {
        groundIdle = 0,
        running = 1,
        jumping = 2,
        climbing = 3,
        climbIdle = 4
    }
    private PlayerState currentState = PlayerState.groundIdle;
    private bool bIsAlive = true;
    private bool bIsMovingHorizontal = false;
    private bool bIsMovingVertical = false;
    private bool bIsOnGroundLayer = false;
    private bool bIsOnClimbLayer = false;

    // Cached References
    private Rigidbody2D playerRigidBody;
    private Collider2D playerCollider2D;
    private Animator playerAnimator;
    private float horizontalControlThrow = 0;
    private float verticalControlThrow = 0;
    private Vector2 jumpVelocityToAdd;

    // Message and Methods

    // Start is called before the first frame update
    void Start()
    {
        // setup references
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerCollider2D = GetComponent<Collider2D>();
        playerAnimator = GetComponent<Animator>();
        jumpVelocityToAdd = new Vector2(0f, jumpVelocity);
    }


    // Update is called once per frame
    void Update()
    {
        // values are between -1 and +1
        horizontalControlThrow = Input.GetAxis("Horizontal");
        verticalControlThrow = Input.GetAxis("Vertical");

        // TODO refine this logic. if we set bool here, animaton will change while there is still movement
        /*
         * we should put checks in for running to allow changes only if touching ground
         * also need to decide if we are going to allow moving off ladder
         * 1. allow horizontal move on ladder
         * 2. allow jump from ladder
         * 3. allow grab ladder from jump
         * 4. add climb idle
         */

        //playerRigidBody.velocity = new Vector2(horizontalControlThrow * runVelocity, verticalControlThrow * climbVelocity);

        // we only want direction changes while player is on the ground or ladder, remove this check if movement mid-air is desired
        if (playerCollider2D.IsTouchingLayers(LayerMask.GetMask("L_Ground")) ||
            playerCollider2D.IsTouchingLayers(LayerMask.GetMask("L_Climb")))
        {
            playerRigidBody.velocity = new Vector2(horizontalControlThrow * runVelocity, playerRigidBody.velocity.y);
        }





        //horizontal movement check
        bIsMovingHorizontal = Mathf.Abs(playerRigidBody.velocity.x) > Mathf.Epsilon;

        //flip sprite to running direction
        if (bIsMovingHorizontal) // TODO && not on ladder
        {
            transform.localScale = new Vector2(Mathf.Sign(playerRigidBody.velocity.x), 1f);
        }







        if (verticalControlThrow != 0)
            Climb();

        if (Input.GetButtonDown("Jump"))
            Jump();

        SetAnimation();
    }

    private void ProcessMovement()
    {

    }


    private void Jump()
    {
        // use this check to disable ability to jump while already in the air
        if (playerCollider2D.IsTouchingLayers(LayerMask.GetMask("L_Ground")))
            playerRigidBody.velocity += jumpVelocityToAdd;
    }

    private void Climb()
    {
        if (playerCollider2D.IsTouchingLayers(LayerMask.GetMask("L_Climb")))
        {
            playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, verticalControlThrow * climbVelocity);
        }
            playerAnimator.SetBool("bIsClimbing", true);
    }

    private void SetAnimation()
    {
        /*
         * Animation Settings
         * 1. ground idle: if on ground and no movment
         * 2. ladder idle: if on ladder/climbing and not touching ground and no movement
         * 2. running: if moving horizontal and not on ladder/climbing, this includes jumping
         * 3. climbing motion: if moving and on ladder and not on ground
         */


        if (bIsMovingHorizontal)
            playerAnimator.SetBool("bIsRunning", true);
        else
            playerAnimator.SetBool("bIsRunning", false);
    }

}// end of class Player
