using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrounded : MonoBehaviour {

    private PlayerController player;

    private void Start()
    {
        player = gameObject.GetComponentInParent<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Ground")
            player.isGrounded = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        player.isGrounded = false;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Ground")
            player.isGrounded = true;
    }
}
