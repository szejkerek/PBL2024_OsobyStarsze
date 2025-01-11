using System;
using UnityEngine;
using System.Collections.Generic;

class SampleDataGenerator
{

    public static GameAnalysisData GenerateGameData()
    {
        // Create sample game analysis data
        GameAnalysisData gameData = new GameAnalysisData
        {
            overallGameTime = 120.5f,
            overallScore = 1500.0f,
            actions = new List<ActionData>()
        };
        
        ActionData action1 = new ActionData
        {
            rightHandPositions = new List<Vector3> { new Vector3(1, 2, 3), new Vector3(2, 3, 4), new Vector3(3, 4, 5), new Vector3(4, 5, 6), new Vector3(5, 6, 7) },
            rightHandSpeeds = new List<Vector3> { new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.6f, 0.6f, 0.6f), new Vector3(0.7f, 0.7f, 0.7f), new Vector3(0.8f, 0.8f, 0.8f), new Vector3(0.9f, 0.9f, 0.9f) },
            leftHandPositions = new List<Vector3> { new Vector3(1, 1, 1), new Vector3(1, 2, 1), new Vector3(1, 3, 1), new Vector3(1, 4, 1), new Vector3(1, 5, 1) },
            leftHandSpeeds = new List<Vector3> { new Vector3(0.3f, 0.3f, 0.3f), new Vector3(0.4f, 0.4f, 0.4f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.6f, 0.6f, 0.6f), new Vector3(0.7f, 0.7f, 0.7f) },
            handMovementToObjectTime = 2.5f,
            reactionTime = 0.8f,
            handReachedDestination = true,
            handReachedDestinationTimestamp = 3f,
            rightHandReachedDestination = true,
            leftHandReachedDestination = false,
            aimAccuracy = 95.0f
        };

        ActionData action2 = new ActionData
        {
            rightHandPositions = new List<Vector3> { new Vector3(3, 4, 5), new Vector3(4, 5, 6), new Vector3(5, 6, 7), new Vector3(6, 7, 8), new Vector3(7, 8, 9) },
            rightHandSpeeds = new List<Vector3> { new Vector3(0.6f, 0.6f, 0.6f), new Vector3(0.7f, 0.7f, 0.7f), new Vector3(0.8f, 0.8f, 0.8f), new Vector3(0.9f, 0.9f, 0.9f), new Vector3(1.0f, 1.0f, 1.0f) },
            leftHandPositions = new List<Vector3> { new Vector3(2, 2, 2), new Vector3(3, 3, 3), new Vector3(4, 4, 4), new Vector3(5, 5, 5), new Vector3(6, 6, 6) },
            leftHandSpeeds = new List<Vector3> { new Vector3(0.4f, 0.4f, 0.4f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.6f, 0.6f, 0.6f), new Vector3(0.7f, 0.7f, 0.7f), new Vector3(0.8f, 0.8f, 0.8f) },
            handMovementToObjectTime = 3.0f,
            reactionTime = 1.0f,
            handReachedDestination = false,
            handReachedDestinationTimestamp = 1f,
            rightHandReachedDestination = false,
            leftHandReachedDestination = true,
            aimAccuracy = 85.0f
        };

        ActionData action3 = new ActionData
        {
            rightHandPositions = new List<Vector3> { new Vector3(2, 3, 4), new Vector3(3, 4, 5), new Vector3(4, 5, 6), new Vector3(5, 6, 7), new Vector3(6, 7, 8) },
            rightHandSpeeds = new List<Vector3> { new Vector3(0.4f, 0.4f, 0.4f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.6f, 0.6f, 0.6f), new Vector3(0.7f, 0.7f, 0.7f), new Vector3(0.8f, 0.8f, 0.8f) },
            leftHandPositions = new List<Vector3> { new Vector3(3, 3, 3), new Vector3(4, 4, 4), new Vector3(5, 5, 5), new Vector3(6, 6, 6), new Vector3(7, 7, 7) },
            leftHandSpeeds = new List<Vector3> { new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.6f, 0.6f, 0.6f), new Vector3(0.7f, 0.7f, 0.7f), new Vector3(0.8f, 0.8f, 0.8f), new Vector3(0.9f, 0.9f, 0.9f) },
            handMovementToObjectTime = 3.2f,
            reactionTime = 1.1f,
            handReachedDestination = true,
            handReachedDestinationTimestamp = 2f,
            rightHandReachedDestination = true,
            leftHandReachedDestination = false,
            aimAccuracy = 90.0f
        };

        ActionData action4 = new ActionData
        {
            rightHandPositions = new List<Vector3> { new Vector3(4, 5, 6), new Vector3(5, 6, 7), new Vector3(6, 7, 8), new Vector3(7, 8, 9), new Vector3(8, 9, 10) },
            rightHandSpeeds = new List<Vector3> { new Vector3(0.3f, 0.3f, 0.3f), new Vector3(0.4f, 0.4f, 0.4f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.6f, 0.6f, 0.6f), new Vector3(0.7f, 0.7f, 0.7f) },
            leftHandPositions = new List<Vector3> { new Vector3(1, 1, 1), new Vector3(2, 2, 2), new Vector3(3, 3, 3), new Vector3(4, 4, 4), new Vector3(5, 5, 5) },
            leftHandSpeeds = new List<Vector3> { new Vector3(0.2f, 0.2f, 0.2f), new Vector3(0.3f, 0.3f, 0.3f), new Vector3(0.4f, 0.4f, 0.4f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.6f, 0.6f, 0.6f) },
            handMovementToObjectTime = 2.8f,
            reactionTime = 0.9f,
            handReachedDestinationTimestamp = 2f,
            handReachedDestination = false,
            rightHandReachedDestination = false,
            leftHandReachedDestination = true,
            aimAccuracy = 88.0f
        };
        

        // Add actions to game data
        gameData.actions.Add(action1);
        gameData.actions.Add(action2);
        gameData.actions.Add(action3);
        gameData.actions.Add(action4);

        Debug.Log("Sample data generated successfully!");
        return gameData;
    }
}



