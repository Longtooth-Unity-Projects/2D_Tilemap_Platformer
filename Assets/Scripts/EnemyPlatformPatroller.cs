using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlatformPatroller : MonoBehaviour
{

    // Configuration Parameters
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private int moveDirection = 1;

    // State



    // Cached References
    private Rigidbody2D enemyRigidBody2D;
    private BoxCollider2D enemyCollider2D;

    // Message and Methods




    // Start is called before the first frame update
    void Start()
    {
        enemyRigidBody2D = GetComponent<Rigidbody2D>();
        enemyCollider2D = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        enemyRigidBody2D.velocity = new Vector2(moveSpeed * moveDirection, 0);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //change direction
        moveDirection *= -1;
        transform.localScale = new Vector2(Mathf.Sign(moveDirection), 1f);
    }
}

