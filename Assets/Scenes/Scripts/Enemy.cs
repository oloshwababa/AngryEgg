using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] int maxHealth = 100;
    [SerializeField] float Speed = 2f;
    [Header("Charger")]

    [SerializeField] bool isCharger;

    [SerializeField] float distanceToCharge = 5f;

    [SerializeField] float chargeSpeed = 12f;

    [SerializeField] float prepareTime = 2f;

    bool isCharging = false;
    bool isPreparingCharge = false;

    private int currentHealth;

    Animator anim;
    Transform target; // Follow target

    // New cached components
    Rigidbody2D rb;
    SpriteRenderer sr;

    private void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        // physics safeties
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void Start()
    {
        // safer: find by tag "Player" (make sure your player GameObject has tag "Player")
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) target = p.transform;
        else
        {
            // fallback to name (keeps original behavior if tag wasn't set)
            GameObject byName = GameObject.Find("Player");
            if (byName != null) target = byName.transform;
            else Debug.LogWarning("Enemy: Player not found. Assign tag 'Player' or name an object 'Player'.");
        }
    }

    // Use FixedUpdate for physics-friendly movement
    private void FixedUpdate()
    {
        if (!WaveManager.Instance.WaveRunning()) return;
        if (isPreparingCharge) return;
        if (target != null)
        {
            Vector3 direction = target.position - transform.position;
            direction.Normalize();

            transform.position += direction * Speed * Time.deltaTime;

            /*var playerToTheRight = target.position.x > transform.position.x;
            transform.localScale = new Vector2(playerToTheRight ? -1 : 1, 1);*/

            if (isCharger &&
              !isCharging &&
              Vector2.Distance(transform.position, target.position) < distanceToCharge)
            {
                isPreparingCharge = true;
                Invoke("StartCharging", prepareTime);
            }
        }
    }

    void StartCharging()
    {
        isPreparingCharge = false;
        isCharging = true;
        Speed = chargeSpeed;
    }
    // This is your existing Hit method (keeps receiving damage unchanged)
    public void Hit(int damage)
    {
        currentHealth -= damage;
        if (anim) anim.SetTrigger("hit");

        if (currentHealth <= 0)
            Destroy(gameObject);
    }

    // ---------- Minimal collision handlers added ----------
    // Handles trigger-based projectile collisions (if projectile collider isTrigger = true)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;

        Debug.Log($"Enemy: OnTriggerEnter2D with {other.name} (tag: {other.tag})");

        // If your projectiles are tagged, handle them
        if (other.CompareTag("Projectile") || other.CompareTag("Bullet"))
        {
            // default damage value; adjust if your projectile provides damage differently
            int damage = 10;
            Hit(damage);

            // destroy the projectile so it doesn't pass through
            Destroy(other.gameObject);
            return;
        }
    }

    // Handles physics collision-based projectiles (isTrigger = false)
    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll == null || coll.collider == null) return;

        Debug.Log($"Enemy: OnCollisionEnter2D with {coll.collider.name} (tag: {coll.collider.tag})");

        if (coll.collider.CompareTag("Projectile") || coll.collider.CompareTag("Bullet"))
        {
            int damage = 10;
            Hit(damage);
            Destroy(coll.collider.gameObject);
            return;
        }
    }
}
