using System.Threading;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] public float maxHealth;
    [SerializeField] public float moveSpeed;
    [SerializeField] public float rateOfFire;
    [HideInInspector] public float health;
    private float invincibilityDuration = 0.75f;
    private float currentInvincibilityTime = 0f;

    private PlayerMovement _movement;
    private PlayerShooting _shooting;

    private void Awake()
    {
        health = maxHealth;

        // Use current scripts values to set individualized values from other scripts
        _movement = GetComponent<PlayerMovement>();
        _shooting = GetComponent<PlayerShooting>();
        if (_movement != null)
        {
            _movement.SetMoveSpeed(moveSpeed);
        }
        if (_shooting != null)
        {
            _shooting.SetRateOfFire(rateOfFire);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "EnemyBullet" && currentInvincibilityTime <= 0)
        {
            health -= collision.GetComponent<EnemyBullet>().damage;
            currentInvincibilityTime = invincibilityDuration;
            print("Player hit! Current health: " + health);

            // MUST set enemy bullet to inactive, as opposed to deleting it, to allow for object pooling to work properly
            collision.gameObject.SetActive(false);
        }
        // TEMPORARY DEATH! should not destroy player in full version
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    // Plan to add a LOW-HP glow such as in RotMG when 1-2 hits away from death^^^

    void Update()
    {
        currentInvincibilityTime -= Time.deltaTime;
    }
}
