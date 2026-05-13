using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] public float maxHealth;
    [SerializeField] public float moveSpeed;
    [SerializeField] public float rateOfFire;
    [HideInInspector] public float health;

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
    // Update is called once per frame
    void Update()
    {
        
    }
}
