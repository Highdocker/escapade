using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public BulletController bullet;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            BulletController instance = ObjectPooler.DequeueObject<BulletController>("Bullet");

            instance.transform.position = transform.position;
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;
            instance.mousePosition = mouseWorld;

            instance.gameObject.SetActive(true);
            instance.Initialise();
        }
    }
}
