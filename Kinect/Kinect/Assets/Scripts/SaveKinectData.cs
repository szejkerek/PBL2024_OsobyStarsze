using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using Plugins.KinectModule.Data;
using Plugins.KinectModule.Gesture;
using UnityEngine.Events;
using System.IO;
using System;
using Plugins.KinectModule.SaveData;


/// <summary>
/// Klasa do zapisu danych do plików csv.
/// </summary>
public class SaveKinectData : MonoBehaviour
{

    public GameObject handLeft;
    public GameObject handRight;

    private GameplayController gameplayController;
    private AdjustDifficulty adjustDifficulty;

    public Vector3 leftHandPositionsToSave;
    public Vector3 rightHandPositionToSave;

    private List<string> handsDataToSave = new List<string>();

    private List<string> data1;
    private List<string> data2;
    private List<string> difficultyData;
    private List<string> collectedData;
    public List<string> CollectedData
    {
        get { return collectedData; }
    }
    void Start()
    {
        GameObject controller = GameObject.FindGameObjectWithTag("Controller");
        gameplayController = controller.GetComponent<GameplayController>();
        GameObject adjust = GameObject.FindGameObjectWithTag("AdjDif");
        adjustDifficulty = adjust.GetComponent<AdjustDifficulty>();
        InvokeRepeating("checkHandsPosition", 10.0f, 0.2f);
    }

    private void OnDestroy()
    {
        data1 = CollectData.Instance.newdataFirstJoint;
        data2 = CollectData.Instance.newdataSecondJoint;
        difficultyData = adjustDifficulty.changesToBeSaved;
        collectedData = gameplayController.collectingData;
        if (handsDataToSave != null)
        {
            SaveHandsData();
        }
        SaveRest();
        SaveRest2();

        if(collectedData != null)
        {
            SaveCollectedData();
        }
        if(difficultyData != null)
        SaveDifficultyData();
    }

    void checkHandsPosition()
    {
        leftHandPositionsToSave = handLeft.transform.position;
        rightHandPositionToSave = handRight.transform.position;

        var newLine = string.Format("{0};{1};{2};{3};{4};{5}", leftHandPositionsToSave.x, leftHandPositionsToSave.y, leftHandPositionsToSave.z,
                                                                rightHandPositionToSave.x, rightHandPositionToSave.y, rightHandPositionToSave.z);

        handsDataToSave.Add(newLine);

    }


    /// <summary>
    /// Zapisuje listê z pozycjami d³oni zebranymi w czasie rozgrywki do pliku csv.
    /// </summary>
    void SaveHandsData()
    {
        string directory = Application.dataPath + "/UserData/";
        string filename = String.Format(DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss_")+"hands.csv");
        string path = Path.Combine(directory, filename);

        using (StreamWriter sw = File.CreateText(path))
        {
            for (int i = 0; i < handsDataToSave.Count; i++)
            {
                sw.WriteLine(handsDataToSave[i]);
            }
        }
    }

    /// <summary>
    /// Zapisuje listê z pozycjami biodra prawergo zebranymi w czasie rozgrywki do pliku csv.
    /// </summary>
    void SaveRest()
    {
        string directory = Application.dataPath + "/UserData/";
        string filename = String.Format(DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss_")+"hipright.csv");
        string path = Path.Combine(directory, filename);

        using (StreamWriter sw = File.CreateText(path))
        {
            for (int i = 0; i < data1.Count; i++)
            {
                sw.WriteLine(data1[i]);
            }
        }
    }

    /// <summary>
    /// Zapisuje listê z pozycjami stopy prawej zebranymi w czasie rozgrywki do pliku csv.
    /// </summary>
    void SaveRest2()
    {
        string directory = Application.dataPath + "/UserData/";
        string filename = String.Format(DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss_")+"footright.csv");
        string path = Path.Combine(directory, filename);

        using (StreamWriter sw = File.CreateText(path))
        {
            for (int i = 0; i < data2.Count; i++)
            {
                sw.WriteLine(data2[i]);
            }
        }
    }


    /// <summary>
    /// Zapisuje listê z zebranymi obiektami, ich nazwami, tag'ami oraz ID miejsca, w którym obiekt zosta³ zebrany w czasie rozgrywki do pliku csv.
    /// </summary>
    void SaveCollectedData()
    {
        string directory = Application.dataPath + "/UserData/";
        string filename = String.Format(DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss_") + "objCollected.csv");
        string path = Path.Combine(directory, filename);

        using (StreamWriter sw = File.CreateText(path))
        {
            for (int i = 0; i < collectedData.Count; i++)
            {
                sw.WriteLine(collectedData[i]);
            }
        }
    }


    void SaveDifficultyData()
    {
        string directory = Application.dataPath + "/UserData/";
        string filename = String.Format(DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss_") + "difficultychanges.csv");
        string path = Path.Combine(directory, filename);

        using (StreamWriter sw = File.CreateText(path))
        {
            for (int i = 0; i < difficultyData.Count; i++)
            {
                sw.WriteLine(difficultyData[i]);
            }
        }
    }
}
