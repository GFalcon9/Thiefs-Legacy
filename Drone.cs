using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    public bool droneMode;

    [Header("Waypoints")]
    [SerializeField] GameObject waypointContainer;
    [SerializeField] GameObject waypoint;
    [SerializeField] Transform player;
    [HideInInspector] public bool waypointActive;
    

    [Header("Player")]
    [SerializeField] PlayerMovement playerMovement;

    [Header("UI")]
    [SerializeField] UIManager uIManager;

    [Header("Drone Actions")]
    [SerializeField] DroneActions droneActions;

    void Start()
    {
        droneMode = false;
        waypointActive = false;
        
        uIManager = FindObjectOfType<UIManager>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        droneActions = FindObjectOfType<DroneActions>();
    }


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            droneMode = !droneMode;

            if(droneMode)
            {
                uIManager.droneVisual.SetActive(true);
                uIManager.normalVisual.SetActive(false);

                if (uIManager.waypointVisual != null)
                {
                    uIManager.waypointVisual.SetActive(true);
                }
            }
            else
            {
                uIManager.droneVisual.SetActive(false);
                uIManager.normalVisual.SetActive(true);

                if(uIManager.waypointVisual != null)
                {
                    uIManager.waypointVisual.SetActive(false);
                }
            }
        }

        if (droneMode && Input.GetKeyDown(KeyCode.Q))
        {
            DropWaypoint();
        }

        if (droneMode && Input.GetKeyDown(KeyCode.R))
        {
            DestroyWaypoint();
        }
    }

    void DropWaypoint()
    {
        if(!droneActions.droneActive)
        {
            waypointActive = true;

            Destroy(waypointContainer);
            waypointContainer = Instantiate(waypoint, player.position, player.rotation);
        }
        else
        {
            uIManager.feedbackBox.SetActive(true);
            uIManager.feedbackText.text = "Deactivate drone first";
        }
    }

    void DestroyWaypoint()
    {
        if(waypointActive)
        {
            Destroy(waypointContainer);
        }
    }
}
