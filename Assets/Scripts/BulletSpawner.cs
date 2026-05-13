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

    float[] rotations;

    private void Start()
    {
        timer = cooldown;
        rotations = new float[numBullets];

        // Exclusively for random bullet patterns
        if (!isRandom)
        {
            DistributedRotations();
        }
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

    // Returns an array of random rotations between the min and max rotation values
    public float[] RandomRotations()
    {
        for (int i = 0; i < numBullets; i++)
        {
            rotations[i] = Random.Range(minRotation, maxrotation);
        }
        return rotations;
    }
    public float[] DistributedRotations()
    {
        for (int i = 0; i < numBullets; i++)
        {
            var fraction = (float)i / ((float)numBullets - 1);
            var difference = maxrotation - minRotation;
            var fractionOfDifference = fraction * difference;
            rotations[i] = minRotation + fractionOfDifference;
        }
        foreach (var r in rotations) print(r);
        return rotations;
    }

    public GameObject[] SpawnBullets()
    {
        if (isRandom)
        {
            RandomRotations();
        }

        GameObject[] spawnedBullets = new GameObject[numBullets];
        for (int i = 0; i < numBullets; i++)
        {
            spawnedBullets[i] = Instantiate(bulletPrefab, transform);
            var b = spawnedBullets[i].GetComponent<EnemyBullet>();
            b.rotation = rotations[i];
            b.speed = bulletSpeed;
            b.velocity = bulletVelocity;
        }
        return spawnedBullets;
    }
}
