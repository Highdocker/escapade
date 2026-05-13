using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] public BulletController bullet;

    // Variables for determining firerate, and when you can shoot
    private float rateOfFire = 1;
    private float fireCooldown = 0f;
    private bool canShoot = true;

    public void SetRateOfFire(float rof)
    {
        // Choose between given rof, or a very small value if one is not given
        // in order to prevent divide by zero errors with future calculation
        rateOfFire = Mathf.Max(0.0001f, rof);
    }

    private void Update()
    {
        fireCooldown -= Time.deltaTime;

        // If the cooldown has passed, allow the player to shoot again
        if (fireCooldown <= 0)
        {
            canShoot = true;
        }

        // If player inputs to shoot, and is allowed to shoot, then shoot a bullet
        if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Mouse0)) && canShoot)
        {
            BulletController instance = ObjectPooler.DequeueObject<BulletController>("Bullet");

            instance.transform.position = transform.position;
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;
            instance.mousePosition = mouseWorld;

            instance.gameObject.SetActive(true);
            instance.Initialise();

            // Reset cooldown that determines when you can shoot again
            canShoot = false;
            fireCooldown = 1f / rateOfFire;
        }
    }
}
