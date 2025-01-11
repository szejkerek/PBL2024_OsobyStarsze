using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks.Triggers;
using Holes;
using Plugins.KinectModule.Scripts.Events;
using LivesSystem;
using System;

public class AdjustDifficulty : MonoBehaviour
{
    private GameplayController gameplayController;
    private HolesManager holesManager;
    private LivesController livesController;
    private SaveKinectData saveKinectData;

//    int levelSelected;
//    float badElementChance;
    int lives;
    bool livesChecked = false;
 //   public float timeInStablePosition;


 //   string tag;
    int lastScore;

    bool VEasySelected;
    bool EasySelected;

    int scoreTreshold = 5000;
    int counter = 0;
    // +5000 co poziom - metoda increase, zaleznie od obecnie osiagnietego


   // int milestonesCount;

    private int goodElementsCollected;      //assigned
 //   private int goodObjectSpawned;          //assigned

    private int saveGElColl;
    private int saveGOSpawned;

    private List<string> collectedData;
    public List<string> changesToBeSaved;

    bool restbonus;
    public bool restartBonus
    {
        get { return restbonus; }
        set 
        { 
            restbonus = value;

            //if (restbonus)// && prevScore == score)
            //{
            //    gameplayController.Combo = 1;
            //    gameplayController.UpdateCombo();
            //}
            //  gameplayController.Combo = 1;
            // gameplayController.UpdateCombo();

        }
    }


    public float TimeInStablePosition()
    {
        return holesManager.timeInStablePosition;
    }
    public int Lives
    {
        get { return lives; }
        set
        {
            lives = value;

            if (lives == 1 && livesChecked == false)
            {
                DecreaseLevel();
                livesChecked = true;
            }
            changesToBeSaved.Add("Number of lives-"+ lives + ";" + DateTime.Now.ToString());
        }

    }
    int score;
    int prevScore;
    public int Score
    {
        get { return score;}
        set 
        {
            prevScore = score;
            score = value;

            //if(score == prevScore && restbonus)
            //{
            //    gameplayController.Combo = 1;
            //    gameplayController.UpdateCombo();
            //}

            if (score > scoreTreshold)//scoreTreshold[milestonesCount] && milestones[milestonesCount]==false)
            {
                IncreaseLevel();
                scoreTreshold+= scoreTreshold;
                string time = DateTime.Now.ToString();
                changesToBeSaved.Add("New Treshold=" + scoreTreshold + ";" + time);
                //milestones[milestonesCount] = true;
                //milestonesCount++;
            }
        }
    }


    void Start()
    {
        gameplayController = GameObject.FindGameObjectWithTag("Controller").GetComponent<GameplayController>();
        holesManager = GameObject.FindGameObjectWithTag("hManager").GetComponent<HolesManager>();
        livesController = GameObject.FindGameObjectWithTag("livesControl").GetComponent<LivesController>();

        StartCoroutine(setInitialParametersAfterDelay());
        InvokeRepeating("CheckParameterValues", 10f, 1.5f);
        InvokeRepeating("CheckScoreEveryHalfMinute", 15f, 40f); //czas-stary-60s-30s
        InvokeRepeating("DestroyCombo", 15f, 8f);
       // InvokeRepeating("CheckScoreValueEverySec", 15f, 0.5f);
    }

    IEnumerator setInitialParametersAfterDelay()
    {
        yield return new WaitForSeconds(5.5f);
        goodElementsCollected = 0;
      //  goodObjectSpawned = 0;
        saveGElColl=0;
        saveGOSpawned=0;
        holesManager.AdjustAllParameters();
        changesToBeSaved.Add("level selected=" + holesManager.levelSelected.ToString());
    }

    void CheckScoreValueEverySec()
    {
        prevScore = score;
    }

    void Update()
    {
        if (restbonus == true)
        {
            //gameplayController.Combo = 1;
            //gameplayController.UpdateCombo();
        }
    }

