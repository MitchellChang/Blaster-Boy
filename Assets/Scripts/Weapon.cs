using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {
    public float fireRate = 0;
    public float Damage = 10;
    public LayerMask notToHit;
    public Transform bulletSpawn;
    public GameObject bullets;

    float timeToFire = 0;
    Transform firePoint;

	// Use this for initialization
	void Awake () {
        firePoint = transform.Find("FirePoint");
        if (firePoint == null)
        {
            Debug.LogError("Fire Point not found.");
        }
	}
	
	// Update is called once per frame
	void Update () {
		if (fireRate == 0)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                Shoot();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Z) && Time.time > timeToFire)
            {
                timeToFire = Time.time + fireRate;
                Shoot();
            }
        }
	}

    void Shoot()
    {
        Instantiate(bullets, firePoint.position, Quaternion.identity);
    }
}
