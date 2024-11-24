using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [SerializeField] private string enemyName;
    [SerializeField] private string description;
    [SerializeField] private float speed;
    [SerializeField] private float shootRate;
    [SerializeField] private Material enemyMaterial;
    [SerializeField] private int maxLife;

    public float Speed { get => speed; }
    public float ShootRate { get => shootRate; }
    public Material EnemyMaterial { get => enemyMaterial; }
    public int MaxLife { get => maxLife; }

}
