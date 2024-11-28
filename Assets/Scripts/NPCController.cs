using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class NPCController : MonoBehaviour
{
    [SerializeField] private bool isZombie = false;
    public bool IsZombie { get =>  isZombie; }

    [SerializeField] private GameObject checkpointsParent;
    private List<GameObject> checkpointsList = new();
    public List<GameObject> CheckpointsList { get => checkpointsList; }

    private void Start()
    {
        if (checkpointsParent != null)
        {
            foreach (Transform child in checkpointsParent.transform)
            {
                checkpointsList.Add(child.gameObject);
            }
        }
    }

    public int getClosestCheckpointIndex()
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
}
