using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour {

    Rigidbody2D rb;
    Animator anim;

    public Rigidbody2D bossBullet;
    public Transform bulletSpawn;
    public GameObject deathAnim;
    GameObject deathAnim2;
    Animator deathAnimator;

    public Slider HealthBar;
    public float attackDelay = 5f;
    public float bulletSpeed = 4f;
    public float maxHealth = 700f;
    public float currentHealth = 700f;
    private bool shouldAttack = true;


	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        StartCoroutine("BossAttackDelay");
        StartCoroutine("AttackAnimDelay");
	}
	
	// Update is called once per frame
	void Update () {
        if (currentHealth <= 0)
        {
            Destroy(this.gameObject);
            deathAnim2 = Instantiate(deathAnim, transform.position, Quaternion.identity);
            deathAnimator = deathAnim2.GetComponent<Animator>();
            Destroy(deathAnim2, deathAnimator.runtimeAnimatorController.animationClips[0].length);
        }
        DealDamage(0);
    }

    private void BossAttack()
    {
        //instantiate enemy attack
        anim.SetBool("attack", true);
        Rigidbody2D bPrefab = Instantiate(bossBullet, bulletSpawn.position, bulletSpawn.rotation) as Rigidbody2D;
        bPrefab.velocity = bulletSpeed * bPrefab.transform.right * -1;

    }

    private void AttackAnim()
    {
        anim.SetBool("attack", false);
    }

    IEnumerator BossAttackDelay()
    {
        while (shouldAttack)
        {
            BossAttack();

            yield return new WaitForSeconds(attackDelay);
        }
    }

    IEnumerator AttackAnimDelay()
    {
        while (shouldAttack)
        {
            AttackAnim();

            yield return new WaitForSeconds(1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "weakshot(Clone)")
        {
            currentHealth -= 10;
        }

        if (collision.gameObject.name == "medshots(Clone)")
        {
            currentHealth -= 30;
        }

        if (collision.gameObject.name == "strongshots(Clone)")
        {
            currentHealth -= 50;
        }

        if (collision.gameObject.name == "WindAttack(Clone)" || collision.gameObject.name == "RockAttack(Clone)" || collision.gameObject.name == "FireAttack(Clone)" || collision.gameObject.name == "WaterAttack(Clone)")
        {
            currentHealth -= 20;
        }

    }

    void DealDamage(float damageValue)
    {
        currentHealth -= damageValue;
        HealthBar.value = CalculateHealth();

    }

    float CalculateHealth()
    {
        return currentHealth / maxHealth;
    }
}
