using System;
using Cysharp.Threading.Tasks.Triggers;
using Holes;
using UnityEngine;

public class DifficultyChange : MonoBehaviour
{
    [SerializeField]
    private GameplayController controller;
    [SerializeField]
    private CalibrationManager calibrationManager;
    [SerializeField]
    private HolesManager holes;

    public int spawningTime;
    private void Awake()
    {
        Time.timeScale = 0;
    }

    private void Disable()
    {
        gameObject.SetActive(false);
        controller.SetSpawningTime(spawningTime);
        calibrationManager.gameObject.SetActive(true);
        calibrationManager.StartCalibration();
    }

    public void SetLevel(int level)
    {
        
        spawningTime = holes.Difflevels[level].SpawningTime;
        Debug.Log("Level:" + level);
        holes.SetList(level);
        Disable();
    }

    //public void VeryEasyLevel()
    //{
    //    spawningTime = 8;
    //    Debug.Log("Level:" + spawningTime); 
    //    holes.SetList(spawningTime);
    //    Disable();
    //}

    //public void EasyLevel()
    //{
    //    spawningTime = 6; //6
    //    Debug.Log("Level:" + spawningTime);
    //    holes.SetList(spawningTime);
    //    Disable();
    //}

    //public void MediumLevel()
    //{
    //    spawningTime = 5; //5
    //    Debug.Log("Level:" + spawningTime);
    //    holes.SetList(spawningTime);
    //    Disable();
    //}
    
    //public void HardLevel()
    //{
    //    spawningTime = 4;
    //    Debug.Log("Level:" + spawningTime);
    //    holes.SetList(spawningTime);
    //    Disable();
    //}

    //public void VeryHardLevel()
    //{
    //    spawningTime = 3;
    //    Debug.Log("Level:" + spawningTime);
    //    holes.SetList(spawningTime);
    //    Disable();
    //}
}
