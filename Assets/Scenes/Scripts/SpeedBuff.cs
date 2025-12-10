using UnityEngine;

public class SpeedStar : MonoBehaviour
{
    [Tooltip("Multiplier applied to player's speed (1.5 = +50%)")]
    public float multiplier = 1.5f;

    [Tooltip("Duration of the speed boost in seconds")]
    public float duration = 5f;

    // optional effects
    public AudioClip pickupSound;
    public GameObject pickupVFXPrefab;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Try to call the Player API
        var player = other.GetComponent<Player>();
        if (player != null)
        {
            player.ApplySpeedBuff(multiplier, duration);

            // optional: play audio
            if (pickupSound != null)
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);

            // optional: spawn particle effect
            if (pickupVFXPrefab != null)
                Instantiate(pickupVFXPrefab, transform.position, Quaternion.identity);

            Destroy(gameObject); // remove the star after pickup
            return;
        }

        // If Player component not found, try to find any script exposing a public ApplySpeedBuff method using SendMessage (fallback)
        // This is optional / less type-safe; uncomment only if needed:
        // other.SendMessage("ApplySpeedBuff", new object[] { multiplier, duration }, SendMessageOptions.DontRequireReceiver);
    }
}
