using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Contains the reference to a bullet prefab
    public BulletController bulletPrefab;

    private void Awake()
    {
        SetupPool();
    }

    private void SetupPool()
    {
        ObjectPooler.SetupPool(bulletPrefab, 5, "Bullet");
    }
}
