using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Inworld;

using System;
public class SendPosition : MonoBehaviour
{
    public GameObject[] gameObjects;
    private readonly string postUrl = "http://localhost:5000/receive_data";
    public float sendInterval = 1.0f; // send interval 1s
    public string title;
    private List<string> regularPositions = new List<string>();
    private List<string> teleportEvents = new List<string>();

    public Transform[] spawns;
    private float timer;
    private bool shouldRecord = false;
    private Transform AIHips;
    private static bool firstTime = true;
    private int number_Teleports = 0;
    void Start()
    {
        timer = sendInterval;
        firstTime = true;
    }

    public void BeginRecord()
    {
        shouldRecord = true;
    }

    public void EndRecord()
    {
        shouldRecord = false;
    }
    public void pushEndTrialInfo(float InteractionTime)
    {

    }

    void Update()
    {
        timer -= Time.deltaTime;

        // Regular update of game object positions every second
        if (timer <= 0)
        {
            SendRegularData();
            timer = sendInterval;
        }
    }

    /*void SendRegularData()
    {
        List<string> positions = new List<string>();
        if (firstTime)
        {
            DateTime currentDate = DateTime.Now;
            //   Debug.Log("Current Date: " + currentDate.ToString("yyyy-MM-dd HH:mm:ss"));
            // Add spawn positions
            positions.Add($"{{\"name\":\"BLOCK: {title}{currentDate.ToString("yyyy-MM-dd HH:mm:ss")} \", \"position\":\",,0\", \"timestamp\":\"0\"}}");
            positions.Add($"{{\"name\":\"SpawnA\", \"position\":\"{spawns[0].position.x},{spawns[0].position.y},{spawns[0].position.z}\", \"timestamp\":\"{GetTimestamp()}\"}}");
            positions.Add($"{{\"name\":\"SpawnB\", \"position\":\"{spawns[1].position.x},{spawns[1].position.y},{spawns[1].position.z}\", \"timestamp\":\"{GetTimestamp()}\"}}");
            firstTime = false;
        }
        if (shouldRecord)
        {
            // Add regular game object positions
            foreach (var go in gameObjects)
            {
                Vector3 position = go.transform.position;
                string timestamp = GetTimestamp();
                if (go.name == "Hips")
                {
                    positions.Add($"{{\"name\":\"UserHips\", \"position\":\"{position.x},{position.y},{position.z}\", \"timestamp\":\"{timestamp}\"}}");

                }
                else if (go.name == "CenterEyeAnchor")
                {
                    positions.Add($"{{\"name\":\"UserHMD\", \"position\":\"{position.x},{position.y},{position.z}\", \"timestamp\":\"{timestamp}\"}}");
                }

                else
                {
                    positions.Add($"{{\"name\":\"{go.name}\", \"position\":\"{position.x},{position.y},{position.z}\", \"timestamp\":\"{timestamp}\"}}");
                }

            }
            string mytimestamp = GetTimestamp();
            if (AIHips != null)
            {
                positions.Add($"{{\"name\":\"AIHips\", \"position\":\"{AIHips.position.x},{AIHips.position.y},{AIHips.position.z}\", \"timestamp\":\"{mytimestamp}\"}}");
            }
            if (AIHead != null)
            {
                positions.Add($"{{\"name\":\"AIHead\", \"position\":\"{AIHead.position.x},{AIHead.position.y},{AIHead.position.z}\", \"timestamp\":\"{mytimestamp}\"}}");

            }

        }
*/
    public Transform UserCam;
    public Transform UserHips;
    public void SendRegularData()
    {
        List<string> positions = new List<string>();
        if (firstTime)
        {
            DateTime currentDate = DateTime.Now;
            //   Debug.Log("Current Date: " + currentDate.ToString("yyyy-MM-dd HH:mm:ss"));
            // Add spawn positions
            positions.Add($"{{\"name\":\"BLOCK: {title}{currentDate.ToString("yyyy-MM-dd HH:mm:ss")} \", \"position\":\" UserCamX, UserCamY, UserCamZ, UserHipsX, UserHipsY, UserHipsZ, AIHeadX, AIHeadY, AIHeadZ, AIHipsX, AIHipsY,AIHipsZ\", \"timestamp\":\"TimeStamp\"}}");
            //  positions.Add($"{{\"name\":\"SpawnA\", \"position\":\"{spawns[0].position.x},{spawns[0].position.y},{spawns[0].position.z}\", \"timestamp\":\"{GetTimestamp()}\"}}");
            //   positions.Add($"{{\"name\":\"SpawnB\", \"position\":\"{spawns[1].position.x},{spawns[1].position.y},{spawns[1].position.z}\", \"timestamp\":\"{GetTimestamp()}\"}}");
            Debug.Log(positions);
            firstTime = false;
        }
        if (shouldRecord)
        {
            string mytimestamp = GetTimestamp();
            // Check if AIHead is not null, use its position; otherwise, use -1,-1,-1
            string aiHeadPosition = AIHead != null
                ? $"{AIHead.position.x},{AIHead.position.y},{AIHead.position.z}"
                : "-1,-1,-1";

            // Check if AIHips is not null, use its position; otherwise, use -1,-1,-1
            string aiHipsPosition = AIHips != null
                ? $"{AIHips.position.x},{AIHips.position.y},{AIHips.position.z}"
                : "-1,-1,-1";

            positions.Add($"{{\"name\":\"Position Data\", \"position\":\"{UserCam.position.x},{UserCam.position.y},{UserCam.position.z},{UserHips.position.x},{UserHips.position.y},{UserHips.position.z},{aiHeadPosition},{aiHipsPosition}\", \"timestamp\":\"{mytimestamp}\"}}");
            //  positions.Add($"{{\"name\":\"Position Data\", \"position\":\"{UserCam.position.x},{UserCam.position.y},{UserCam.position.z},{UserHips.position.x},{UserHips.position.y},{UserHips.position.z},{0},{0},{0},{0},{0},{0}\", \"timestamp\":\"{mytimestamp}\"}}");
        }

        // Combine regular positions with teleport events
        positions.AddRange(teleportEvents);
        teleportEvents = new List<string>();
        string jsonData = $"[ {string.Join(", ", positions)} ]";
        if (jsonData != "" || jsonData != null)
            StartCoroutine(SendData(jsonData));
    }

