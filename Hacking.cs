using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hacking : MonoBehaviour
{
    [SerializeField] bool doors;
    [SerializeField] bool finalHack;
    public bool distract;
    [SerializeField] bool information;
    [Space]
    public bool interactable;

    [Header("Scripts")]
    UIManager uIManager;
    DoorInteraction doorInteraction;
    [SerializeField] NPCNavMesh enemyToDistract;
    Drone drone;
    PlayerActions playerActions;
    PlayerMovement playerMovement;
    NotesCodex notesCodex;
    ObjectivesManager objectivesManager;

    [Header("Info Text")]
    bool infoUnlocked;
    int index;
    bool textActive;
    bool firstTime;
    [TextArea(1, 3)]
    [SerializeField] string[] texts;
    [Space]
    [SerializeField] bool objective;

    [Header("Camera")]
    [SerializeField] bool distraction1;
    [SerializeField] bool distraction2;
    [SerializeField] bool distraction3;
    [SerializeField] bool distraction4;

    [SerializeField] GameObject distractionCamera;
    [SerializeField] GameObject distractionCameraSpace;


    [Header("Material")]
    public Renderer objectRenderer;
    public Material objectNormalMaterial;
    public Material objectInteractableMaterial;

    [Header("Icon")]
    [SerializeField] GameObject icon;

    float timeUntilHackAgain;
    float startTime;

    void Awake()
    {
        uIManager = FindObjectOfType<UIManager>();
        drone = FindObjectOfType<Drone>();
        playerActions = GetComponent<PlayerActions>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        objectivesManager = FindObjectOfType<ObjectivesManager>();

        if (distract || information || finalHack)
        {
            objectRenderer = objectRenderer.GetComponent<Renderer>();
        }

        if (doors)
        {
            doorInteraction = GetComponent<DoorInteraction>(); 
        }
        
        if(distract)
        {
            enemyToDistract.GetComponent<NPCNavMesh>();
        }

        if(information)
        {
            notesCodex = GetComponent<NotesCodex>();
        }

    }
    #region Information Dialogue
    void Update()
    {
        if(distract || information || finalHack)
        {
            if (interactable)
            {
                objectRenderer.material = objectInteractableMaterial;

                icon.SetActive(true);
            }
            else
            {
                objectRenderer.material = objectNormalMaterial;

                icon.SetActive(false);
            }
        }

        if (textActive)
        {
            if (Input.GetKeyDown(KeyCode.E) && uIManager.dialogueBox.activeSelf)
            {
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
    #endregion

    public void HackComplete()
    { 
        if (doors)
        {
            doorInteraction.locked = false;

            uIManager.feedbackBox2.SetActive(true);
            uIManager.feedbackText2.text = "Door Unlocked";

            playerActions.canBeHacked = false;
        }

        if (distract)
        {
            enemyToDistract.distractedByObject = true;

            uIManager.feedbackBox2.SetActive(true);
            uIManager.feedbackText2.text = "Enemy Distracted";

            playerActions.canBeHacked = false;
            enemyToDistract.timeLeft = enemyToDistract.timeToWait;

            if (distraction1)
            {
                distractionCameraSpace.SetActive(true);
                distractionCamera.SetActive(true);
                StartCoroutine(ResetCamera());
            }

            if (distraction2)
            {
                distractionCameraSpace.SetActive(true);
                distractionCamera.SetActive(true);
                StartCoroutine(ResetCamera());
            }

            if (distraction3)
            {
                distractionCameraSpace.SetActive(true);
                distractionCamera.SetActive(true);
                StartCoroutine(ResetCamera());
            }

            if (distraction4)
            {
                distractionCameraSpace.SetActive(true);
                distractionCamera.SetActive(true);
                StartCoroutine(ResetCamera());
            }
        }

        if (information)
        {
            uIManager.interactableBox.SetActive(false);
            uIManager.dialogueBox.SetActive(true);

            uIManager.dialogueText.text = texts[index];

            infoUnlocked = true;
            firstTime = true;

            playerActions.canBeHacked = false;
        }

        if (finalHack)
        {
            uIManager.winFadeScreen.SetActive(true);

            playerMovement.moveSpeed = 0;
            playerMovement.canMove = false;

            playerActions.canBeHacked = false;
        }
    }
    #region Information Dialogue
    void OnTriggerEnter(Collider other)
    {
        if (infoUnlocked && !drone.droneMode)
        {
            if (other.tag == "Player")
            {
                uIManager.interactableBox.SetActive(true);
                uIManager.interactableText.text = "[E] Use";

                index = 0;
                uIManager.dialogueText.text = texts[index];
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if(infoUnlocked && firstTime)
            {
                firstTime = false;

                uIManager.interactableBox.SetActive(false);

                uIManager.dialogueBox.SetActive(true);

                textActive = true;
            }

            if (infoUnlocked && Input.GetKeyDown(KeyCode.E) && !drone.droneMode)
            {
                uIManager.interactableBox.SetActive(false);

                uIManager.dialogueBox.SetActive(true);

                textActive = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (infoUnlocked)
        {
            if (other.tag == "Player")
            {
                uIManager.interactableBox.SetActive(false);
                uIManager.dialogueBox.SetActive(false);
            }
        }
    }

    public void EndDialogue()
    {
        if(infoUnlocked)
        {
            uIManager.dialogueBox.SetActive(false);
            textActive = false;

            uIManager.interactableBox.SetActive(true);
            uIManager.interactableText.text = "[E] Use";

            index = 0;
            uIManager.dialogueText.text = texts[index];

            notesCodex.AddToCodex();

            if(objective)
            {
                objectivesManager.GoToObjective7();
            }
        }
    }
    #endregion


    IEnumerator ResetCamera()
    {
        yield return new WaitForSeconds(5);

        if (distraction1)
        {
            distractionCameraSpace.SetActive(false);
            distractionCamera.SetActive(false);
        }

        if (distraction2)
        {
            distractionCameraSpace.SetActive(false);
            distractionCamera.SetActive(false);
        }

        if (distraction3)
        {
            distractionCameraSpace.SetActive(false);
            distractionCamera.SetActive(false);
        }

        if (distraction4)
        {
            distractionCameraSpace.SetActive(false);
            distractionCamera.SetActive(false);
        }
    }
}
