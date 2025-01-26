using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public enum AppType
{
    Kinect,
    VR
}

public class DataTracker : MonoBehaviour
{
    public static DataTracker Instance { get; private set; }
    [SerializeField] private AppType appType;

    [Header("Hands")]
    [SerializeField] private float handDataRecordInterval = 0.2f;
    [SerializeField] private HandsDataTracker rightHandTracker;
    [SerializeField] private HandsDataTracker leftHandTracker;

    private string filePath;
    private GameAnalysisData currentGameData;
    private ActionData currentActionData;

    private float nextRecordTime;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeFilePath();
        ResetGameData();
    }

    private void OnDestroy()
    {
        SaveData();
    }

    public void EndAction()
    {
        if (currentActionData == null)
        {
            Debug.LogWarning("No action to end. Call StartTrackingActions first.");
            return;
        }

        // Populate action data
        currentActionData.handMovementToObjectTime = 2.5f;
        currentActionData.reactionTime = 1.25f;
        currentActionData.handReachedDestinationTimestamp =
            currentActionData.handMovementToObjectTime + currentActionData.reactionTime;
        currentActionData.handReachedDestination = true;
        currentActionData.rightHandReachedDestination = true;
        currentActionData.leftHandReachedDestination = false;
        currentActionData.aimAccuracy = 0.70f;

        // Add to game data
        currentGameData.AddAction(currentActionData);

        // Initialize new action data
        currentActionData = new ActionData
        {
            rightHandFrames = new List<FrameHandData>(),
            leftHandFrames = new List<FrameHandData>()
        };
    }

    private void InitializeFilePath()
    {
        string folderPath = Path.Combine(Application.dataPath, "..", "..", "DataAnalysis");
        string timestamp = DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string subFolder = appType == AppType.VR ? "DataVR" : "DataKinect";
        string fullFolderPath = Path.Combine(folderPath, subFolder);

        if (!Directory.Exists(fullFolderPath))
        {
            Directory.CreateDirectory(fullFolderPath);
        }

        filePath = Path.Combine(fullFolderPath, $"{timestamp}.json");
    }

    private void Update()
    {
        if (Time.time >= nextRecordTime)
        {
            if (currentActionData == null)
            {
                Debug.LogError("No action is being tracked. Call StartTrackingActions first.");
                return;
            }

            RecordHand();
            nextRecordTime = Time.time + handDataRecordInterval;
        }
    }

    private void RecordHand()
    {
        if (currentActionData == null)
        {
            Debug.LogError("No action is being tracked. Call StartTrackingActions first.");
            return;
        }

        // Record hand data
        currentActionData.rightHandFrames.Add(rightHandTracker.GetHandFrameData());
        currentActionData.leftHandFrames.Add(leftHandTracker.GetHandFrameData());
    }

    public void ResetGameData(bool saveCurrentData = false)
    {
        if (saveCurrentData)
        {
            SaveData();
            Debug.Log("Current game data saved before resetting.");
        }

        currentGameData = new GameAnalysisData(appType.ToString())
        {
            startTimestamp = Time.time
        };

        currentActionData = new ActionData
        {
            rightHandFrames = new List<FrameHandData>(),
            leftHandFrames = new List<FrameHandData>()
        };
    }

    public void SaveData()
    {
        if (currentGameData == null)
        {
            Debug.LogWarning("No game data to save.");
            return;
        }

        currentGameData.endTimestamp = Time.time;
        currentGameData.overallGameTime = currentGameData.endTimestamp - currentGameData.startTimestamp;

        try
        {
            string jsonData = JsonUtility.ToJson(currentGameData, true);
            File.WriteAllText(filePath, jsonData);
            Debug.Log("Data saved to " + filePath);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save data: {ex.Message}");
        }
    }
}
