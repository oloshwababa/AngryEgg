using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI healthText; // Player's health bar
    [SerializeField] float moveSpeed = 9;
    Animator anim;
    Rigidbody2D rb;


    int maxHealth = 100;
    int currentHealth;

    bool dead = false;

    // add near other fields
    float baseMoveSpeed;
    Coroutine speedBuffCoroutine;


    float moveHorizontal, moveVertical;
    Vector2 movement;

    int facingDirection = 1; //1 = right, -1 = left

    public void ApplySpeedBuff(float multiplier, float duration)
    {
        // stop any existing buff so we don't leak coroutines or permanently change speed
        if (speedBuffCoroutine != null)
            StopCoroutine(speedBuffCoroutine);

        speedBuffCoroutine = StartCoroutine(SpeedBuffCoroutine(multiplier, duration));
    }

    private System.Collections.IEnumerator SpeedBuffCoroutine(float multiplier, float duration)
    {
        baseMoveSpeed = moveSpeed;                 // remember current speed
        moveSpeed = baseMoveSpeed * multiplier;    // apply buff

        yield return new WaitForSeconds(duration); // wait

        moveSpeed = baseMoveSpeed;                 // restore original
        speedBuffCoroutine = null;
    }


    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;
        healthText.text = maxHealth.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Hit(10);

        if (dead)
        {
            movement = Vector2.zero;
            anim.SetFloat("Velocity", 0);
            return;
        }

        moveHorizontal = Input.GetAxisRaw("Horizontal");
        moveVertical = Input.GetAxisRaw("Vertical");

        movement = new Vector2(moveHorizontal, moveVertical).normalized;

        anim.SetFloat("velocity", movement.magnitude); //sets the running animation

        if (movement.x != 0)
            facingDirection = movement.x > 0 ? 1 : -1;

        transform.localScale = new Vector2(facingDirection, 1);
    }
    private void FixedUpdate()
    {
        rb.linearVelocity = movement * moveSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();

        if (enemy != null)
            Hit(10);
    }

    void Hit(int damage)
    {
        anim.SetTrigger("hit");
        currentHealth -= damage;
        healthText.text = Mathf.Clamp(currentHealth, 0, maxHealth).ToString();

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        dead = true;
        GameManager.instance.GameOver();
    }

    public bool Heal(int healthRestore)
    {
        // only heal if player is alive and not already at max
        if (!dead && currentHealth < maxHealth)
        {
            int missingHealth = Mathf.Max(maxHealth - currentHealth, 0);
            int actualRestore = Mathf.Min(missingHealth, healthRestore);

            if (actualRestore <= 0)
                return false;

            currentHealth += actualRestore;
            // ensure we never go above max (extra safety)
            currentHealth = Mathf.Min(currentHealth, maxHealth);

            // update UI
            if (healthText != null)
                healthText.text = currentHealth.ToString();

            return true; // we healed something
        }

        return false; // could not heal
    }
}





