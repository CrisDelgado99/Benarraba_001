using System.Collections;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    #region Attributes
    [Header("Bullet Info")]
    [SerializeField] private float activeTime;

    private float shootTime;

    [Header("Particles")]
    //[SerializeField] private GameObject bloodParticle;
    //[SerializeField] private GameObject impactParticle;

    private bool isPlayer;
    public bool IsPlayer { get => isPlayer; set => isPlayer = value; }

    private int damage;

    public int Damage { get => damage; set => damage = value; }
    #endregion

    #region Event Functions
    private void OnEnable() //When the gameobject SetActive = true;
    {
        StartCoroutine(DeactivateAfterTime());
    }

    //When the bullet collides with something
    private void OnTriggerEnter(Collider other)
    {
        gameObject.SetActive(false);

        if (other.CompareTag("Enemy"))
        {
            if (isPlayer)
            {
                //Instantiate damageParticle "Blood"
                //Instantiate(bloodParticle, transform.position, Quaternion.identity);
                other.GetComponent<EnemyController>().DamageEnemy(damage);
            }
            
        } else if(other.CompareTag("Player")){
            if (!isPlayer)
            {
                //Instantiate(bloodParticle, transform.position, Quaternion.identity);
                other.GetComponent<PlayerController>().DamagePlayer(damage);
            }

        } else
        {
            //Instantiate(impactParticle, transform.position, Quaternion.identity);
        }
    }
    #endregion

    #region Other Methods
    private IEnumerator DeactivateAfterTime()
    {
        yield return new WaitForSeconds(activeTime);
        gameObject.SetActive(false);
    }
    #endregion
}
