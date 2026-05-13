using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] public GameObject bulletPrefab;
    [SerializeField] public float minRotation;
    [SerializeField] public float maxrotation;
    [SerializeField] public int numBullets;
    [SerializeField] bool isRandom;
    [SerializeField] public float cooldown;
    [HideInInspector] public float timer;
    [SerializeField] public float bulletSpeed;
    [SerializeField] public Vector2 bulletVelocity;
    [SerializeField] public float spawnedBulletLifetime = 20f;

    float[] rotations;
    private const string EnemyBulletPoolKey = "EnemyBullet";

    private void Start()
    {
        timer = cooldown;
        rotations = new float[Mathf.Max(1, numBullets)];

        if (!isRandom)
            DistributedRotations();

        if (bulletPrefab == null)
            Debug.LogWarning("BulletSpawner: bulletPrefab not assigned.");
    }

    void Update()
    {
        if (timer <= 0)
        {
            SpawnBullets();
            timer = cooldown;
        }
        timer -= Time.deltaTime;
    }

    public float[] RandomRotations()
    {
        for (int i = 0; i < numBullets; i++)
            rotations[i] = Random.Range(minRotation, maxrotation);
        return rotations;
    }

    public float[] DistributedRotations()
    {
        for (int i = 0; i < numBullets; i++)
        {
            var fraction = (float)i / ((float)numBullets - 1);
            var difference = maxrotation - minRotation;
            rotations[i] = minRotation + fraction * difference;
        }
        return rotations;
    }

    public GameObject[] SpawnBullets()
    {
        if (isRandom) RandomRotations();

        GameObject[] spawnedBullets = new GameObject[numBullets];

        for (int i = 0; i < numBullets; i++)
        {
            EnemyBullet instance = null;

            // Try get from pool
            instance = ObjectPooler.DequeueObject<EnemyBullet>(EnemyBulletPoolKey);

            // Configure the bullet instance BEFORE activating
            instance.transform.position = transform.position;

            // Use the new Initialize method to set all runtime state and apply rotation on the transform
            instance.Initialize(rotations[i], bulletSpeed, bulletVelocity, spawnedBulletLifetime);

            // Activate after configuration
            instance.gameObject.SetActive(true);

            spawnedBullets[i] = instance.gameObject;
        }

        return spawnedBullets;
    }
}
