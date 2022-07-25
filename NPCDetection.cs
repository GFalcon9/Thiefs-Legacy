using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDetection : MonoBehaviour
{
    [Header("Type")]
    [SerializeField] bool enemy;
    [SerializeField] bool npc;

    [Header("Player")]
    public Transform player;

    bool isPlayerInRange = false;

    [Header("FOV")]
    [SerializeField] PlayerMovement playerSneaking;
    [SerializeField] GameObject nFOV;
    [SerializeField] GameObject sFOV;

    [Header("Restricted Areas")]
    public RestrictedAreas restrictedAreas;
    public Transform restrictedAreaContainer;

    [Header("NavMesh")]
    [SerializeField] NPCNavMesh nPCNavMesh;
    [SerializeField] Transform nPCDude;

    [Header("UI")]
    [SerializeField] NPCIcons nPCIcons;
    [SerializeField] DialogueScript dialogueScript;

    [Header("Fail")]
    [SerializeField] Fail fail;

    [Header("Detection Time")]
    [SerializeField] float detectionTime = 5f;
    [SerializeField] float timeLeft = 2f;
    bool timerActive;

    [Header("Enemy Type")]
    [SerializeField] EnemyTypes enemyTypes;

    void Start()
    {
        playerSneaking = FindObjectOfType<PlayerMovement>();
        fail = FindObjectOfType<Fail>();
        nPCIcons = GetComponent<NPCIcons>();
        nPCNavMesh = GetComponent<NPCNavMesh>();

        if (enemy)
        {
            restrictedAreas = restrictedAreaContainer.GetComponentInChildren<RestrictedAreas>();

            enemyTypes = GetComponent<EnemyTypes>();
        }
            
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            isPlayerInRange = true;
        }

        if(nPCNavMesh.enemy)
        {
            if (other.transform == nPCDude)
            {
                nPCNavMesh.timeLeft = 3f;
                nPCNavMesh.moveAway = true;
            }
        }

        if(enemy)
        {
            detectionTime = enemyTypes.detectionTime;

            timeLeft = enemyTypes.timeForDetection;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            isPlayerInRange = false;

            nPCIcons.uIIcons.SetActive(false);
            nPCIcons.dialogueObject.SetActive(false);

            nPCIcons.miniMapSpriteRenderer.sprite = nPCIcons.normalMMIcon;

            ResetTimer();
        }

        if (dialogueScript.canTalk)
        {
            nPCIcons.uIIcons.SetActive(true);
            nPCIcons.spriteRenderer.sprite = nPCIcons.talkingIcon;
        }
    }

    void Update()
    {
        if (playerSneaking.isSneaking)
        {
            nFOV.SetActive(false);
            sFOV.SetActive(true);
        }
        else
        {
            nFOV.SetActive(true);
            sFOV.SetActive(false);
        }

        if (timerActive)
        {
            Timer();
        }
    }

    void FixedUpdate()
    {
        if (isPlayerInRange)
        {
            Vector3 direction = player.position - transform.position;

            Ray ray = new Ray(transform.position + Vector3.up, direction);
            RaycastHit raycastHit;

            Debug.DrawRay(transform.position + Vector3.up, direction);

            if (Physics.Raycast(ray, out raycastHit))
            {
                if (raycastHit.collider.transform == player)
                {
                    if (RestrictedAreas.isPlayerInRA == true)
                    {
                        nPCIcons.uIIcons.SetActive(true);
                        nPCIcons.spriteRenderer.sprite = nPCIcons.alertedIcon;
                        nPCIcons.miniMapSpriteRenderer.sprite = nPCIcons.alertedMMIcon;

                        nPCIcons.dialogueObject.SetActive(true);

                        if (enemy)
                        {
                            timerActive = true;
                        }
                    }

                    else if (PlayerActions.beingIllegal == true)
                    {
                        nPCIcons.uIIcons.SetActive(true);
                        nPCIcons.spriteRenderer.sprite = nPCIcons.alertedIcon;
                        nPCIcons.miniMapSpriteRenderer.sprite = nPCIcons.alertedMMIcon;

                        nPCIcons.dialogueObject.SetActive(true);

                        if (enemy)
                        {
                            timerActive = true;
                        }
                    }

                    else
                    {
                        ResetTimer();

                        nPCIcons.uIIcons.SetActive(true);
                        nPCIcons.spriteRenderer.sprite = nPCIcons.seenIcon;
                        nPCIcons.miniMapSpriteRenderer.sprite = nPCIcons.seenMMIcon;

                        nPCIcons.dialogueObject.SetActive(false);
                    }
                }
            }
        }
    }

    void Detected()
    {
        if (fail.isDetected)
        {
            return;
        }
        fail.isDetected = true;

        fail.GotCaught();
    }

    void Timer()
    {
        timeLeft -= 1 * Time.deltaTime;

        if (timeLeft <= 0)
        {
            Detected();
        }
    }

    void ResetTimer()
    {
        timerActive = false;

        if(enemy)
        {
            timeLeft = enemyTypes.detectionTime;
        }  

        if(npc)
        {
            timeLeft = detectionTime;
        }
    }
}
