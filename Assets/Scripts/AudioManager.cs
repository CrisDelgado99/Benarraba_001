using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource saveZombieAudio;
    [SerializeField] private AudioSource shatteringBottleAudio;
    [SerializeField] private AudioSource getLifeAudio;
    [SerializeField] private AudioSource getAmmoAudio;
    [SerializeField] private AudioSource shootAudio;
    [SerializeField] private AudioSource zombieHitAudio;
    void Awake()
    {
        // If an instance already exists and it's not this one, destroy this object
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Assign the instance and mark this object to not be destroyed on load
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySaveZombieAudio()
    {
        saveZombieAudio.Play();
    }

    public void PlayShatteringBottleAudio()
    {
        shatteringBottleAudio.Play();
    }

    public void PlayGetLifeAudio()
    {
        getLifeAudio.Play();
    }

    public void PlayGetAmmoAudio()
    {
        getAmmoAudio.Play();
    }

    public void PlayShootAudio()
    {
        shootAudio.Play();
    }

    public void PlayZombieHitAudio()
    {
        zombieHitAudio.Play();
    }
}
