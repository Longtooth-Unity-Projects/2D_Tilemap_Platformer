using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO see if we can do cross platform without this library
//using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    // Configuration Parameters
    [SerializeField] private float movementSpeed = 5f;

    // State
    private bool bIsAlive = true;
    private bool bIsRunning = false;

    // Cached References
    private Rigidbody2D playerRigidBody;
    private Animator playerAnimator;
    private float controlThrow = 0;

    // Message and Methods

    // Start is called before the first frame update
    void Start()
    {
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        controlThrow = Input.GetAxis("Horizontal");   // value is between -1 and +1
        playerRigidBody.velocity = new Vector2(controlThrow * movementSpeed, playerRigidBody.velocity.y);

        //if moving change animation and flip sprite depending on x velocity value 
        bIsRunning = Mathf.Abs(playerRigidBody.velocity.x) > Mathf.Epsilon;
        if (bIsRunning)
        {
            transform.localScale = new Vector2(Mathf.Sign(playerRigidBody.velocity.x), 1f);
        }
        playerAnimator.SetBool("bIsRunning", bIsRunning);
    }

}// end of class Player
