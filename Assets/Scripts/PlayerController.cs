using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private int currentLives;
    [SerializeField] private int maxLives;

    private void Awake()
    {
        currentLives = maxLives;
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
            //TODO GamManager, HUD 
            Debug.Log("Game Over!!");
        }
    }
}
