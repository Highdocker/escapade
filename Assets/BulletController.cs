using UnityEngine;

public class BulletController : MonoBehaviour
{
    float speed = 5f;
    float lifetime = 2f;
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

        lifetime = 2f;
    }


    private void Update()
    {

        lifetime -= Time.deltaTime;

        if (lifetime <= 0f)
        {
            ObjectPooler.EnqueueObject(this, "Bullet");
        }
    }
}