    private Transform AIHead;


    public void setNewAIObjects(Transform thisAIHips, Transform thisAIHead)
    {
        AIHips = thisAIHips;
        AIHead = thisAIHead;

    }
    public void AddTeleportEvent()
    {
        number_Teleports++;
        string timestamp = GetTimestamp();
        string teleportData = $"{{\"name\":\"Teleported\", \"position\":\"{-1},{-1},{-1},{-1},{-1},{-1},{-1},{-1},{-1},{-1},{-1},{-1}\", \"timestamp\":\"{timestamp}\"}}";
        teleportEvents.Add(teleportData);
    }

    public void AddAIOpenEvent()
    {
        string timestamp = GetTimestamp();
        string characterName = InworldController.CurrentCharacter.name.Split('_')[0];
        string teleportData = $"{{\"name\":\"AI Started{characterName}\", \"position\":\"{-1},{-1},{-1},{-1},{-1},{-1},{-1},{-1},{-1},{-1},{-1},{-1}\", \"timestamp\":\"{timestamp}\"}}";
        teleportEvents.Add(teleportData);
    }



    public void AddAICloseEvent()
    {
        string timestamp = GetTimestamp();
        string characterName = InworldController.CurrentCharacter.name.Split('_')[0];
        string teleportData = $"{{\"name\":\"AI Stopped{characterName}\", \"position\":\"{-1},{-1},{-1},{-1},{-1},{-1},{-1},{-1},{-1},{-1},{-1},{-1}\", \"timestamp\":\"{timestamp}\"}}";
        teleportEvents.Add(teleportData);
    }

    string GetTimestamp()
    {
        return DateTime.UtcNow.ToString("HH:mm:ss");
    }
    IEnumerator SendData(string jsonData)
    {
        using (UnityWebRequest webRequest = new UnityWebRequest(postUrl, "POST"))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
             //   Debug.Log("Error: " + webRequest.error);
            }
            else
            {
           //     Debug.Log("Response: " + webRequest.downloadHandler.text);
            }
        }
    }
}
