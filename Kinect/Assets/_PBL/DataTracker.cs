using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public struct GameAnalysisData
{
    public float overallGameTime;
    public float overallScore;
    public List<ActionData> actions;
}

[System.Serializable]
public struct ActionData
{
    public List<Vector3> rightHandPositions;
    public List<Vector3> rightHandSpeeds;
    public List<Vector3> leftHandPositions;
    public List<Vector3> leftHandSpeeds;
    public float handMovementToObjectTime;
    public float reactionTime;
    public float handReachedDestinationTimestamp;
    public bool handReachedDestination;
    public bool rightHandReachedDestination;
    public bool leftHandReachedDestination;
    public float aimAccuracy;
}

[System.Serializable]
public enum AppType
{
    Kinect,
    VR
}
public class DataTracker : MonoBehaviour
{
    public static DataTracker Instance { get; private set; }
    private string filePath;
    [SerializeField] private AppType appType;

    private GameAnalysisData currentGameData;
    private ActionData currentActionData;

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

        string folderPath = Path.Combine(Application.dataPath, "..", "..", "DataAnalysis");
        string timestamp = DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss");

        switch (appType)
        {
            case AppType.VR:
                filePath = Path.Combine(folderPath, "DataVR", $"{timestamp}.json");
                break;
            case AppType.Kinect:
                filePath = Path.Combine(folderPath, "DataKinect", $"{timestamp}.json");
                break;
        }
    }

    public void StartTrackingActions()
    {
        currentActionData = new ActionData
        {
            rightHandPositions = new List<Vector3>(),
            rightHandSpeeds = new List<Vector3>(),
            leftHandPositions = new List<Vector3>(),
            leftHandSpeeds = new List<Vector3>()
        };

        Debug.Log("Started tracking actions.");
    }

    public void StopTrackingActions()
    {
        currentGameData.actions.Add(currentActionData);
        SaveData(currentGameData);
        Debug.Log("Stopped tracking actions and saved data.");
    }

    public void RecordAction(Vector3 rightHandPosition, Vector3 rightHandSpeed, Vector3 leftHandPosition, Vector3 leftHandSpeed)
    {
        currentActionData.rightHandPositions.Add(rightHandPosition);
        currentActionData.rightHandSpeeds.Add(rightHandSpeed);
        currentActionData.leftHandPositions.Add(leftHandPosition);
        currentActionData.leftHandSpeeds.Add(leftHandSpeed);
    }

    void Start()
    {
        ResetGameData();
        SaveData(currentGameData);
    }

    public void ResetGameData()
    {
        currentGameData = new GameAnalysisData
        {
            actions = new List<ActionData>()
        };
    }

    public void SaveData(GameAnalysisData data)
    {
        string jsonData = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, jsonData);
        Debug.Log("Data saved to " + filePath);
    }

    public GameAnalysisData LoadData()
    {
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            return JsonUtility.FromJson<GameAnalysisData>(jsonData);
        }
        else
        {
            Debug.LogError("No save file found!");
            return new GameAnalysisData();
        }
    }
}