    void CheckParameterValues()
    {
        try
        {
            Lives = livesController.lives;
            Score = gameplayController.Score;
            goodElementsCollected = gameplayController.NGoodElCollected; // nr of good elements collected from last bad element
          //  goodObjectSpawned = holesManager.GoodObjectsSpawned;
           // collectedData = saveKinectData.CollectedData;

            //Debug.Log("Parameters checked!");
        }
        catch
        {
            Debug.Log("Couldn't check parameters to adjust difficulty level!");
        }
    }

 
    /*Increase level of difficulty*/
    void IncreaseLevel()
    {
        if (holesManager.levelSelected + 1 == holesManager.Difflevels.Length) //max difficulty level
        {
            holesManager.badElementChance = 60;
            //holesManager.timeInStablePosition = 3.5f;
        }
        else
        {
            holesManager.SetNewElementList(holesManager.levelSelected + 1);
            changesToBeSaved.Add("More items added;" + DateTime.Now.ToString());
            holesManager.levelSelected++;
            holesManager.AdjustAllParameters();
        }

        string n = "increased-" + holesManager.levelSelected.ToString() + ";"+DateTime.Now.ToString();
        string n1 = holesManager.badElementChance.ToString() + ";" + holesManager.timeInStablePosition.ToString();
        changesToBeSaved.Add(n);
        changesToBeSaved.Add(n1);
        Debug.Log("Level increased!");
    }

    void DecreaseLevel()
    {
        if (holesManager.levelSelected == 0)
        {
            holesManager.badElementChance = 10;
        }
        else
        {
            holesManager.SetNewElementList(holesManager.levelSelected-1);
            changesToBeSaved.Add("Items removed;" + DateTime.Now.ToString());
            holesManager.levelSelected--;
            holesManager.AdjustAllParameters();
        }
       
     
        Debug.Log("Level decreased!");
        string n = "decreased-" + holesManager.levelSelected.ToString() + ";"+DateTime.Now.ToString(); 
        string n1 = holesManager.badElementChance.ToString() + ";" + holesManager.timeInStablePosition.ToString();
        changesToBeSaved.Add(n);
        changesToBeSaved.Add(n1);
    }


    void CheckScoreEveryHalfMinute()
    {
      // int diff1 = goodElementsCollected - saveGElColl;
      // int diff2 = goodObjectSpawned - saveGOSpawned;

        //if(diff2 - diff1>=5) //10
        //{

        //}
         
        if( (holesManager.GoodObjectsSpawned - (goodElementsCollected + (counter*10))) >= 10)
        {
            DecreaseLevel();
            counter++;
            gameplayController.LooseLife();
            changesToBeSaved.Add("10 items not collected;" + DateTime.Now.ToString());
        }
         //  saveGElColl = goodElementsCollected;
        //saveGOSpawned = goodObjectSpawned;

    }

    //void setPrivateParamForThisClassAndEasyLvlsOnly()
    //{
    //    if(levelSelected ==8)
    //    {
    //        VEasySelected = true;
    //        EasySelected = true;
    //    }
    //    else if(levelSelected==6)
    //    {
    //        VEasySelected= false;
    //        EasySelected=true;
    //    }
    //    else
    //    {
    //        VEasySelected= false;
    //        EasySelected=false;
    //    }
    //}
  
    int countupdates = 0;
    int param = 1;

    void DestroyCombo()
    {
        int diff1 = goodElementsCollected - saveGElColl;
        int diff2 = holesManager.GoodObjectsSpawned - saveGOSpawned;// goodElementsCollected;
        double diff3 = (diff2 - diff1);//10;
        if(diff3>=1)//0.1)//diff2 - diff1>=4)
        {
            gameplayController.Combo = 1;
            gameplayController.UpdateCombo();
            countupdates++;
        }
        saveGElColl = goodElementsCollected;// + countupdates*4;
        saveGOSpawned = holesManager.GoodObjectsSpawned;

        string time = DateTime.Now.ToString();
        changesToBeSaved.Add("combo restarted to 1;"+time);
    }

    void TurnOffHole(int activeHoles)
    {

    }
}
