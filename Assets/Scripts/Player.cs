using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    Rigidbody2D rb;
    Animator anim;
    public Transform groundCheck;
    public GameObject Camera;
    private Rigidbody2D formBullet;
    public Rigidbody2D bullets;
    public Rigidbody2D mediumBullets;
    public Rigidbody2D strongBullets;
    public Rigidbody2D windBullets;
    public Rigidbody2D rockBullets;
    public Rigidbody2D fireBullets;
    public Rigidbody2D waterBullets;
    public Transform firePoint;

    public GameObject deathAnim;
    GameObject deathAnim2;
    Animator deathAnimator;


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
    public float fireRate = 0.3f;
    public float timeToFire = 0.3f;
    public float Damage = 10;
    public float bulletSpeed = 10f;
    private bool facingLeft = false;
    public float chargeTime = 0f;
    public float maxHealth = 20f;
    public float currentHealth = 20f;
    public float iFrameLength = 2.5f;
    public float timeHit = 0f;
    public Image Fill;
    public Color MaxHealthColor = Color.blue;
    public Color MedHealthColor = Color.yellow;
    public Color MinHealthColor = Color.red;
    int powerChecker = 0;



    //ABILITIES
    public bool chargeShot = true;
    public bool dash = true;
    public bool armor = true;

    //POWERS
    bool[] powers = new bool[] {true, true, true, true, true};
    //0: Regular
    //1: Water
    //2: Rock
    //3: Wind
    //4: Fire

    public int equipped = 0;

    public Slider HealthBar;
    public Text powerText;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        firePoint = transform.Find("FirePoint");
        if (firePoint == null)
        {
            Debug.LogError("Fire Point not found.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        changePowerUI();
        UpdateCamera();
        grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));

        if (Input.GetButton("Jump") && grounded)
        {
            jump = true;
            //anim.SetBool("jumpCheck", true);
        }


        //Ability swaps down
        if (Input.GetKeyDown(KeyCode.X))
        {
            powerChecker = equipped - 1;
            if (powerChecker < 0)
            {
                powerChecker = 4;
            }
            while (powers[powerChecker] != true)
            {
                powerChecker--;
                if (powerChecker < 0)
                {
                    powerChecker = 4;
                }
            }
            equipped = powerChecker;
            changePowerUI();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            powerChecker = equipped + 1;
            if (powerChecker > 4)
            {
                powerChecker = 0;
            }
            while (powers[powerChecker] != true)
            {
                powerChecker++;
                if (powerChecker > 4)
                {
                    powerChecker = 0;
                }
            }
            equipped = powerChecker;
            changePowerUI();
        }

        //Regular bullet fire
        if (Input.GetKeyDown(KeyCode.Z) && Time.time > timeToFire)
        {
            timeToFire = Time.time + fireRate;
            if (equipped == 0)
            {
                Shoot("Regular");
            }
            else if (equipped == 1)
            {
                Shoot("Water");
            }
            else if (equipped == 2)
            {
                Shoot("Rock");
            }
            else if (equipped == 3)
            {
                Shoot("Wind");
            }
            else if (equipped == 4)
            {
                Shoot("Fire");
            }
            Debug.Log("Shot Fired.");
        }



        //Charge shot and regular weapon equipped
        if (chargeShot && equipped == 0)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                chargeTime = Time.time;
            }

            if (Input.GetKeyUp(KeyCode.Z) && ((Time.time - chargeTime) > 1))
            {
                if (((Time.time - chargeTime) > 1.1) && ((Time.time - chargeTime) < 2))
                {
                    chargeTime = Time.time;
                    Shoot("Medium");
                    Debug.LogError("Medium Charge Shot.");
                }
                else if ((Time.time - chargeTime) > 2)
                {
                    chargeTime = Time.time;
                    Shoot("Strong");
                    Debug.LogError("Strong Charge Shot.");
                }
            }
            else if (Input.GetKeyUp(KeyCode.Z))
            {
                if (Time.time > timeToFire)
                {
                    timeToFire = Time.time + fireRate;
                    Shoot("Regular");
                    Debug.LogError("Regular pre-charge shot Fired.");
                }
                chargeTime = 0;
            }
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

        //flips player scale to face direction it is moving
        if (moveX > 0)
        {
            rb.GetComponent <Transform>().localScale = new Vector3(0.3615583f, 0.3615583f, 0.3615583f);
            mediumBullets.GetComponent<Transform>().localScale = new Vector3(3.734925f, 3.734925f, 3.734925f);
            strongBullets.GetComponent<Transform>().localScale = new Vector3(4.08799f, 4.08799f, 4.08799f);
            windBullets.GetComponent<Transform>().localScale = new Vector3(1.740225f, 1.779703f, 1f);
            rockBullets.GetComponent<Transform>().localScale = new Vector3(1.474272f, 1.242966f, 1f);
            fireBullets.GetComponent<Transform>().localScale = new Vector3(2.957596f, 2.91796f, 1f);
            waterBullets.GetComponent<Transform>().localScale = new Vector3(2.692935f, 2.656751f, 1f);
            facingLeft = false;
        }
        else if (moveX < 0)
        {
            rb.GetComponent<Transform>().localScale = new Vector3(-0.3615583f, 0.3615583f, 0.3615583f);
            mediumBullets.GetComponent<Transform>().localScale = new Vector3(-3.734925f, 3.734925f, 3.734925f);
            strongBullets.GetComponent<Transform>().localScale = new Vector3(-4.08799f, 4.08799f, 4.08799f);
            windBullets.GetComponent<Transform>().localScale = new Vector3(-1.740225f, 1.779703f, 1f);
            rockBullets.GetComponent<Transform>().localScale = new Vector3(-1.474272f, 1.242966f, 1f);
            fireBullets.GetComponent<Transform>().localScale = new Vector3(-2.957596f, 2.91796f, 1f);
            waterBullets.GetComponent<Transform>().localScale = new Vector3(-2.692935f, 2.656751f, 1f);
            facingLeft = true;
        }
    }

    private void UpdateCamera()
    {
        Camera.transform.position = new Vector3(transform.position.x, transform.position.y + 2, CameraZoom);
    }

    void Shoot(string shotType)
    {
        if (shotType == "Regular")
        {
            formBullet = Instantiate(bullets, firePoint.position, firePoint.rotation) as Rigidbody2D;
        }
        else if (shotType == "Medium")
        {
            formBullet = Instantiate(mediumBullets, firePoint.position, firePoint.rotation) as Rigidbody2D;
        }
        else if (shotType == "Strong")
        {
            formBullet = Instantiate(strongBullets, firePoint.position, firePoint.rotation) as Rigidbody2D;
        }
        else if (shotType == "Wind")
        {
            formBullet = Instantiate(windBullets, firePoint.position, firePoint.rotation) as Rigidbody2D;
        }
        else if (shotType == "Rock")
        {
            formBullet = Instantiate(rockBullets, firePoint.position, firePoint.rotation) as Rigidbody2D;
        }
        else if (shotType == "Fire")
        {
            formBullet = Instantiate(fireBullets, firePoint.position, firePoint.rotation) as Rigidbody2D;
        }
        else if (shotType == "Water")
        {
            formBullet = Instantiate(waterBullets, firePoint.position, firePoint.rotation) as Rigidbody2D;
        }

        if (facingLeft)
        {
            formBullet.velocity = bulletSpeed * formBullet.transform.right * -1;
            
        }
        else if (!facingLeft)
        {
            formBullet.velocity = bulletSpeed * formBullet.transform.right;
            
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        string collisionName = collision.gameObject.name.Substring(0,3);
        //Tests fir enemy hits
        if ((Time.time - timeHit) > iFrameLength)
        {
            if (collisionName == "Gro" || collisionName == "Air")
            {
                DealDamage(5);
                Debug.Log(currentHealth);
            }
            //Tests for boss hits, "boss" name is just temporary until boss is implemented
            if (collisionName == "boss")
            {
                DealDamage(7);
            }
        }

        if (collisionName == "Hea")
        {
            Destroy(collision.gameObject);
            GiveHealth(10);
        }
        if (collisionName == "Win")
        {
            maxHealth += 5;
            GiveHealth(50);
            Debug.Log(currentHealth);
            Destroy(collision.gameObject);
        }
        if (currentHealth/maxHealth > 0.5)
        {
            Fill.color = Color.Lerp(MedHealthColor, MaxHealthColor, currentHealth / maxHealth);
        }
        else
        {
            Fill.color = Color.Lerp(MinHealthColor, MedHealthColor, currentHealth / maxHealth);
        }
    }

    void DealDamage(float damageValue)
    {
        currentHealth -= damageValue;
        HealthBar.value = CalculateHealth();
        timeHit = Time.time;
        if (currentHealth <= 0)
        {
            Die();
        }

    }

        void GiveHealth(float healValue)
    {
        currentHealth += healValue;
        HealthBar.value = CalculateHealth();
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
            HealthBar.value = CalculateHealth();
        }
    }

    float CalculateHealth()
    {
        return currentHealth / maxHealth;
    }

    void Die()
    {
        currentHealth = 0;
        Destroy(gameObject);
        deathAnim2 = Instantiate(deathAnim, transform.position, Quaternion.identity);
        deathAnimator = deathAnim2.GetComponent<Animator>();
        Destroy(deathAnim2, deathAnimator.runtimeAnimatorController.animationClips[0].length + 5);
    }

    void changePowerUI()
    {
        if (equipped == 0)
        {
            powerText.GetComponent<Text>().text = "Blaster";
            Color tempcolor = new Color(0, 223, 255);
            powerText.GetComponent<Text>().color = tempcolor;
        }
        else if (equipped == 1)
        {
            powerText.GetComponent<Text>().text = "Water";
            Color tempcolor = Color.blue;
            powerText.GetComponent<Text>().color = tempcolor;
        }
        else if (equipped == 2)
        {
            powerText.GetComponent<Text>().text = "Rock";
            Color tempcolor = Color.gray;
            powerText.GetComponent<Text>().color = tempcolor;
        }
        else if (equipped == 3)
        {
            powerText.GetComponent<Text>().text = "Wind";
            Color tempcolor = Color.white;
            powerText.GetComponent<Text>().color = tempcolor;
        }
        else if (equipped == 4)
        {
            powerText.GetComponent<Text>().text = "Fire";
            Color tempcolor = Color.red;
            powerText.GetComponent<Text>().color = tempcolor;
        }
    }

}
