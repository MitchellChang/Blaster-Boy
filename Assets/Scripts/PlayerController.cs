using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    Rigidbody2D playerRigid;
    public float camZoom = -10f;

    //Player Movement
    public float playerSpeed = 4f;
    public float jumpForce = 500f;


    public bool isGrounded;

	// Use this for initialization
	void Start () {
        playerRigid = GetComponent<Rigidbody2D>();
        isGrounded = true;
	}
	
	
	void FixedUpdate () {
        //horizontal movement
        float moveX = Input.GetAxis("Horizontal");
        playerRigid.velocity = new Vector2(moveX * playerSpeed, 0);

        //flips player sprite to face direction it is moving
        if (moveX > 0)
        {
            playerRigid.GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (moveX < 0)
        {
            playerRigid.GetComponent<SpriteRenderer>().flipX = true;
        }

        //jump movement
        if (Input.GetKey("up") && isGrounded)
        {
            Debug.Log("why won't you jump");
            playerRigid.AddForce(Vector2.up * jumpForce);
        }
	}


    void OnTriggerEnter2D(Collider2D collision)
    {

    }
}
