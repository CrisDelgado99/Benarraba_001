using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;
using System;

public class IntroMovement : MonoBehaviour
{
    private Animator helperGuyAnimator;
    private List<GameObject> dialoguesList = new();
    private Transform dialoguesParent;
    
    private int currentDialogueIndex;
    private bool isTalking = true;

    private NavMeshAgent agent;

    private List<Transform> patrolList = new();
    private Transform patrolParent;
    private int currentPatrolIndex = 0;

    private Transform dialoguesParent2;

    // Start is called before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        helperGuyAnimator = GameObject.Find("HelperGuyAnim").GetComponent<Animator>();
        helperGuyAnimator.SetBool("isTalking", true);

        dialoguesParent = GameObject.Find("DialoguePanel").transform;

        dialoguesParent2 = GameObject.Find("DialoguePanel2").transform;
        dialoguesParent2.gameObject.SetActive(false);

        foreach (Transform dialogue in dialoguesParent)
        {
            dialoguesList.Add(dialogue.gameObject);
            dialogue.gameObject.SetActive(false);
        }

        agent = GetComponent<NavMeshAgent>();

        patrolParent = GameObject.Find("PersonPatrolPoints").transform;

        foreach (Transform patrol in patrolParent)
        {
            patrol.gameObject.GetComponent<MeshRenderer>().enabled = false;
            patrolList.Add(patrol);
        }

        if (dialoguesList.Count > 0)
        {
            ShowDialogue(currentDialogueIndex);
        }




    }

    private void ShowDialogue(int index)
    {
        dialoguesList[index].SetActive(true);
        Button nextButton = dialoguesList[index].GetComponentInChildren<Button>();

        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(NextDialogue);
    }

    private void NextDialogue()
    {
        dialoguesList[currentDialogueIndex].SetActive(false);
        currentDialogueIndex++;

        if (currentDialogueIndex < dialoguesList.Count)
        {
            ShowDialogue(currentDialogueIndex);
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        isTalking = false;
        helperGuyAnimator.SetBool("isTalking", false);
        dialoguesParent.gameObject.SetActive(false);
        StartCoroutine(MoveAcrossTown());
    }

    private IEnumerator MoveAcrossTown()
    {
        yield return new WaitForSeconds(2f);

        agent.speed = 20;
        agent.height = 10;
        agent.baseOffset = 5;

        foreach (Transform patrolPoint in patrolList)
        {
            agent.SetDestination(patrolPoint.position);
            dialoguesParent2.gameObject.SetActive(false);
            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            {
                yield return null; // Wait for the next frame
            }
        }

        StartCoroutine(ShowSecondDialogue());

    }

    private IEnumerator ShowSecondDialogue()
    {
        yield return new WaitForSeconds(2f);
        dialoguesParent2.gameObject.SetActive(true);

        Button nextButton = dialoguesParent2.GetChild(0).GetComponentInChildren<Button>();
        nextButton.onClick.AddListener(GoToGame);
    }

    private void GoToGame()
    {
        ManageScenes manageScenes = GetComponent<ManageScenes>();
        manageScenes.GoToNextScene();
    }
}
