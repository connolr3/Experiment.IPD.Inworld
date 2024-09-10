using UnityEngine;
using Oculus.Interaction.Input;
using Oculus.Interaction.Locomotion;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Rendering.LookDev;


public class snapPlayer : MonoBehaviour
{
    public SendPosition sendPositionScript;
    private List<TeleportInteractor> teleportInteractors = new List<TeleportInteractor>();
    public GameObject toMatch;
    public GameObject rig;
    //  public Transform targetMatch;
    public bool playSnapAudio;
  
    private Transform target;
    public AudioSource playMe;
    public Transform cameraOffset;
    private bool[] alreadySnapped;
    public GameObject MainCamera;

    public bool snapRotate;

    private void Start()
    {
        index = 0;
        int arraySize = 10; // Example size

        // Instantiate the array and set all elements to false
        alreadySnapped = new bool[arraySize];
        for (int i = 0; i < alreadySnapped.Length; i++)
        {
            alreadySnapped[i] = false;
        }
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
    private WaitForSeconds delay = new WaitForSeconds(0.01f); // Adjust the delay time as needed

    private void OnLocomotionPerformed(LocomotionEvent locomotionEvent)
    {
        sendPositionScript.AddTeleportEvent(MainCamera.transform.position);
        // Call a coroutine to delay the execution of Debug.Log
        StartCoroutine(DebugPositionWithDelay());
    }

    private IEnumerator DebugPositionWithDelay()
    {
        yield return delay; // Wait for the specified delay

        // Debug the position after the delay
        //Debug.Log(player.position);
        float distanceFrom = Vector3.Distance(player.position, target.position);
        Debug.Log(distanceFrom);
        if (distanceFrom < snapRadius && !alreadySnapped[index]) {
            sendPositionScript.AddSnapTeleportEvent(MainCamera.transform.position);
            Vector3 moveTo = CalculatetargetPoint();
            MoveXROriginToFinalPosXY(moveTo);
        }

    }
    private int index = 0;
    public void setTarget(Transform newTarget) {
       
       
      target=  newTarget;
    }
    private Vector3 CalculatetargetPoint()
    {
        if (target != null) {
            // Get the direction vector from player to target
            Vector3 direction = (target.position - player.position).normalized;

            // Calculate the position 0.7m away from the target along the direction
            Vector3 newPosition = target.position - direction * 1.2f;
            return newPosition;
        }
        return new Vector3(-1,-1,-1);
       
    }
   // private void Update()
   // {
      //  Debug.Log(index);
   // }
    public void updateIndex(int newindex) {
        index = newindex;

    }
    private Vector3 targetPoint;
    public float snapRadius = 2f;
   private void MoveXROriginToFinalPosXY(Vector3 finalPosition)
{
    if (rig != null && MainCamera != null && !alreadySnapped[index])
    {
        // Calculate the offset needed to move MainCamera to the final position (only affecting X and Z)
        Vector3 offset = new Vector3(finalPosition.x - MainCamera.transform.position.x, 0f, finalPosition.z - MainCamera.transform.position.z);
        // Calculate the rotation needed to go from the current Y rotation to the target Y rotation
        float targetYRotation = finalPosition.y; // Assuming finalPosition.y represents the Y rotation
        float currentYRotation = MainCamera.transform.eulerAngles.y;
        float deltaRotation = targetYRotation - currentYRotation;
        Quaternion rotationNeeded = Quaternion.Euler(0f, deltaRotation, 0f);

       

        // Calculate the vector from player to target
        Vector3 playerToTarget = target.position - MainCamera.transform.position;

        // Check if the player is in front of the target (dot product should be positive)
        if (Vector3.Dot(playerToTarget.normalized, MainCamera.transform.forward) > 0)
        {
               // Move XROrigin with the calculated offset
        rig.transform.position += offset;
            // Apply the calculated rotation to toMatch
            if (snapRotate)
            {
                toMatch.transform.rotation *= rotationNeeded;
            }
             if (playSnapAudio)
            playMe.Play();
            alreadySnapped[index] = true;
        }
        else
        {
            Debug.LogWarning("Player is not in front of the target.");
        }
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
