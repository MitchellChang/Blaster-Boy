using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    Rigidbody2D rb;
    Animator anim;
    public Transform groundCheck;
    public GameObject Camera;

    public float CameraZoom = -10f;

    //player movement
    [HideInInspector] public bool jump = false;
    [Range(1, 10)]
    public float jumpVelocity;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public float moveForce = 365f;
    public float maxSpeed = 3f;
    private bool grounded = false;


    
    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCamera();
        grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));

        if (Input.GetButton("Jump") && grounded)
        {
            jump = true;
            //anim.SetBool("jumpCheck", true);
        }
    }

    void FixedUpdate()
    {

        //needs serious revamping
        if (jump)
        {
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpVelocity, ForceMode2D.Impulse);
            jump = false;
            //anim.SetBool("jumpCheck", false);
        }
        //needs an else if statement allowing for ^> arrow key jumping, to ensure that the player can constantly jump while running

        //Fall gravity
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        //Debug.Log(rb.velocity.y);
        anim.SetFloat("fallCheck", rb.velocity.y);
        //player somehow maintains a velocity of between 0 and -0.2943, according to the debug log

        rb.MoveRotation(0); //keeps player upright

        //horizontal movement
        float moveX = Input.GetAxis("Horizontal");
        anim.SetFloat("moveCheck", Mathf.Abs(moveX));
        //Debug.Log(moveX);

        //cannot move while midair
        if (grounded)
        {
            if (moveX * rb.velocity.x < maxSpeed)
                rb.AddForce(Vector2.right * moveX * moveForce);
            if (Mathf.Abs(rb.velocity.x) > maxSpeed)
                rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x), rb.velocity.y);
            //if (moveX < 0.3 && moveX > -0.3)
                //rb.velocity = new Vector2(0, rb.velocity.y);
        }

        //flips player sprite to face direction it is moving
        if (moveX > 0)
        {
            rb.GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (moveX < 0)
        {
            rb.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    private void UpdateCamera()
    {
        Camera.transform.position = new Vector3(transform.position.x, transform.position.y + 2, CameraZoom);
    }
}
