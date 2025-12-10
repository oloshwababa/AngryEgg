using UnityEngine;
using System.Collections;

public class HealthPickupSpawner : MonoBehaviour
{
    [Header("Orb Prefab")]
    [SerializeField] GameObject orbPrefab; // assign your orb prefab here

    [Header("Spawn Timing")]
    [SerializeField] float minSpawnDelay = 3f;
    [SerializeField] float maxSpawnDelay = 15f;

    [Header("Spawn Area")]
    [SerializeField] float spawnRadius = 5f;

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);

            SpawnOrb();
        }
    }

    void SpawnOrb()
    {
        if (orbPrefab == null)
        {
            Debug.LogError("No orb prefab assigned to HealthOrbSpawner!");
            return;
        }

        Vector2 randomPos = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
        Instantiate(orbPrefab, randomPos, Quaternion.identity);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
