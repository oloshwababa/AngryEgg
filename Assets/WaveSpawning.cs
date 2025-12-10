using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class Wave
{
    public string waveName;
    public int numberOfEnemies;
    public GameObject[] typeOfEnemies;
    public float spawnInterval;
}

public class WaveSpawning : MonoBehaviour
{
    public Wave[] waves;
    public Transform[] spawnPoints;
    public Animator animator;
    public Text waveName;

    private Wave currentWave;
    private int currentWaveNumber = 0;
    private float nextSpawnTime = 0f;
    private bool canSpawn = true;
    private bool canAnimate = false;

    private void Start()
    {
        // safety
        if (waves == null || waves.Length == 0)
        {
            Debug.LogError("No waves set up in inspector!");
            enabled = false;
            return;
        }
        currentWave = waves[currentWaveNumber];
        UpdateWaveUI();
    }

    private void Update()
    {
        // Make sure currentWaveNumber is valid
        if (currentWaveNumber >= waves.Length) return;

        currentWave = waves[currentWaveNumber];

        // Spawn enemies for the current wave
        SpawnWave();

        // Check if all enemies are dead and wave finished spawning
        GameObject[] totalEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (totalEnemies.Length == 0 && currentWaveNumber + 1 != waves.Length && canAnimate)
        {
            if (currentWaveNumber + 1 != waves.Length && canAnimate)
            {
                if (canAnimate)
                {
                    waveName.text = waves[currentWaveNumber + 1].waveName;
                    animator.SetTrigger("WaveComplete");
                    canAnimate = false;
                }
                
            }
        }
        else
        {
            Debug.Log("GameFinish");

        }
    }

    public void SpawnNextWave()
    {
        currentWaveNumber++;

        // clamp to avoid going out of bounds (defensive)
        if (currentWaveNumber >= waves.Length)
        {
            currentWaveNumber = waves.Length; // disables further spawning
            return;
        }

        currentWave = waves[currentWaveNumber];
        canSpawn = true;
        nextSpawnTime = Time.time + 0.5f; // small buffer before first spawn

        // update UI/animator if needed
        UpdateWaveUI();
        if (animator != null)
        {
            animator.SetTrigger("NewWave"); // ensure you have this trigger or change as needed
        }
    }

    void SpawnWave()
    {
        if (!canSpawn) return;
        if (currentWave == null) return;
        if (currentWave.numberOfEnemies <= 0)
        {
            canSpawn = false;
            canAnimate = true;
            return;
        }
        if (nextSpawnTime > Time.time) return;

        // choose random enemy and spawn point
        GameObject randomEnemy = currentWave.typeOfEnemies[Random.Range(0, currentWave.typeOfEnemies.Length)];
        Transform randomSpawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(randomEnemy, randomSpawn.position, Quaternion.identity);

        currentWave.numberOfEnemies--;
        nextSpawnTime = Time.time + currentWave.spawnInterval;

        if (currentWave.numberOfEnemies <= 0)
        {
            canSpawn = false;
        }
    }

    private void UpdateWaveUI()
    {
        if (waveName != null && currentWave != null)
            waveName.text = currentWave.waveName;
    }
}
