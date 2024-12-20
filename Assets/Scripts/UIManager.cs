using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Attributes
    //Singleton: to use methods from this class I don't need to get a component
    public static UIManager Instance; //Capitalize cause this is a static reference to my object
    //Stamina containers
    [Header("Stamina UI")]
    [SerializeField] private Image staminaQuantity;
    private float staminaPercentage;

    private PlayerMovement playerMovement;
    private PlayerController playerController;
    private WeaponController weaponController;

    [SerializeField] private Image damageFlash;
    [SerializeField] private float damageTime;

    private Coroutine disappearCoroutine;

    [SerializeField] private TextMeshProUGUI zombieCounterText;
    [SerializeField] private TextMeshProUGUI personCounterText;
    [SerializeField] private TextMeshProUGUI savedCounterText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI ammoText;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameWinPanel;
    [SerializeField] private GameObject crossHair;

    #endregion

    #region Event Functions
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        playerController = FindFirstObjectByType<PlayerController>();
        weaponController = FindFirstObjectByType<WeaponController>();

        gameOverPanel.SetActive(false);
        gameWinPanel.SetActive(false);
        crossHair.SetActive(true);
    }

    private void Update()
    {
        ManageStaminaBar();
        UpdateCounters();
        UpdatePlayerHealth();
        UpdatePlayerAmmo();
    }
    #endregion

    #region Other Methods
    /// <summary>
    /// This method manages the stamina bar in order for it to represent the quantity of stamina at any moment
    /// </summary>
    private void ManageStaminaBar()
    {
        staminaPercentage = (float) playerMovement.CurrentStamina / playerMovement.MaxStamina;

        //Use of math lerp in order for the quantity of stamina to be represented smoothly 
        //Mathf.Lerp(previous stamina, new stamina, period of time between them both)
        staminaQuantity.fillAmount = Mathf.Lerp(staminaQuantity.fillAmount, staminaPercentage, 10f * Time.deltaTime);
    }

    public void ChangeStaminaBarColor(bool isDrowned)
    {
        if (isDrowned)
        {
            staminaQuantity.color = new Color(0.945f, 0.165f, 0.114f);
        }
        else
        {
            staminaQuantity.color = new Color(0.965f, 0.925f, 0.467f);

        }
    }

    /// <summary>
    /// Turn on Damage Flash when the Player receives a shoot
    /// </summary>
    public void ShowDamageFlash()
    {
        //if there is a coroutine running stop it
        if (disappearCoroutine != null)
            StopCoroutine(disappearCoroutine);


        Debug.Log("DamageFlash");

        //restart the image Color
        damageFlash.color = Color.white;

        //Start coroutine
        disappearCoroutine = StartCoroutine(DamageDissapear());
    }

    private IEnumerator DamageDissapear()
    {
        //Alpha variable reset to 1
        float alpha = 1.0f;

        while (alpha > 0.0f)
        {
            alpha -= (1.0f / damageTime) * Time.deltaTime;
            damageFlash.color = new Color(1.0f, 1.0f, 1.0f, alpha);
            yield return null;
        }

        Debug.Log("DamageOff");
    }

    private void UpdateCounters()
    {
        zombieCounterText.text = LevelManager.Instance.zombieTransformList.Count.ToString("000");
        personCounterText.text = LevelManager.Instance.personTransformList.Count.ToString("000");
        savedCounterText.text = LevelManager.Instance.NumberOfSavedPeople.ToString("000");
    }

    private void UpdatePlayerHealth()
    {
        healthText.text = playerController.CurrentLives.ToString("00") + "/" + playerController.MaxLives.ToString("00");
    }

    private void UpdatePlayerAmmo()
    {
        ammoText.text = weaponController.CurrentAmmo.ToString("000");
    }

    public void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
        crossHair.SetActive(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ShowGameWinPanel()
    {
        gameWinPanel.SetActive(true);
        crossHair.SetActive(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    #endregion
}
