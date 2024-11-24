using UnityEngine;
using UnityEngine.Rendering;

public class WeaponController : MonoBehaviour
{
    #region Attributes
    [SerializeField] private Transform barrel;

    [Header("Ammonition")]
    [SerializeField] private int currentAmmo;
    [SerializeField] private int maxAmmo;
    [SerializeField] private bool infiniteAmmo;

    [Header("Performance")]
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float shootingRate;
    public float ShootingRate { get => shootingRate; set => shootingRate = value; }
    [SerializeField] private int damage;

    private ObjectPool objectPool;
    private float lastShootTime;
    private bool isPlayer;
    #endregion

    #region Event Functions
    private void Awake()
    {
        //Check if I am a Player
        isPlayer = GetComponent<PlayerMovement>() != null;
        
        //Get Object Pool Component
        objectPool = GetComponent<ObjectPool>();
    }
    #endregion

    #region Other Methods
    /// <summary>
    /// This method checks if it's possible to shoot
    /// </summary>
    /// <returns>Bool</returns>
    public bool CanShoot()
    {
        //Check shootingRate
        if(Time.time - lastShootTime >= shootingRate)
        {
            //Check Ammo
            if(currentAmmo > 0 || infiniteAmmo)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// This method handles weapon shoot
    /// </summary>
    public void Shoot()
    {
        lastShootTime = Time.time;

        //reduce the Ammo
        if (!infiniteAmmo) currentAmmo--;

        //Get a new bullet
        GameObject bullet = objectPool.GetGameObject();

        //Put the bullet at the barrel position and rotation
        bullet.transform.position = barrel.position;
        bullet.transform.rotation = barrel.rotation;

        //Assign damage to bullet
        bullet.GetComponent<BulletController>().Damage = damage;


        if (isPlayer)
        {
            //Create ray from camera to the middle of the screen
            //Viewport is a Vector3(1, 1, 0) so this is the middle
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));


            //Calculate direction point
            RaycastHit hit;
            Vector3 targetPoint;

            //Check if you are pointing to something and adjust the direction
            if(Physics.Raycast(ray, out hit))
            {
                targetPoint = hit.point;
            } else
            {
                targetPoint = ray.GetPoint(5); //Get point at 5m
            }

            bullet.GetComponent<Rigidbody>().linearVelocity = (targetPoint - barrel.position).normalized * bulletSpeed;
            bullet.GetComponent<BulletController>().IsPlayer = true;
            
        }
        //The object shooting is the enemy
        else
        {
            //Give velocity to bullet
            bullet.GetComponent<Rigidbody>().linearVelocity = barrel.forward * bulletSpeed;
            bullet.GetComponent<BulletController>().IsPlayer = false;

        }
 



    }
    #endregion
}
