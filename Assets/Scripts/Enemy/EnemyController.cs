using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] public float maxHealth = 5f;
    public float health;
    void Start()
    {
        health = maxHealth;
    }

    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
