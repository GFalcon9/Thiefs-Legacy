using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

public class DroneActions : MonoBehaviour
{
    [Header("Bools")]
    public bool droneIsMoving;
    public bool droneInWaypoint;
    [HideInInspector] public bool droneActive;
    [SerializeField] bool distractMode;

    [Header("Transforms")]
    [SerializeField] GameObject droneGO;
    [SerializeField] Transform drone;
    [SerializeField] Transform target;
    [SerializeField] Transform player;

    public List<Transform> nPCInRange;
    public List<GameObject> nPCDistracted;

    float speed = 2f;
    public float timeLeft;
    public bool timeOver;

    [Header("Scripts")]
    [SerializeField] Drone droneScript;
    [SerializeField] NPCNavMesh nPCNavMesh;
    [SerializeField] UIManager uIManager;

    void Start()
    {
        nPCNavMesh = FindObjectOfType<NPCNavMesh>();
        droneScript = FindObjectOfType<Drone>();
        uIManager = FindObjectOfType<UIManager>();
    }

    void Update()
    {
        if (droneActive && droneIsMoving && target != null)
        {
            drone.transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        }

        if(distractMode)
        {
            uIManager.droneDistractIcon.SetActive(true);
        }
        else
        {
            uIManager.droneDistractIcon.SetActive(false);
        }

        if(droneScript.droneMode)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ActivateDrone();
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                DeactivateDrone();
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Distract();
            }
        }
    }

    void FixedUpdate()
    {
        if (distractMode)
        {
            foreach (Transform nPC in nPCInRange)
            {
                Vector3 direction = nPC.position - transform.position;

                Ray ray = new Ray(transform.position + Vector3.up, direction);
                RaycastHit raycastHit;

                Debug.DrawRay(transform.position + Vector3.up, direction);

                if (Physics.Raycast(ray, out raycastHit))
                {
                    foreach (GameObject nPCGO in nPCDistracted)
                    {
                        nPCNavMesh = nPCGO.GetComponent<NPCNavMesh>();

                        nPCNavMesh.distractedByDrone = true;

                        if (nPCNavMesh.timeLeft <= 0)
                        {
                            DeactivateDrone();
                        }
                    }
                }
            }
        }
    }

    public void ActivateDrone()
    {
        if (droneScript.waypointActive)
        {
            target = GameObject.FindWithTag("Waypoint").transform;
            droneGO.SetActive(true);
            drone.position = player.position;

            droneActive = true;
            distractMode = false;

            uIManager.feedbackBox.SetActive(true);
            uIManager.feedbackText.text = "Drone Activated";

            uIManager.droneImage.sprite = uIManager.activatedIcon;

            if (target != null)
            {
                droneIsMoving = true;
            }
        }
        else
        {
            uIManager.feedbackBox.SetActive(true);
            uIManager.feedbackText.text = "No active waypoint";
        }
    }

    public void DeactivateDrone()
    {
        droneGO.SetActive(false);
        droneIsMoving = false;

        droneActive = false;
        distractMode = false;

        droneInWaypoint = false;

        timeOver = false;

        uIManager.feedbackBox.SetActive(true);
        uIManager.feedbackText.text = "Drone Deactivated";

        uIManager.droneImage.sprite = uIManager.deactivatedIcon;
    }

    public void Distract()
    {
        if(droneActive)
        {
            if (droneInWaypoint)
            {
                distractMode = true;
                droneIsMoving = false;
            }
            else
            {
                uIManager.feedbackBox.SetActive(true);
                uIManager.feedbackText.text = "Drone not at waypoint";
            }
        }
        else
        {
            uIManager.feedbackBox.SetActive(true);
            uIManager.feedbackText.text = "Drone is not active";
        }
    }

    public void ListenIn()
    {
        if (droneInWaypoint)
        {
            Debug.Log("Listening In");
        }
        else
        {
            Debug.Log("Not at waypoint");
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "NPC" && !nPCInRange.Contains(other.transform))
        {
            nPCInRange.Add(other.transform);
        }

        if (other.tag == "NPC" && !nPCDistracted.Contains(other.gameObject))
        {
            nPCDistracted.Add(other.gameObject);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "NPC" && nPCInRange.Contains(other.transform))
        {
            nPCInRange.Remove(other.transform);
        }

        if (other.tag == "NPC" && nPCDistracted.Contains(other.gameObject))
        {
            nPCDistracted.Remove(other.gameObject);
        }
    }
}
