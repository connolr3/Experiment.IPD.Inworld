using UnityEngine;
using Oculus.Interaction.Input;
using Oculus.Interaction.Locomotion;
using System.Collections;
using System.Collections.Generic;


public class snapPlayer : MonoBehaviour
{
   public SendPosition sendPositionScript;
    private List<TeleportInteractor> teleportInteractors = new List<TeleportInteractor>();
 public GameObject toMatch;
        public GameObject rig;
    public Transform targetMatch;

     public Transform cameraOffset;
    public GameObject MainCamera;

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
public Transform player;
public Transform SpawnA;
  private WaitForSeconds delay = new WaitForSeconds(0.01f); // Adjust the delay time as needed

    private void OnLocomotionPerformed(LocomotionEvent locomotionEvent)
    {
        // Call a coroutine to delay the execution of Debug.Log
        StartCoroutine(DebugPositionWithDelay());
    }

    private IEnumerator DebugPositionWithDelay()
    {
        yield return delay; // Wait for the specified delay

        // Debug the position after the delay
        Debug.Log(player.position);
        Debug.Log(Vector3.Distance(player.position, SpawnA.position));
        Vector3 moveTo = CalculatetargetPoint();
        MoveXROriginToFinalPosXY(moveTo);
    }

    public Transform target;
private Vector3 CalculatetargetPoint(){
      // Get the direction vector from player to target
        Vector3 direction = (target.position - player.position).normalized;

        // Calculate the position 0.7m away from the target along the direction
        Vector3 newPosition = target.position - direction * 0.7f;
        return newPosition;
        // Make the player face the same direction
       // player.rotation = Quaternion.LookRotation(direction);

        // Set the player's position to the calculated position
     //   player.position = newPosition;
}
private Vector3 targetPoint;

     private void MoveXROriginToFinalPosXY(Vector3 finalPosition)
{
    if (rig != null && MainCamera != null)
    {
        // Calculate the offset needed to move MainCamera to the final position (only affecting X and Z)
        Vector3 offset = new Vector3(finalPosition.x - MainCamera.transform.position.x, 0f, finalPosition.z - MainCamera.transform.position.z);

        // Move XROrigin with the calculated offset
        rig.transform.position += offset;

        // Calculate the rotation needed to go from the current Y rotation to the target Y rotation
        float targetYRotation = finalPosition.y; // Assuming finalPosition.y represents the Y rotation
        float currentYRotation = MainCamera.transform.eulerAngles.y;
        float deltaRotation = targetYRotation - currentYRotation;
        Quaternion rotationNeeded = Quaternion.Euler(0f, deltaRotation, 0f);

        // Apply the calculated rotation to toMatch
       // toMatch.transform.rotation *= rotationNeeded;
    }
    else
    {
        Debug.LogWarning("Set 'XROrigin' and 'MainCamera' references in the inspector.");
    }
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
