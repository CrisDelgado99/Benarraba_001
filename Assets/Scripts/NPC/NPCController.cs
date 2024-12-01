using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class NPCController : MonoBehaviour
{
    [SerializeField] private bool isZombie = false;
    [SerializeField] private Material zombieMaterial;
    public Material ZombieMaterial { get => zombieMaterial; }
    [SerializeField] private Material personMaterial;
    public Material PersonMaterial { get => personMaterial; }
    public bool IsZombie { get =>  isZombie; }

    [SerializeField] private GameObject personCheckpointsParent;
    [SerializeField] private GameObject zombieCheckpointsParent;

    private List<GameObject> personCheckpointsList = new();
    public List<GameObject> PersonCheckpointsList { get => personCheckpointsList; }


    private List<GameObject> zombieCheckpointsList = new();
    public List<GameObject> ZombieCheckpointsList { get => zombieCheckpointsList; }

    private void Start()
    {
        if (personCheckpointsParent != null)
        {
            foreach (Transform child in personCheckpointsParent.transform)
            {
                personCheckpointsList.Add(child.gameObject);
            }
        }

        if (zombieCheckpointsParent != null)
        {
            foreach (Transform child in zombieCheckpointsParent.transform)
            {
                zombieCheckpointsList.Add(child.gameObject);
            }
        }
    }

    public int GetClosestCheckpointIndex(List<GameObject> checkpointsList)
    {
        if (checkpointsList.Count == 0)
        {
            return -1;
        }

        int closestIndex = 0;
        float closestDistance = float.MaxValue;

        Vector3 npcPosition = transform.position;

        for (int i = 0; i < checkpointsList.Count; i++)
        {
            float distance = Vector3.Distance(npcPosition, checkpointsList[i].transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }

    public Transform NearestNpcOfTypeTransform(List<Transform> transformList, float distance)
    {
        foreach (Transform npcTransform in transformList)
        {
            if (Vector3.Distance(transform.position, npcTransform.position) < distance)
            {
                return npcTransform; // A person is near
            }
        }

        return null;
    }

    public void SetSpriteColor(Material spriteMaterial)
    {
        MeshRenderer childRenderer = transform.Find("Capsule").GetComponent<MeshRenderer>();

        if (childRenderer != null)
        {
            // Set the material of the MeshRenderer to the provided material
            childRenderer.material = spriteMaterial;
        }
    }
}
