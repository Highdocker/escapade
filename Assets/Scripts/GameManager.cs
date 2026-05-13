using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Contains the reference to a bullet prefab
    [Header("Pooling")]
    [SerializeField] private PlayerBulletController playerBulletPrefab;
    [SerializeField] private int playerPoolSize = 250;
    [SerializeField] private EnemyBullet enemyBulletPrefab;
    [SerializeField] private int enemyPoolSize = 1000;

    private const string EnemyBulletPoolKey = "EnemyBullet";

    private void Awake()
    {
        SetupPool();
    }

    private void SetupPool()
    {
        // Initiate the object pooler, with the second parameter determining
        // how many objects to pre-instantiate (Reccomended: 1000)
        ObjectPooler.SetupPool(playerBulletPrefab, playerPoolSize, "Bullet");
        ObjectPooler.SetupPool(enemyBulletPrefab, enemyPoolSize, EnemyBulletPoolKey);
    }
}
