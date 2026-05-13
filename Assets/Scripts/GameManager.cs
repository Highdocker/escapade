using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Contains the reference to a bullet prefab
    public PlayerBulletController bulletPrefab;

    private void Awake()
    {
        SetupPool();
    }

    private void SetupPool()
    {
        // Initiate the object pooler, with the second parameter determining
        // how many objects to pre-instantiate (Reccomended: 1000)
        ObjectPooler.SetupPool(bulletPrefab, 500, "Bullet");
    }
}
