using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    Rigidbody2D rb;
    public float speed = 1f;
    public float health = 100f;
    public LayerMask enemyMask;
    Transform myTrans;
    float myWidth, myHeight;

	// Use this for initialization
	void Start () {
        myTrans = this.transform;
        rb = this.GetComponent<Rigidbody2D>();
        SpriteRenderer mySprite = this.GetComponent<SpriteRenderer>();
        myWidth = mySprite.bounds.extents.x;
        myHeight = mySprite.bounds.extents.y;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        //casts isGrounded/isBlocked lines
        Vector2 lineCastPos = myTrans.position.toVector2() - myTrans.right.toVector2() * myWidth + Vector2.down * myHeight;

        //checks to see if there is ground in front before moving forward
        Debug.DrawLine(lineCastPos, lineCastPos + Vector2.down);
        bool isGrounded = Physics2D.Linecast(lineCastPos, lineCastPos + Vector2.down, enemyMask);

        //checks to see if there's a wall in front before moving forward
        Debug.DrawLine(lineCastPos, lineCastPos - myTrans.right.toVector2() * 0.01f);
        bool isBlocked = Physics2D.Linecast(lineCastPos, lineCastPos - myTrans.right.toVector2() * 0.01f, enemyMask);

        //turns around if there is no ground or hits a wall
        if (!isGrounded || isBlocked)
        {
            Vector3 currRot = myTrans.eulerAngles;
            currRot.y += 180;
            myTrans.eulerAngles = currRot;
        }

        //moves enemy forward
        Vector2 myVel = rb.velocity;
        myVel.x = -myTrans.right.x * speed;
        rb.velocity = myVel;

        if (health < 1)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "weakshot(Clone)")
        {
            health -= 10;
        }

        if (collision.gameObject.name == "medshots(Clone)")
        {
            health -= 30;
        }

        if (collision.gameObject.name == "strongshots(Clone)")
        {
            health -= 50;
        }

        Debug.LogError(collision.gameObject.name);
    }

}
