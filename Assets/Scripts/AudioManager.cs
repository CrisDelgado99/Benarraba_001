using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

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
}
