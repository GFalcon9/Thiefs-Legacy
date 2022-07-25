using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour
{
    DoorInteraction doorInteraction;
    Hacking hacking;
    PlayerMovement playerMovement;

    bool timerActive;
    bool rewindingTime;
    bool inTrigger;

    public static bool beingIllegal;

    [Header("Object")]
    [SerializeField] bool door;
    [SerializeField] bool item;

    [Header("Skill")]
    public bool canBeHacked;
    Drone drone;

    [Header("Difficulty")]
    public bool easy;
    public bool medium;
    public bool hard;

    [SerializeField] float startTime = 0;
    [SerializeField] float fullTime = 0f;
    [SerializeField] float shortTime = 3f;
    [SerializeField] float midTime = 6f;
    [SerializeField] float longTime = 12f;

    [Header("UI")]
    [SerializeField] UIManager uIManager;

    [Header("Fail")]
    Fail fail;

    void Start()
    {
        uIManager = FindObjectOfType<UIManager>();
        drone = FindObjectOfType<Drone>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        fail = FindObjectOfType<Fail>();

        if (door)
        {
            doorInteraction = GetComponent<DoorInteraction>();
            if(doorInteraction.inactive)
            {
                this.GetComponent<PlayerActions>().enabled = false;
            }
            else
            {
                this.GetComponent<PlayerActions>().enabled = true;
            }
        }

        if(canBeHacked)
        {
            hacking = GetComponent<Hacking>();
        }

        if(canBeHacked)
        {
            startTime = 0;

            if (easy)
            {
                fullTime = shortTime;
            }

            if (medium)
            {
                fullTime = midTime;
            }

            if (hard)
            {
                fullTime = longTime;
            }
        }
    }

    void Update()
    {
        if(inTrigger)
        {
            if (canBeHacked)
            {
                uIManager.progressImage.fillAmount = startTime / fullTime;

                if (timerActive)
                {
                    rewindingTime = false;
                    Timer();
                }
                else
                {
                    rewindingTime = true;
                    RewindTimer();
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            inTrigger = true;

            if(!drone.droneMode)
            {
                if(door)
                {
                    if (!doorInteraction.inactive)
                    {
                        uIManager.interactableBox.SetActive(true);

                        if (!doorInteraction.open)
                        {
                            uIManager.interactableText.text = "[E] Open";
                        }
                    }

                    if (canBeHacked)
                    {
                        uIManager.interactableBox.SetActive(true);
                        uIManager.interactableText.text = "Hack";
                    }
                }

                if (item && hacking.interactable)
                {
                    uIManager.interactableBox.SetActive(true);
                    uIManager.interactableText.text = "Hack";
                }
            }
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!drone.droneMode) 
            {
                if(!fail.isDetected)
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        if (door)
                        {
                            if (!doorInteraction.locked)
                            {
                                if (!doorInteraction.open)
                                {
                                    doorInteraction.DoorOpen();

                                    uIManager.interactableBox.SetActive(false);
                                }
                            }
                            else
                            {
                                uIManager.feedbackBox.SetActive(true);
                                uIManager.feedbackText.text = "Locked";
                            }
                        }
                    }

                    if (canBeHacked)
                    {
                        if (door)
                        {
                            if (Input.GetMouseButtonDown(0))
                            {
                                timerActive = true;
                                beingIllegal = true;

                                uIManager.interactableBox.SetActive(false);
                                uIManager.feedbackBox.SetActive(false);
                            }

                            if (Input.GetMouseButtonUp(0))
                            {
                                beingIllegal = false;
                                timerActive = false;
                            }
                        }

                        if (item && hacking.interactable)
                        {
                            if (Input.GetMouseButtonDown(0))
                            {
                                timerActive = true;
                                beingIllegal = true;

                                uIManager.interactableBox.SetActive(false);
                                uIManager.feedbackBox.SetActive(false);
                            }

                            if (Input.GetMouseButtonUp(0))
                            {
                                beingIllegal = false;
                                timerActive = false;
                            }
                        }
                    }
                }
                else
                {
                    timerActive = false;
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            inTrigger = false;

            uIManager.interactableBox.SetActive(false);

            if(door)
            {
                if (doorInteraction.open)
                {
                    doorInteraction.DoorClose();
                }
            }

            if(canBeHacked)
            {
                uIManager.feedbackBox.SetActive(false);
                ResetTimer();
            }

            if (item && hacking.distract)
            {
                canBeHacked = true;

                timerActive = false;
            }
        }
    }

    void Timer()
    {
        if (!rewindingTime)
        {
            uIManager.progressUI.SetActive(true);

            startTime += 1 * Time.deltaTime;

            if (startTime >= fullTime)
            {
                ResetTimer();

                uIManager.feedbackBox.SetActive(true);

                if (canBeHacked)
                {
                    beingIllegal = false;

                    uIManager.feedbackText.text = "Hacking Successful";

                    hacking.HackComplete();
                }
            }
        }
    }

    void RewindTimer()
    {
        if (rewindingTime)
        {
            uIManager.progressUI.SetActive(true);

            startTime -= 1* Time.deltaTime;

            if (startTime <= 0)
            {
                uIManager.interactableBox.SetActive(true);
                ResetTimer();
            }
        }  
    }

    void ResetTimer()
    {
        uIManager.progressUI.SetActive(false);
        startTime = 0;
    }
}
