using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] public float speed;
    [SerializeField] public float rotation;
    [SerializeField] public float damage;
    [SerializeField] public Vector2 velocity;

    [Header("Bullets Explode")]
    [SerializeField] bool explodeOnDeath;
    [SerializeField] public GameObject childSpawner;

    // Might be temporary, can be removed if map boundaries are added
    [SerializeField] public float lifetime = 20f;
    private float remainingLifetime;

    // Pool key this instance belongs to. Set by pool setup or spawner when dequeued.
    [HideInInspector] public string pooledKey = "EnemyBullet";

    private void OnEnable()
    {
        // Ensure lifetime is reset when enabled; rotation is applied in Initialize
        remainingLifetime = lifetime;
    }
        
    // Accept an optional poolKey so spawners can mark instances with the correct pool.
    public void Initialize(float rotationDeg, float speedValue, Vector2 velocityValue, float lifetimeValue, string poolKey = null)
    {
        rotation = rotationDeg;
        speed = speedValue;
        velocity = velocityValue;
        lifetime = lifetimeValue;

        if (!string.IsNullOrEmpty(poolKey))
            pooledKey = poolKey;

        // Apply rotation immediately so reused instances don't keep old rotation
        transform.rotation = Quaternion.Euler(0f, 0f, rotation);

        // Reset lifetime counter immediately as well
        remainingLifetime = lifetime;
    }

    void Start()
    {
        // Set bullet to the angular rotation (in degrees)
        transform.rotation = Quaternion.Euler(0, 0, rotation);
    }

    void Update()
    {
        // Move the bullet in the direction of its velocity
        transform.Translate(velocity * speed * Time.deltaTime);

        // Count down, and return bullet to Pool when expired
        remainingLifetime -= Time.deltaTime;
        if (remainingLifetime <= 0f)
        {
            // If configured, create a temporary BulletSpawner at this bullet's position
            if (explodeOnDeath)
            {
                var go = Instantiate(childSpawner, transform.position, Quaternion.identity);
                var sp = go.GetComponent<BulletSpawner>();
            }

            // Return to pool using this instance's pool key (not a hardcoded string)
            ObjectPooler.EnqueueObject(this, pooledKey);
        }
    }
    private void OnDisable()
    {
        // Clean up states so they arent carried over when the bullet is reused
        velocity = Vector2.zero;
        speed = 0f;
    }
}
