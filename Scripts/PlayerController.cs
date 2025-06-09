using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Controls { mobile, pc }

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float doubleJumpForce = 8f;
    public LayerMask groundLayer;
    public Transform groundCheck;

    private Rigidbody2D rb;
    private bool isGroundedBool = false;
    private bool canDoubleJump = false;

    public Animator playeranim;
    public Controls controlmode;

    private float moveX;
    public bool isPaused = false;

    public ParticleSystem footsteps;
    private ParticleSystem.EmissionModule footEmissions;
    public ParticleSystem ImpactEffect;
    private bool wasonGround;

    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

    // ✅ AudioSources
    public AudioSource bgmAudio;
    public AudioSource sfxAudio;

    // ✅ AudioClips
    public AudioClip jumpSound;
    public AudioClip coinPickupSound;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        footEmissions = footsteps.emission;

        if (controlmode == Controls.mobile)
        {
            UIManager.instance.EnableMobileControls();
        }

        // ✅ Play BGM
        if (bgmAudio != null && !bgmAudio.isPlaying)
        {
            bgmAudio.loop = true;
            bgmAudio.Play();
        }
    }

    private void Update()
    {
        isGroundedBool = IsGrounded();

        if (isGroundedBool)
        {
            canDoubleJump = true;

            if (controlmode == Controls.pc)
            {
                moveX = Input.GetAxis("Horizontal");
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.localScale = new Vector2(-1, 1);
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.localScale = new Vector2(1, 1);
            }

            if (Input.GetButtonDown("Jump"))
            {
                Jump(jumpForce);
            }
        }
        else
        {
            if (canDoubleJump && Input.GetButtonDown("Jump"))
            {
                Jump(doubleJumpForce);
                canDoubleJump = false;
            }
        }

        if (!isPaused)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 lookDirection = mousePosition - transform.position;
            float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;

            if (controlmode == Controls.pc && Input.GetButtonDown("Fire1") && Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + 1f / fireRate;
            }
        }

        SetAnimations();

        if (moveX != 0)
        {
            FlipSprite(moveX);
        }

        if (!wasonGround && isGroundedBool)
        {
            ImpactEffect.gameObject.SetActive(true);
            ImpactEffect.Stop();
            ImpactEffect.transform.position = new Vector2(footsteps.transform.position.x, footsteps.transform.position.y - 0.2f);
            ImpactEffect.Play();
        }

        wasonGround = isGroundedBool;
    }

    public void SetAnimations()
    {
        if (moveX != 0 && isGroundedBool)
        {
            playeranim.SetBool("run", true);
            footEmissions.rateOverTime = 35f;
        }
        else
        {
            playeranim.SetBool("run", false);
            footEmissions.rateOverTime = 0f;
        }

        playeranim.SetBool("isGrounded", isGroundedBool);
    }

    private void FlipSprite(float direction)
    {
        transform.localScale = new Vector3(direction > 0 ? 1 : -1, 1, 1);
    }

    private void FixedUpdate()
    {
        if (controlmode == Controls.pc)
        {
            moveX = Input.GetAxis("Horizontal");
        }

        rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);
    }

    private void Jump(float jumpForce)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        playeranim.SetTrigger("jump");

        // ✅ Play Jump Sound
        if (sfxAudio && jumpSound)
        {
            sfxAudio.PlayOneShot(jumpSound);
        }
    }

    private bool IsGrounded()
    {
        float rayLength = 0.25f;
        Vector2 rayOrigin = new Vector2(groundCheck.transform.position.x, groundCheck.transform.position.y - 0.1f);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, groundLayer);
        return hit.collider != null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("killzone"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else if (collision.CompareTag("coin"))
        {
            // ✅ Play Coin Sound
            if (sfxAudio && coinPickupSound)
            {
                sfxAudio.PlayOneShot(coinPickupSound);
            }

            Destroy(collision.gameObject);
            GameManager.instance.IncrementCoinCount();
        }
    }

    public void MobileMove(float value)
    {
        moveX = value;
    }

    public void MobileJump()
    {
        if (isGroundedBool)
        {
            Jump(jumpForce);
        }
        else if (canDoubleJump)
        {
            Jump(doubleJumpForce);
            canDoubleJump = false;
        }
    }

    public void Shoot()
    {
        // Add projectile logic here
    }

    public void MobileShoot()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }
}
