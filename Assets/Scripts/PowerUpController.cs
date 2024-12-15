using DG.Tweening;
using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    private enum PowerUpType { OrangeJuice, AmmoBox }
    private float upDownDistance = 0.5f; 
    private float upDownDuration = 3f;    
    private bool isUpDownLoop = true;

    private Vector3 targetRotation = new Vector3(30f, 360f, 30f);
    private float rotationDuration = 3f;

    [SerializeField] PowerUpType powerUpType;

    void Start()
    {
        MoveUpAndDown();
        Rotate();
    }

    void MoveUpAndDown()
    {
        // Get the initial position
        float startY = transform.position.y;

        // Move up and down
        transform.DOMoveY(startY + upDownDistance, upDownDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(isUpDownLoop ? -1 : 0, LoopType.Yoyo);
    }

    private void Rotate()
    {
        transform.DORotate(targetRotation, rotationDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (powerUpType == PowerUpType.OrangeJuice)
            {
                PlayerController playerController = other.GetComponent<PlayerController>();
                if (playerController.CurrentLives < playerController.MaxLives)
                {
                    other.GetComponent<PlayerController>().CurrentLives++;
                    AudioManager.Instance.PlayGetLifeAudio();
                    Destroy(gameObject);
                }
            }
            else if (powerUpType == PowerUpType.AmmoBox)
            {
                WeaponController weaponController = other.GetComponent<WeaponController>();
                weaponController.CurrentAmmo += Random.Range(5, 9);
                AudioManager.Instance.PlayGetAmmoAudio();
                Destroy(gameObject);
            }
        }
    }
}
