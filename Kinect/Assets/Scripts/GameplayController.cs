using Holes;
using LivesSystem;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using UnityEngine.UIElements;
using System.Collections;

public class GameplayController : MonoBehaviour
{

    private int score;
    public int Score
    {
        get { return score; }
    }

    private int numberOfElCollected;

    public int NumberOfElCollected
    { get { return numberOfElCollected; } }

    private int nGoodElCollected;

    public int NGoodElCollected
    { get { return nGoodElCollected; } }

    int lastCollected;
    public int LastCollected
    { get { return lastCollected; } }

    public int Combo
    { get { return combo; }
        set { combo = value; } }

    //private int scoreCounterToChangeLvl;
    private int points;
    private int combo = 1;
    private float spawningTimer = 1;
    private int badHits;
    private float spawningDelay;
    private bool canStartGame;
    private float timeLeft = 600f; ///30//60//180
    private int seconds;
    private bool levelHard = false;
    private bool checkedDif = false;

    private List<TMP_Text> scores = new List<TMP_Text>();

    [SerializeField]
    private TMP_Text scoreText;
    [SerializeField]
    private TMP_Text comboText;
    [SerializeField]
    private LivesController livesController;
    [SerializeField]
    private HolesManager holesManager;
    [SerializeField]
    private GameOverPanel gameoverPanel;
    [SerializeField]
    private RectTransform uiGameplay;

    [Space()]

    [SerializeField]
    private TMP_Text finalScoreText;

    [SerializeField]
    private RectTransform rowScoreContent;

    [SerializeField]
    private ScoreRow rowScorePrefab;

    [Space()]

    [SerializeField]
    private Camera camera;

    [SerializeField]
    private TMP_Text textPrefab;

    [Space()]
    [SerializeField]
    private GameObject scoreImg;
    [SerializeField]
    private TMP_Text textCounter;
    [Space]
    [SerializeField]
    private GameObject StartHint;
    [SerializeField]
    public Text StartHintTxt;
    bool hintWasShown;

    public AudioSource audioSource;
    public AudioClip clipBad;
    public AudioClip clipEnd;

    public List<string> collectingData;
    public int lastScore;


Dictionary<string, string> startInfDic = new Dictionary<string, string>()
    {
        { "Food", "Zbieraj jedzenie!" },
        { "Dishes", "Zbieraj naczynia!" },
        { "Sport", "Zbieraj przedmioty sportowe!" },
        { "Electr", "Zbieraj sporzet elektroniczny!" }
    };
 

    public void SetSpawningTime(int time)
    {
        spawningDelay = time;
        spawningTimer = spawningDelay;
    }

    public void Start()
    {
        
        audioSource = GetComponent<AudioSource>();  
        textCounter = scoreImg.GetComponentInChildren<TMP_Text>();
        //SelectStartInfo();
        hintWasShown = false;
        numberOfElCollected = 0;
        nGoodElCollected = 0;
        //StartCoroutine(showStartInfo());
        scoreImg.gameObject.SetActive(false);
        Debug.Log(Application.persistentDataPath);
    }

    private void Awake() 
    {
        badHits = 0;
        canStartGame = false;
    }

