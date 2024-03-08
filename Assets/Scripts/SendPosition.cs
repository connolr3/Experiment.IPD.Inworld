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

    private int number_Teleports=0;
    void Start()
    {
        timer = sendInterval;
    }

    public void BeginRecord()
    {
        shouldRecord = true;
    }

    public void EndRecord()
    {
        shouldRecord = false;
    }
    public void pushEndTrialInfo(float InteractionTime) { 
    
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
    private bool firstTime = true;
    void SendRegularData()
    {
        List<string> positions = new List<string>();
           if (firstTime)
         
        {
            DateTime currentDate = DateTime.Now;
         //   Debug.Log("Current Date: " + currentDate.ToString("yyyy-MM-dd HH:mm:ss"));
            // Add spawn positions
            positions.Add($"{{\"name\":\"BLOCK: \", \"position\":\"{title},{currentDate.ToString("yyyy-MM-dd HH:mm:ss")},0\", \"timestamp\":\"0\"}}");
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
                positions.Add($"{{\"name\":\"{go.name}\", \"position\":\"{position.x},{position.y},{position.z}\", \"timestamp\":\"{timestamp}\"}}");
            }

        }
     

        // Combine regular positions with teleport events
        positions.AddRange(teleportEvents);
        teleportEvents = new List<string>();
        string jsonData = $"[ {string.Join(", ", positions)} ]";
if(jsonData!=""||jsonData!=null)
        StartCoroutine(SendData(jsonData));
    }


    public void AddTeleportEvent()
    {
        number_Teleports++;
        string timestamp = GetTimestamp();
        string teleportData = $"{{\"name\":\"Teleport\", \"position\":\"{0},{0},{0}\", \"timestamp\":\"{timestamp}\"}}";
        teleportEvents.Add(teleportData);
    }

    public void AddAIOpenEvent()
    {
        string timestamp = GetTimestamp();
        string teleportData = $"{{\"name\":\"AI Started{InworldController.CurrentCharacter}\", \"position\":\"{0},{0},{0}\", \"timestamp\":\"{timestamp}\"}}";
        teleportEvents.Add(teleportData);
    }

    public void AddAICloseEvent()
    {
        string timestamp = GetTimestamp();
        string teleportData = $"{{\"name\":\"AI Stopped{InworldController.CurrentCharacter}\", \"position\":\"{0},{0},{0}\", \"timestamp\":\"{timestamp}\"}}";
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
                Debug.Log("Error: " + webRequest.error);
            }
            else
            {
                Debug.Log("Response: " + webRequest.downloadHandler.text);
            }
        }
    }
}
