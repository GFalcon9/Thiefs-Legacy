using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueScript : MonoBehaviour
{
    public bool canTalk;
    Drone drone;

    [Header("Dialogue Type")]
    [SerializeField] bool uniqueDialogue;
    [SerializeField] bool repetableDialogue;
    [SerializeField] bool triggeredDialogue;

    [Header("Dialogue")]
    [TextArea(1, 3)]
    [SerializeField] string[] texts;
    int index;
    bool textActive;
    bool endDialogue;

    [Header("UI")]
    [SerializeField] UIManager uIManager;
    [SerializeField] NPCIcons nPCIcons;

    [Header("Player")]
    PlayerMovement playerMovement;

    [Header("Camera")]
    CMCameraBlends cMCameraBlends;
    [SerializeField] Transform playerTarget;
    [SerializeField] Transform nPCTarget;

    [Header("Quest")]
    [SerializeField] bool countsForQuest;
    [Space]
    [SerializeField] bool unlocksObject;
    [SerializeField] bool unlocksNPC;

    [Header("Achievement")]
    [SerializeField] bool countsForSoulAchievement;
    [SerializeField] bool countsForEelAchievement;
    Achievements achievements;

    [Header("Object")]
    [SerializeField] Hacking interactableObject;

    [Header("NPC")]
    [SerializeField] NPCNavMesh nPCNavMesh;

    [Header("Font")]
    [SerializeField] bool changeFont;

    void Start()
    {
        uIManager = FindObjectOfType<UIManager>();
        drone = FindObjectOfType<Drone>();
        nPCIcons = GetComponent<NPCIcons>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        cMCameraBlends = FindObjectOfType<CMCameraBlends>();
        achievements = FindObjectOfType<Achievements>();

        if (canTalk)
        {
            if(!countsForEelAchievement)
            {
                nPCIcons.uIIcons.SetActive(true);
                nPCIcons.spriteRenderer.sprite = nPCIcons.talkingIcon;
            }
        }

        if(countsForQuest)
        {
            if (unlocksObject)
            {
                interactableObject = interactableObject.GetComponent<Hacking>();
            }

            if (unlocksNPC)
            {
                nPCNavMesh = nPCNavMesh.GetComponent<NPCNavMesh>();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(canTalk && !drone.droneMode)
        {
            if (other.tag == "Player")
            {
                uIManager.interactableBox.SetActive(true);
                if (!countsForEelAchievement)
                {
                    uIManager.interactableText.text = "[E] Talk";
                }
                else
                {
                    uIManager.interactableText.text = "[E] Interact";
                }

                index = 0;
                uIManager.dialogueText.text = texts[index];
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (canTalk && Input.GetKeyDown(KeyCode.E) && !drone.droneMode)
            {
                uIManager.interactableBox.SetActive(false);

                uIManager.dialogueBox.SetActive(true);

                textActive = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(canTalk)
        {
            if (other.tag == "Player")
            {
                uIManager.interactableBox.SetActive(false);
                uIManager.dialogueBox.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (textActive)
        {
            if (Input.GetKeyDown(KeyCode.E) && uIManager.dialogueBox.activeSelf)
            {
                if(changeFont)
                {
                    uIManager.dialogueText.font = uIManager.ghostlyFont;
                }
                else
                {
                    uIManager.dialogueText.font = uIManager.normalFont;
                }

                playerMovement.moveSpeed = 0;
                playerMovement.canMove = false;

                cMCameraBlends.zoom = true;
                cMCameraBlends.view = false;

                playerTarget.transform.LookAt(nPCTarget);

                if (++index < texts.Length)
                {
                    uIManager.dialogueText.text = texts[index];
                }
                else
                {
                    if (index == texts.Length)
                    {
                        EndDialogue();
                    }
                }
            }
        }
    }

    public void EndDialogue()
    {
        playerMovement.moveSpeed = 4;
        playerMovement.canMove = true;

        cMCameraBlends.zoom = false;
        cMCameraBlends.view = true;

        if (uniqueDialogue)
        {
            canTalk = false;

            uIManager.dialogueBox.SetActive(false);
            textActive = false;

            if (countsForQuest)
            {
                InformationManager.informationFound++;

                if (unlocksObject)
                {
                    interactableObject.interactable = true;
                }

                if (unlocksNPC)
                {
                    nPCNavMesh.patrolling = true;
                }
            }

            if (countsForSoulAchievement)
            {
                achievements.SoulAchievementFound();
            }

            if (countsForEelAchievement)
            {
                achievements.EelAchievementFound();
            }
        }

        if (repetableDialogue)
        {
            uIManager.dialogueBox.SetActive(false);
            textActive = false;

            uIManager.interactableBox.SetActive(true);
            uIManager.interactableText.text = "[E] Talk";

            index = 0;
            uIManager.dialogueText.text = texts[index];
        }
    }
}
