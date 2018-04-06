using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    Rigidbody2D playerRigid;
    public float camZoom = -10f;

    //Player Movement
    public float playerSpeed = 4.5f;
    public float jumpForce = 500f;
    public float playerFallSpeed = 1f;

    [HideInInspector] public bool jump = false;
    public bool isGrounded;

	// Use this for initialization
	void Start () {
        playerRigid = gameObject.GetComponent<Rigidbody2D>();
        isGrounded = true;
	}


    void Update() {

        //jump movement
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jump = true;
        }
    }
	
	
	void FixedUpdate () {
        PlayerGravity();

        //horizontal movement
        float moveX = Input.GetAxis("Horizontal");
        playerRigid.velocity = new Vector2(moveX * playerSpeed, 0);

        playerRigid.MoveRotation(0); //keeps player upright

        //flips player sprite to face direction it is moving
        if (moveX > 0)
        {
            playerRigid.GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (moveX < 0)
        {
            playerRigid.GetComponent<SpriteRenderer>().flipX = true;
        }

        if (jump)
        {
            Debug.Log("why won't you jump");
            playerRigid.AddForce(new Vector2(0f, jumpForce));
            jump = false;
        }
	}

    void PlayerGravity()
    {
        Debug.Log(playerRigid.velocity.y);
        if (playerRigid.velocity.y < -0.001)
            playerRigid.AddForce(new Vector2(0, -playerFallSpeed));
    }


    void OnTriggerEnter2D(Collider2D collision)
    {

    }
}
