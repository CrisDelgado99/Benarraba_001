using System.Collections;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    #region Attributes
    [Header("Bullet Info")]
    [SerializeField] private float activeTime;

    private float shootTime;

    [Header("Particles")]
    [SerializeField] private GameObject healParticle;
    [SerializeField] private GameObject impactParticle;

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

        if (other.CompareTag("npc"))
        {
            if (other.GetComponent<NPCController>().IsZombie)
            {
                other.GetComponent<NPCController>().IsZombie = false;
                LevelManager.Instance.zombieTransformList.Remove(other.transform);
                LevelManager.Instance.personTransformList.Add(other.transform);
                Instantiate(healParticle, transform.position, Quaternion.identity);
            } else
            {
                Instantiate(healParticle, transform.position, Quaternion.identity);
            }

        } else
        {
            Instantiate(impactParticle, transform.position, Quaternion.identity);
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
