using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using static State;
using Unity.VisualScripting;
using UnityEngine.InputSystem.HID;

public class NPCController : MonoBehaviour
{
    [SerializeField] private bool isZombie = false;
    public bool IsZombie { get => isZombie; set => isZombie = value; }
    private bool isAttacking = false;
    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }

    private Transform attackingZombie = null;
    public Transform AttackingZombie { get => attackingZombie; set => attackingZombie = value; }


    [SerializeField] private Material zombieMaterial;
    public Material ZombieMaterial { get => zombieMaterial; }
    [SerializeField] private Material personMaterial;
    public Material PersonMaterial { get => personMaterial; }

    [SerializeField] private GameObject personCheckpointsParent;
    [SerializeField] private GameObject zombieGroupOfCheckpointsParent;

    private List<GameObject> personCheckpointsList = new();
    public List<GameObject> PersonCheckpointsList { get => personCheckpointsList; }

    private List<List<GameObject>> zombieGroupOfCheckpointsList = new();
    public List<List<GameObject>> ZombieGroupOfCheckpointsList { get => zombieGroupOfCheckpointsList; }

    private void Start()
    {
        if (personCheckpointsParent != null)
        {
            foreach (Transform checkpoint in personCheckpointsParent.transform)
            {
                personCheckpointsList.Add(checkpoint.gameObject);
                checkpoint.gameObject.GetComponent<MeshRenderer>().enabled = false;
            }
        }

        if (zombieGroupOfCheckpointsParent != null)
        {
            foreach (Transform group in zombieGroupOfCheckpointsParent.transform)
            {
                List<GameObject> groupCheckpoints = new();

                foreach (Transform checkpoint in group)
                {
                    groupCheckpoints.Add(checkpoint.gameObject);
                    checkpoint.gameObject.GetComponent<MeshRenderer>().enabled = false;
                }

                zombieGroupOfCheckpointsList.Add(groupCheckpoints);
            }

        }
    }

    public int GetClosestGroupIndex(List<List<GameObject>> groupList, Transform npcTransform)
    {
        if (groupList.Count == 0)
        {
            return -1;
        }

        int closestGroupIndex = 0;
        float closestDistance = float.MaxValue;

        Vector3 npcPosition = npcTransform.position;

        for (int i = 0; i < groupList.Count; i++)
        {
            Vector3 groupCenter = GetGroupCenter(groupList[i]);

            float distance = Vector3.Distance(npcPosition, groupCenter);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestGroupIndex = i;
            }
        }

        return closestGroupIndex;
    }

    private Vector3 GetGroupCenter(List<GameObject> group)
    {
        if (group.Count == 0)
            return Vector3.zero;

        Vector3 groupCenter = Vector3.zero;

        // Calculate the average world position of all the objects in the group
        foreach (GameObject obj in group)
        {
            groupCenter += obj.transform.position;
        }

        // Return the average position as the center of the group
        return groupCenter / group.Count;
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
        Transform child = transform.GetChild(0);

        if (child != null)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = child.Find("Character").GetComponent<SkinnedMeshRenderer>();

            Material[] materials = skinnedMeshRenderer.materials;

            // Assume the skin material is named "Skin" (or replace with your specific name)
            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i].name.Contains("Skin")) // Adjust the condition as needed
                {
                    Debug.Log(spriteMaterial);
                    materials[i] = spriteMaterial; // Replace the skin material
                    break;
                }
            }

            // Reassign the updated materials array back to the renderer
            skinnedMeshRenderer.materials = materials;

        }
    }

    public bool IsTransformInFOV(Transform otherTransform, float maxAngle)
    {
        Vector3 directionToTransform = otherTransform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToTransform);

        return angle < maxAngle;
    }

    public void LookAtWithNoYRotation(Transform currentDestination)
    {
        transform.LookAt(new Vector3(currentDestination.transform.position.x, transform.position.y, currentDestination.transform.position.z));
    }

    private void OnTriggerEnter(Collider other)
    {
        NPCController otherNpcController = other.GetComponent<NPCController>();
        if(otherNpcController != null)
        {
            if (otherNpcController.IsZombie)
            {
                this.attackingZombie = other.transform;
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {

        NPCController otherNpcController = other.GetComponent<NPCController>();
        if (otherNpcController != null)
        {
            if (otherNpcController.IsZombie)
            {
                this.attackingZombie = null;
            }
        }
    }

}
