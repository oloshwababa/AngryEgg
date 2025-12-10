using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] int healthRestore = 20;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            bool wasHealed = player.Heal(healthRestore); // returns true if any HP was added
            if (wasHealed)
                Destroy(gameObject);
        }
    }
}
