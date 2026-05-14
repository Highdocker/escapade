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

    [Header("Explode")]
    [SerializeField] bool explodeOnDeath;
    [SerializeField] float deleteSpawnerTime;
    [HideInInspector] float deletionTimer;

    float[] rotations;
    private const string EnemyBulletPoolKey = "EnemyBullet";

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

            // Old method, caused projectileoverlapping. Left here for future debugging.
            //
            //var fraction = (float)i / ((float)numBullets - 1);
            //var difference = maxrotation - minRotation;
            //rotations[i] = minRotation + fraction * difference;
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

            // Try get from pool
            instance = ObjectPooler.DequeueObject<EnemyBullet>(EnemyBulletPoolKey);

            // Configure the bullet instance BEFORE activating
            instance.transform.position = transform.position;

            // Important: pass spawner rotation + per-bullet offset so direction is relative to spawner
            float spawnRotation = spawnerZ + rotations[i];
            // keep angle normalized if you prefer:
            // spawnRotation = Mathf.Repeat(spawnRotation, 360f);

            instance.Initialize(spawnRotation, bulletSpeed, bulletVelocity, spawnedBulletLifetime);

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
