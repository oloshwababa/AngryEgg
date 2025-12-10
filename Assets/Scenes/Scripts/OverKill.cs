using UnityEngine;

public class OverkillStar : MonoBehaviour
{
    [Header("Speed")]
    public float speedMultiplier = 1.6f;   // 60% faster
    public float speedDuration = 6f;

    [Header("Fire Rate")]
    [Tooltip("Multiplier applied to the fire interval. <1 = faster shooting (0.5 -> twice as fast)")]
    public float fireRateMultiplier = 0.5f;
    public float fireRateDuration = 6f;

    [Header("Feedback (optional)")]
    public AudioClip pickupSound;
    public GameObject pickupVFXPrefab;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;

        // Find the Player component on the collider or any parent object (robust to child-collider setups)
        var player = other.GetComponentInParent<Player>();
        if (player != null)
        {
            // Apply the speed buff via the Player API you already have
            player.ApplySpeedBuff(speedMultiplier, speedDuration);

            // Try to apply the fire rate buff on the player's GameObject.
            // This uses SendMessage so it won't error if the method doesn't exist.
            player.gameObject.SendMessage("ApplyFireRateBuff",
                new object[] { fireRateMultiplier, fireRateDuration }, SendMessageOptions.DontRequireReceiver);

            // Optional feedback
            if (pickupSound != null) AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            if (pickupVFXPrefab != null) Instantiate(pickupVFXPrefab, transform.position, Quaternion.identity);

            Destroy(gameObject);
            return;
        }

        // Fallback: if no Player found on parents, attempt to notify the collider's root upwards (safe no-op if missing).
        other.gameObject.SendMessageUpwards("ApplyFireRateBuff",
            new object[] { fireRateMultiplier, fireRateDuration }, SendMessageOptions.DontRequireReceiver);
    }
}
