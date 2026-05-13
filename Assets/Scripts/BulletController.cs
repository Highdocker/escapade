using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] public float speed = 5f;
    [SerializeField] public float lifetime = 5f;
    private float lifespan;

    public Vector3 mousePosition;
    public void Initialise()
    {
        // Get mouse position in world space
        mousePosition.z = 0f;

        // Calculates direction from current position to mouse position
        Vector3 direction = (mousePosition - transform.position).normalized;

        // Move component towards cursor location
        GetComponent<Rigidbody2D>().linearVelocity = direction * speed;

        // Rotates object to face the direction of movement
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        lifespan = lifetime;
    }


    private void Update()
    {

        lifespan -= Time.deltaTime;

        if (lifespan <= 0f)
        {
            ObjectPooler.EnqueueObject(this, "Bullet");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<EnemyController>().health -= 1;
            Destroy(gameObject);
        }
    }
}
