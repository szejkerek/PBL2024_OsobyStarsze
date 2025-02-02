using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[System.Serializable]
public class FrameHandData
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 rotationEuler;
    public Vector3 direction;
    public float speed;
}

[System.Serializable]
public class GameAnalysisData
{
    public float overallGameTime;
    public float overallScore;
    public string appType;
    public float startTimestamp;
    public float endTimestamp;
    
    public List<ActionData> actions;
    public GameAnalysisData(string appType)
    {
        this.appType = appType;
        actions = new List<ActionData>();
    }

    public void AddAction(ActionData action)
    {
        actions.Add(action);
    }
}

[System.Serializable]
public class ActionData
{
    public float handMovementToObjectTime;
    public float reactionTime;
    public float handReachedDestinationTimestamp;
    public bool goodTargetFound;
    public bool handReachedDestination;
    public bool rightHandReachedDestination;
    public bool leftHandReachedDestination;
    [FormerlySerializedAs("aimAccuracy")] public float handDistanceToTarget;
    public List<FrameHandData> rightHandFrames;
    public List<FrameHandData> leftHandFrames;

    // Constructor to initialize the lists
    public ActionData(
        float handMovementToObjectTime = 0f, 
        float reactionTime = 0f, 
        float handReachedDestinationTimestamp = 0f, 
        bool handReachedDestination = false, 
        bool goodTargetFound = false, 
        bool rightHandReachedDestination = false, 
        bool leftHandReachedDestination = false, 
        float handDistanceToTarget = 0f)
    {
        rightHandFrames = new List<FrameHandData>();
        leftHandFrames = new List<FrameHandData>();
        this.handMovementToObjectTime = handMovementToObjectTime;
        this.reactionTime = reactionTime;
        this.handReachedDestinationTimestamp = handReachedDestinationTimestamp;
        this.handReachedDestination = handReachedDestination;
        this.rightHandReachedDestination = rightHandReachedDestination;
        this.leftHandReachedDestination = leftHandReachedDestination;
        this.handDistanceToTarget = handDistanceToTarget;
    }
}