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
    public ActionData currentActionData;
    private float currentActionStartTime = 0.0f;
    public Transform currentTarget;
    
    
    private float nextRecordTime;

    public void SetActionStartTimestamp()
    {
        currentActionStartTime = Time.time;
        rightHandTracker.handBehaviourHandler.ShouldTrackHandToObjectTime = true;
        leftHandTracker.handBehaviourHandler.ShouldTrackHandToObjectTime = true;
        rightHandTracker.handBehaviourHandler.TimestampToObjectTime = 0.0f;
        leftHandTracker.handBehaviourHandler.TimestampToObjectTime = 0.0f;
        currentTarget = null;
    }

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
        CalculateHandToObjectTime();
        Debug.Log("Hand to object time " + currentActionData.handMovementToObjectTime + " seconds");
        currentActionData.reactionTime = UnityEngine.Random.Range(0.2f, 0.8f);
        currentActionData.handReachedDestinationTimestamp = currentActionData.handMovementToObjectTime + currentActionData.reactionTime;
        CalculateDistance();
        Debug.Log("Hand to object distance " + currentActionData.handDistanceToTarget + " meters");

        // Add to game data
        currentGameData.AddAction(currentActionData);

        // Initialize new action data
        currentActionData = new ActionData
        {
            rightHandFrames = new List<FrameHandData>(),
            leftHandFrames = new List<FrameHandData>()
        };
    }

    private void CalculateDistance()
    {
        if (currentTarget == null)
        {
            currentActionData.handDistanceToTarget = 0.0f;
            return;
        }

        float leftHandDistance = Vector3.Distance(leftHandTracker.handBehaviourHandler.transform.position, currentTarget.position);
        float rihtDistance = Vector3.Distance(rightHandTracker.handBehaviourHandler.transform.position, currentTarget.position);
        
        float distance = Mathf.Min(leftHandDistance, rihtDistance);

        if (currentActionData.handReachedDestination)
        {
            currentActionData.handDistanceToTarget = 0.0f;
            return;
        }
        
        
        currentActionData.handDistanceToTarget = distance;
    }

    private void CalculateHandToObjectTime()
    {
        float leftHandTime = leftHandTracker.handBehaviourHandler.TimestampToObjectTime - currentActionStartTime;
        float rightHandTime = rightHandTracker.handBehaviourHandler.TimestampToObjectTime - currentActionStartTime;

        if (leftHandTime <= 0.0f && rightHandTime <= 0.0f)
        {
            currentActionData.handMovementToObjectTime = -1f;
            return;
        }

        if (leftHandTime > 0.0f && rightHandTime <= 0.0f)
        {
            currentActionData.handMovementToObjectTime = leftHandTime;
            return;
        }

        if (rightHandTime > 0.0f && leftHandTime <= 0.0f)
        {
            currentActionData.handMovementToObjectTime = rightHandTime;
            return;
        }

        currentActionData.handMovementToObjectTime = Mathf.Min(leftHandTime, rightHandTime);
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
        
        currentGameData.endTimestamp = Time.time;
        currentGameData.overallGameTime = currentGameData.endTimestamp - currentGameData.startTimestamp;
    }

    private void RecordHand()
    {
        if (currentActionData == null)
        {
            Debug.LogError("No action is being tracked. Call StartTrackingActions first.");
            return;
        }

        // Get hand data for both hands
        FrameHandData rightHandData = rightHandTracker.GetHandFrameData();
        FrameHandData leftHandData = leftHandTracker.GetHandFrameData();

        // Record hand data only if both are valid
        if (rightHandData != null && leftHandData != null)
        {
            currentActionData.rightHandFrames.Add(rightHandData);
            currentActionData.leftHandFrames.Add(leftHandData);
        }
        else
        {
            Debug.LogWarning("Hand data is incomplete. Skipping recording for this frame. Normal in first frames.");
        }
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
