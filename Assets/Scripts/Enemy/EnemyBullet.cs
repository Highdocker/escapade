using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] public float speed;
    [SerializeField] public float rotation;
    [SerializeField] public float damage;
    [SerializeField] public Vector2 velocity;

    // Might be temporary, can be removed if map boundaries are added
    [SerializeField] public float lifetime = 20f;
    private float remainingLifetime;

    private void OnEnable()
    {
        // Ensure lifetime is reset when enabled; rotation is applied in Initialize
        remainingLifetime = lifetime;
    }

    public void Initialize(float rotationDeg, float speedValue, Vector2 velocityValue, float lifetimeValue)
    {
        rotation = rotationDeg;
        speed = speedValue;
        velocity = velocityValue;
        lifetime = lifetimeValue;

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
            ObjectPooler.EnqueueObject(this, "EnemyBullet");
        }
    }
    private void OnDisable()
    {
        // Clean up states so they aren't carried over when the bullet is reused
        velocity = Vector2.zero;
        speed = 0f;
    }
}
