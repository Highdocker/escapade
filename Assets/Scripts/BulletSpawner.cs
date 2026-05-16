using UnityEngine;
using UnityEngine.Rendering;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] public GameObject bulletPrefab;

    [Header("Basic Values")]
    [SerializeField] public float minRotation;
    [SerializeField] public float maxrotation;
    [SerializeField] public int numBullets;
    [SerializeField] public bool isRandom;
    [SerializeField] public float cooldown;
    [HideInInspector] public float timer;
    [SerializeField] public float bulletSpeed;
    [HideInInspector] public Vector2 bulletVelocity = new Vector2(1, 0);
    [SerializeField] public float spawnedBulletLifetime = 20f;

    [Header("Bursts")]
    [SerializeField] public int burstCount;
    [SerializeField] public float burstCooldown;
    [HideInInspector] public int wavesLeftInBurst;
    [HideInInspector] public float burstTimer;

    [Header("Rotating Spawner")]
    [SerializeField] public float rotationSpeed;
    [HideInInspector] public float rotationTimer;

    // Used commonly for children bullets.
    // Essentially means when checked, the spawner/script will delete itself in X seconds.
    [Header("Explode")]
    [SerializeField] bool explodeOnDeath;
    [SerializeField] float deleteSpawnerTime;
    [HideInInspector] float deletionTimer;

    // Pool key to use for this spawner. Set in inspector per spawner.
    [Header("Pooling")]
    [SerializeField] public string poolKey = "EnemyBullet";

    float[] rotations;

    private void Start()
    {
        // For burst spawning
        burstTimer = 0; // Allow burst to fire on first frame
        wavesLeftInBurst = burstCount; // Set first burst round equal to designated amount

        deletionTimer = deleteSpawnerTime;

        rotations = new float[Mathf.Max(1, numBullets)];

        if (!isRandom)
            DistributedRotations();

        if (bulletPrefab == null)
            Debug.LogWarning("BulletSpawner: bulletPrefab not assigned.");

        // Attempt to auto-assign poolKey from the assigned bulletPrefab if user didn't set it.
        if ((string.IsNullOrEmpty(poolKey) || poolKey == "EnemyBullet") && bulletPrefab != null)
        {
            var prefabComp = bulletPrefab.GetComponent<EnemyBullet>();
            if (prefabComp != null)
            {
                foreach (var kvp in ObjectPooler.poolLookup)
                {
                    // Compare the stored prefab component reference to the prefab's component.
                    if (kvp.Value == prefabComp || kvp.Value.name == prefabComp.name)
                    {
                        poolKey = kvp.Key;
                        Debug.Log($"BulletSpawner: auto-assigned poolKey '{poolKey}' for prefab '{bulletPrefab.name}'.");
                        break;
                    }
                }

                if (string.IsNullOrEmpty(poolKey))
                {
                    Debug.LogWarning($"BulletSpawner: could not find a registered pool for prefab '{bulletPrefab.name}'. Set poolKey in inspector or register the prefab in GameManager.");
                }
            }
        }
    }

    void Update()
    {
        if ((timer <= 0 && burstTimer <= 0))
        {
            SpawnBullets();
            // Wave of bullets has occured, so reset cooldown until next wave
            timer = cooldown;
            // If using bursts, reduce wave count by one
            wavesLeftInBurst -= 1;
        }

        if (wavesLeftInBurst <= 0)
        {
            burstTimer = burstCooldown;
            wavesLeftInBurst = burstCount;
        }

        if(explodeOnDeath)
        {
            deletionTimer -= Time.deltaTime;
            if (deletionTimer <= 0f)
            {
                Destroy(gameObject);
            }
        }
        timer -= Time.deltaTime;
        burstTimer -= Time.deltaTime;
    }

    public float[] RandomRotations()
    {
        for (int i = 0; i < numBullets; i++)
            rotations[i] = Random.Range(minRotation, maxrotation);
        return rotations;
    }

    public float[] DistributedRotations()
    {
        float totalAngle = maxrotation - minRotation;
        float anglePerBullet = totalAngle / (float)numBullets;
        for (int i = 0; i < numBullets; i++)
        {
            rotations[i] = minRotation + anglePerBullet * (i + 0.5f); // center of each slice
            rotations[i] = Mathf.Repeat(rotations[i], 360f);
        }
        return rotations;
    }

    public GameObject[] SpawnBullets()
    {
        if (isRandom) RandomRotations();

        GameObject[] spawnedBullets = new GameObject[numBullets];

        // read spawner's current world Z rotation once per spawn batch
        float spawnerZ = transform.eulerAngles.z;

        for (int i = 0; i < numBullets; i++)
        {
            EnemyBullet instance = null;

            // Try get from pool using the configured key
            instance = ObjectPooler.DequeueObject<EnemyBullet>(poolKey);

            if (instance == null)
            {
                Debug.LogWarning($"BulletSpawner: no pooled object available for key '{poolKey}'. Check GameManager.SetupPool and inspector settings.");
                continue;
            }

            // Make sure instance knows its pool key (safeguard for instances created dynamically)
            instance.pooledKey = poolKey;

            // Configure the bullet instance BEFORE activating
            instance.transform.position = transform.position;

            // Important: pass spawner rotation + per-bullet offset so direction is relative to spawner
            float spawnRotation = spawnerZ + rotations[i];

            // Pass poolKey so Initialize can also set pooledKey if needed
            instance.Initialize(spawnRotation, bulletSpeed, bulletVelocity, spawnedBulletLifetime, poolKey);

            // Activate after configuration
            instance.gameObject.SetActive(true);

            spawnedBullets[i] = instance.gameObject;
        }

        return spawnedBullets;
    }

    private void FixedUpdate()
    {
        rotationTimer += Time.fixedDeltaTime;
        transform.rotation = Quaternion.Euler(0, 0, rotationTimer * rotationSpeed);
    }
}
