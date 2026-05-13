using UnityEngine;

public class PlayerBulletController : MonoBehaviour
{
    [SerializeField] public float speed = 5f;
    [SerializeField] public float lifetime = 5f;
    [SerializeField] public float damage = 1f;

    private float lifespan;
    private Vector2 _velocity;

    public Vector3 mousePosition;

    public void Initialise()
    {
        // Make sure z is ZERO
        mousePosition.z = 0f;

        // Calculates direction from current position to mouse position
        Vector3 direction3 = (mousePosition - transform.position).normalized;
        Vector2 direction = new Vector2(direction3.x, direction3.y);

        // Compute movement velocity
        _velocity = direction * speed;

        // Rotates object to face the direction of movement
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        lifespan = lifetime;
    }


    private void Update()
    {
        // Move the bullet
        transform.position += (Vector3)_velocity * Time.deltaTime;

        lifespan -= Time.deltaTime;

        if (lifespan <= 0f)
        {
            // Return the bullet to the object pool instead of destroying it
            ObjectPooler.EnqueueObject(this, "Bullet");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            var enemy = other.gameObject.GetComponent<EnemyController>();
            enemy.health -= damage;
            Destroy(gameObject);
        }
    }
}