    private void Update()
    {
        if (canStartGame) {         //during gameplay
            HolesController();

            if (!hintWasShown)
            {
                SelectStartInfo();
                hintWasShown = true;
                StartCoroutine(showStartInfo());
            }
        }

        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;

            if (timeLeft < 60)
            {
                if (!scoreImg.gameObject.activeSelf)
                {
                    scoreImg.gameObject.SetActive(true);
                }

                seconds = (int)(timeLeft % 60);
                textCounter.text = "Czas: " + seconds.ToString();
            }
        }
        else
        {
            audioSource.PlayOneShot(clipEnd);
            Debug.Log("Koniec gry!");

            if (livesController.lives != 0)
            {
                LooseLife();
            }
        }

        }

    private void HolesController()
    {
        spawningTimer -= Time.deltaTime;

        if (spawningTimer < 0)
        {
            holesManager.SpawnEnemy();
            spawningTimer = spawningDelay;
        }
    }

    public void StartGame() {
        uiGameplay.gameObject.SetActive(true);
        canStartGame = true;
        Time.timeScale = 1f;
        UpdateCombo();
    }

    public void CheckSpawnedObject(GameObject spawnedObject, int id){
        int x = 0;
        if (spawnedObject.CompareTag(holesManager.selectedTag))
        {
            UpdateScore(spawnedObject);
            x = 1;
            nGoodElCollected++;
        }
  //      for (int i = 0; i < 3; i++)
  //      if (spawnedObject.CompareTag(holesManager.otherTags[i]))
        else
        {
            LooseLife();
             x = 0;
             nGoodElCollected = 0;
        }
        numberOfElCollected++;
        //
        var src = DateTime.Now;
        var format = (src.Hour + ":" + src.Minute + ":" + src.Second).ToString();
        string line = spawnedObject.name +"," + spawnedObject.tag + "," + id.ToString() +","+ score.ToString() + "," + x.ToString() + "," + format;
        collectingData.Add(line);

    }
    public void LooseLife() //priv
    {
        badHits++;
        livesController.ToggleHearts(badHits);
        combo = 1;
        UpdateCombo();
        if (badHits == 3)
        {
            audioSource.PlayOneShot(clipEnd);
            WaitAndStopTheGame().Forget();
        }
        audioSource.volume = 1f;
        audioSource.PlayOneShot(clipBad);
        audioSource.volume = 0.5f;
    }

    private void StopGame()
    {
        finalScoreText.text = "Liczba uzbieranych punktów: " + score;
        gameoverPanel.Enable();
        HandleScoreHistory().Forget();
        
    }

    private async UniTaskVoid HandleScoreHistory() {
        List <PlayersScoreController.PlayerScore> playersScores = await PlayersScoreController.Instance.HandleScoreHistory(score);
        foreach (PlayersScoreController.PlayerScore playerScore in playersScores) {
            ScoreRow scoreRow = Instantiate(rowScorePrefab, rowScoreContent);
            scoreRow.Init(playerScore.playerName, playerScore.score.ToString());
        }
    }

    void SelectStartInfo()
    {
        string textinfo;
        startInfDic.TryGetValue(holesManager.selectedTag, out textinfo );
        StartHintTxt.text = textinfo;
    }


    //poka¿ info co bêdzie zbierane w rozgrywce 5s
    IEnumerator showStartInfo()
    {
        yield return new WaitForSeconds(12);
        StartHint.gameObject.SetActive(false);
    }

    private void UpdateScore(GameObject spawnedObject)
    {
        points = combo * 100;
        score += points;
        SpawnScore(points, spawnedObject);
        audioSource.Play();
        if(combo <=4)
        {
            combo++;
            UpdateCombo();
        }
        //lastScore = points;
    }

    public void UpdateCombo()
    {
        comboText.text = "Bonus x" + (combo);
    }

    private async UniTaskVoid WaitAndStopTheGame() {
        Time.timeScale = 0.5f;
        await UniTask.Delay(1 * 1000, true);
        StopGame();
    }

    private void SpawnScore(int addScore, GameObject spawnedObject) {
        TMP_Text useText = null;
        foreach (TMP_Text text in scores) {
            if (text && !text.gameObject.activeSelf) {
                useText = text;
                text.gameObject.SetActive(true);
                break;
            }
        }


        if(useText == null) {
            useText = Instantiate(textPrefab, uiGameplay);
            scores.Add(useText.GetComponent<TMP_Text>());
        }

        Vector3 screenPosition = camera.WorldToScreenPoint(spawnedObject.transform.position);
        useText.transform.position = screenPosition;
        useText.text = addScore.ToString();
        useText.transform.DOPunchScale(Vector3.one, 0.5f, 3, 0.75f);

        useText.transform.DOMove(scoreText.transform.position, 1f).SetEase(Ease.InCubic).OnComplete(()=> {
            scoreText.text = "Punkty: " + (score);
            scoreText.transform.DOKill();
            scoreText.transform.DOPunchScale(Vector3.one, 0.5f, 3, 0.75f);
            useText.gameObject.SetActive(false);
        });
    }
    
}
