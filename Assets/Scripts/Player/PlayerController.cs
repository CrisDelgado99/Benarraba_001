using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private int currentLives;
    public int CurrentLives { get => currentLives; set => currentLives = value; }
    
    [SerializeField] private int maxLives;
    public int MaxLives { get => maxLives; }

    private void Awake()
    {
        currentLives = maxLives;
        Time.timeScale = 1f;
    }

    /// <summary>
    /// When the Player receives Damage
    /// </summary>
    /// <param name="quantity"></param>
    public void DamagePlayer(int quantity)
    {
        currentLives -= quantity;

        UIManager.Instance.ShowDamageFlash();

        if (currentLives <= 0)
        {
            UIManager.Instance.ShowGameOverPanel();
            Time.timeScale = 0;
        }
    }
}


//Este comentario va a causar un conflicto

// merge conflict hopefully

