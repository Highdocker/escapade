using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] public float speed;
    [SerializeField] public float rotation;
    [SerializeField] public float damage;
    [SerializeField] public Vector2 velocity;

    void Start()
    {
        // Set bullet to the angular rotation (in degrees)
        transform.rotation = Quaternion.Euler(0, 0, rotation);
    }

    void Update()
    {
        transform.Translate(velocity * speed * Time.deltaTime);
    }
}
