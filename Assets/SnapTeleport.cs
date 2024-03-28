using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using Oculus.Interaction.Locomotion;

public class SnapTeleport : MonoBehaviour
{
    public SendPosition sendPositionScript;
    private List<TeleportInteractor> teleportInteractors = new List<TeleportInteractor>();
  //  public GameObject teleportMarkerPrefab; // Prefab for the teleportation marker
    private GameObject teleportMarkerInstance; // Instance of the teleportation marker
    public GameObject[] ProximityZones;
    private bool hasTeleportedIntoZone = false; // Flag to track if the player has teleported into a zone

    private void Start()
    {
        // Find all TeleportInteractor components in the scene
        TeleportInteractor[] allTeleportInteractors = FindObjectsOfType<TeleportInteractor>();

        // Add each TeleportInteractor to the list and subscribe to its event
        foreach (TeleportInteractor ti in allTeleportInteractors)
        {
            teleportInteractors.Add(ti);
            ti.WhenLocomotionPerformed += OnLocomotionPerformed;
        }

        // Check if any TeleportInteractor components were found
        if (teleportInteractors.Count == 0)
        {
            Debug.LogError("No TeleportInteractor components found in the scene.");
        }
    }

    private bool IsPlayerNearZone(GameObject zone)
    {
        // Check if the player is near the zone based on the hit point provided by TeleportInteractable.
        TeleportInteractable teleportInteractable = zone.GetComponent<TeleportInteractable>();
        if (teleportInteractable != null)
        {
            TeleportHit hit;
            if (teleportInteractable.DetectHit(PlayerController.transform.position, PlayerController.transform.position, out hit))
            {
                // Check proximity based on hit point
                Debug.Log(hit.Point);
                float distanceToZone = Vector3.Distance(hit.Point, PlayerController.transform.position);
                 Debug.Log(distanceToZone);
                return distanceToZone < proximity;
            }
        }
        return false;
    }


    public RemindPlayerAudio audio;
public Transform player;
public GameObject PlayerController;
  
      private void OnLocomotionPerformed(LocomotionEvent locomotionEvent)
    {
        foreach (GameObject zone in ProximityZones)
        {
            if (zone.activeSelf && IsPlayerNearZone(zone) && !hasTeleportedIntoZone)
            {
                // Teleport the player to a certain position
                audio.Play();
                snapDistance(zone);
                zone.SetActive(false);
                hasTeleportedIntoZone = true;
                break;
            }
        }
    }
public float proximity = 0.7f;
public void snapDistance(GameObject zone){
    Transform cylinder = zone.transform;
          // Calculate the direction vector from player to cylinder
        Vector3 directionToCylinder = cylinder.position - player.position;

        // Ignore the Y-axis (vertical) component
        directionToCylinder.y = 0;

        // Normalize the direction vector
        directionToCylinder.Normalize();

        // Calculate the displacement vector by multiplying the normalized direction vector by the distance
        Vector3 displacement = directionToCylinder * proximity;

        // Calculate the new position by adding the displacement vector to the player's current position
        Vector3 newPosition = player.position + displacement;

        // Assign the new position to the player
        PlayerController.transform.position = newPosition;
}
    private void OnDestroy()
    {
        // Unsubscribe from events when the script is destroyed
        foreach (TeleportInteractor ti in teleportInteractors)
        {
            ti.WhenLocomotionPerformed -= OnLocomotionPerformed;
        }
    }
}
