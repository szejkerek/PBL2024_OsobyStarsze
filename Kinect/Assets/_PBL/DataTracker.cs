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
    private string filePath;
    [SerializeField] private AppType appType;

    private void Awake()
    {
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


    void Start()
    {
        GameAnalysisData gameAnalysisData = SampleDataGenerator.GenerateGameData();
        SaveData(gameAnalysisData);
    }

    public void SaveData(GameAnalysisData data)
    {
        string jsonData = JsonUtilityEx.ToJson(data, true);
        File.WriteAllText(filePath, jsonData);
        Debug.Log("Data saved to " + filePath);
    }

    public GameAnalysisData LoadData()
    {
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            return JsonUtilityEx.FromJson<GameAnalysisData>(jsonData);
        }
        else
        {
            Debug.LogError("No save file found!");
            return new GameAnalysisData();
        }
    }
}