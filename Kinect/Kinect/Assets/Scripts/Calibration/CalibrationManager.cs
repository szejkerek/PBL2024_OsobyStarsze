using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Plugins.KinectModule.Data;
using Plugins.KinectModule.Steering;
using Plugins.KinectModule.Scripts.Events;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using System.Linq;

public class CalibrationManager : MonoBehaviour
{

    [SerializeField] SettingKinect settingKinect;
    [SerializeField] GameplayController gameplayController;

    List<Joint2DInput> jointInputs = new List<Joint2DInput>();
    List<MouseInput> mouseInputs = new List<MouseInput>();
    List<CheckCollision> checkCollisions = new List<CheckCollision>();

    [Space()]

    [SerializeField]
    TextMeshProUGUI infoText;

    [SerializeField]
    RawImage visualization;

    private void OnValidate() {
        if (settingKinect == null) {
            Debug.LogError("Give me SettingKinect!");
        }
        if (gameplayController == null) {
            Debug.LogError("Give me GameplayController!");
        }
    }

    private void OnEnable()
    {
        settingKinect.onDeviceStarting.AddListener(OnStarting);
        settingKinect.onDeviceRunning.AddListener(OnStart);
        settingKinect.onDeviceOff.AddListener(OnError);
        Init();
    }

    private void OnDisable() 
    {
        settingKinect.onDeviceStarting.RemoveListener(OnStarting);
        settingKinect.onDeviceRunning.RemoveListener(OnStart);
        settingKinect.onDeviceOff.RemoveListener(OnError);
    }

    public void StartCalibration()
    {
        Init();
        settingKinect.StartDevice();
        infoText.text = "Czekam na urz�dzenie...";
        LoadingImage.instance.Show();
    }

    public void UseMouse() {
        Init();
        mouseInputs.ForEach(x => x.enabled = true);
        jointInputs.ForEach(x => x.enabled = false);
        gameplayController.StartGame();
        settingKinect.enabled = false;
        gameObject.SetActive(false);
    }

    private void Init() {
        if (jointInputs.Count == 0 || mouseInputs.Count == 0 || checkCollisions.Count == 0) {
            jointInputs = new List<Joint2DInput>(FindObjectsOfType<Joint2DInput>());
            mouseInputs = new List<MouseInput>(FindObjectsOfType<MouseInput>());
            checkCollisions = new List<CheckCollision>(FindObjectsOfType<CheckCollision>());
        }
        
        checkCollisions.ForEach(x => x.SetTriggerTime(SettingsController.Instance.GetTimeToActiveHand()));
    }

    private void OnStarting() 
    {
        infoText.text = "Urz�dzenie zosta�o wykryte. Pr�bujemy si� z nim po��czy�...";
        LoadingImage.instance.Show();
    }

    private void OnStart()
    {
        infoText.text = "Poprawnie pod��czono si� do urz�dzenia!";
        visualization.gameObject.SetActive(SettingsController.Instance.GetUseVisualization());
        StartGame().Forget();
        LoadingImage.instance.Hide();
    }

    private void OnError() {
        infoText.text = "Co� posz�o nie tak. Staramy si� ponownie po��czy�...";
        LoadingImage.instance.Hide();
    }

    private async UniTaskVoid StartGame() {
        await UniTask.Delay(2 * 1000, true);

        infoText.text = "Sprawdzanie stanu u�ytkownika...";

        await UniTask.Delay(1 * 1000, true);

        while (settingKinect.playerDistanceStatus != PlayerDistanceStatus.Perfect) {
            switch (settingKinect.playerDistanceStatus) {
                case PlayerDistanceStatus.TooClose:
                    infoText.text = "Jeste� za blisko urz�dzenia!";
                    break;
                case PlayerDistanceStatus.TooFar:
                    infoText.text = "Jeste� za daleko urz�dzenia!";
                    break;
                default:
                    infoText.text = "Nie wykryto �adnej postaci.";
                    break;
            }

            await UniTask.Yield();
        }

        infoText.text = "Zosta�e� wykryty! Zaraz rozpocznie si� gra!";

        await UniTask.Delay(2 * 1000, true);
        infoText.text = "3";
        await UniTask.Delay(1 * 1000, true);
        infoText.text = "2";
        await UniTask.Delay(1 * 1000, true);
        infoText.text = "1";
        await UniTask.Delay(1 * 1000, true);

        jointInputs.ForEach(x => x.enabled = true);
        mouseInputs.ForEach(x => x.enabled = false);
        gameplayController.StartGame();
        gameObject.SetActive(false);
    }

}
