using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    //Singleton
    public static LevelManager Instance;

    public List<Transform> personTransformList = new();
    public List<Transform> zombieTransformList = new ();

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        GameObject[] npcObjects = GameObject.FindGameObjectsWithTag("npc");
        foreach (GameObject npc in npcObjects)
        {
            NPCController npcController = npc.GetComponent<NPCController>();
            if (npcController.IsZombie)
            {
                zombieTransformList.Add(npc.transform);
            }
            else
            {
                personTransformList.Add(npc.transform);
            }
        }
    }
}
