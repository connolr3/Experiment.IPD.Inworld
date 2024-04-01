using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using Oculus.Interaction.Locomotion;

public class OnTeleportEventScript : MonoBehaviour
{
    public SendPosition sendPositionScript;
    private List<TeleportInteractor> teleportInteractors = new List<TeleportInteractor>();


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

    private void OnLocomotionPerformed(LocomotionEvent locomotionEvent)
    {
        // Handle locomotion event here
        sendPositionScript.AddTeleportEvent();
      //  Vector3 teleportPosition = locomotionEvent.Interactor.GetComponent<TeleportInteractor>().ArcEnd.Point;
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
