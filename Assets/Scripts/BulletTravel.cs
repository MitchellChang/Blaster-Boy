using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTravel : MonoBehaviour {

    private Rigidbody2D rb;
    public float bulletSpeed = 10f;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        rb.velocity = new Vector2 (bulletSpeed, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy (this.gameObject);
    }
}
