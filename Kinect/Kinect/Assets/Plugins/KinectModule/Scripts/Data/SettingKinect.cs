using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Plugins.KinectModule.Data{

    /// <summary>
    /// Klasa służąca do zarządzania połączeniem z urządzeniem.
    /// </summary>
    public class SettingKinect : MonoBehaviour {

        /// <summary>
        /// Zdarzenie, wywoływane przy poprawnym połaczeniu się z urządzeniem.
        /// </summary>
        public UnityEvent onDeviceRunning;
        /// <summary>
        /// Zdarzenie, wywoływane przy próbie połączenia się z urządzeniem.
        /// </summary>
        public UnityEvent onDeviceStarting;
        /// <summary>
        /// Zdarzenie, wywoływane przy odłączenia się urządzenia lub przy niepoprawnym połączeniu się.
        /// </summary>
        public UnityEvent onDeviceOff;

        /// <summary>
        /// Referencja do obiektu klasy KinectConnection.
        /// </summary>
        private KinectConnection connection;
        /// <summary>
        /// Referencja do obiektu klasy BodyTracker.
        /// </summary>
        private BodyTracker bodyTracker;

        [Space()]

        /// <summary>
        /// Minimalna odległość od kamery danej postaci.
        /// </summary>
        [SerializeField]
        private float minDistanceFromCamera = 1.2f;
        /// <summary>
        /// Maksymalna odległość od kamery danej postaci.
        /// </summary>
        [SerializeField]
        private float maxDistanceFromCamera = 3f;
        /// <summary>
        /// Czas w sekundach, po którym jest próba ponownego połączenia się z urządzeniem. 
        /// </summary>
        [SerializeField]
        private float timeToReconnectDeviceInSeconds = 4f;

        /// <summary>
        /// Status dystansu wykrytego ciała.
        /// </summary>
        public PlayerDistanceStatus playerDistanceStatus { private set; get; } = PlayerDistanceStatus.Undetected;

        /// <summary>
        /// Poprzedni status urządzenia.
        /// </summary>
        private KinectStatus prevStatus = KinectStatus.Off;
        /// <summary>
        /// Czy jakieś ciało zostało wykryte przez urządzenie.
        /// </summary>
        private bool isDetected = false;

        private void Start() {
            connection = GetComponent<KinectConnection>();
            bodyTracker = GetComponent<BodyTracker>();
            onDeviceOff.AddListener(TryToReconnectToDevice);
        }

        /// <summary>
        /// Co klatke wywołuje metody CatchChangeKinectDeviceStatus i CatchDistanceFromCamera.
        /// </summary>
        private void Update() {
            CatchChangeKinectDeviceStatus();
            CatchDistanceFromCamera();
        }

        public void StartDevice() {
            connection.StartThread();
        }

        /// <summary>
        /// Sprawdza czy zmienił się status urządzenia i wywołuje odpowiednie zdarzenie.
        /// </summary>
        private void CatchChangeKinectDeviceStatus() {
            if (connection.status != prevStatus) {
                prevStatus = connection.status;
                switch (prevStatus) {
                    case KinectStatus.Off:
                    case KinectStatus.OffWithError:
                        onDeviceOff?.Invoke();
                        break;
                    case KinectStatus.Starting:
                        onDeviceStarting?.Invoke();
                        break;
                    case KinectStatus.Running:
                        onDeviceRunning?.Invoke();
                        break;
                }
            }
        }

        /// <summary>
        /// Sprawdza czy zmieniła się odległość od urządzenia. 
        /// </summary>
        private void CatchDistanceFromCamera() {
            if(connection.status == KinectStatus.Running){
                if (bodyTracker.CanGetUpdateData()) {
                    Vector3 position;
                    Quaternion rotation;
                    (position, rotation) = bodyTracker.GetDataByJoint(Microsoft.Azure.Kinect.BodyTracking.JointId.Pelvis, ref isDetected);
                    if (!isDetected) {
                        playerDistanceStatus = PlayerDistanceStatus.Undetected;
                    } else if (position.z < minDistanceFromCamera) {
                        playerDistanceStatus = PlayerDistanceStatus.TooClose;
                    } else if (position.z > maxDistanceFromCamera) {
                        playerDistanceStatus = PlayerDistanceStatus.TooFar;
                    } else {
                        playerDistanceStatus = PlayerDistanceStatus.Perfect;
                    }
                }
            } else {
                playerDistanceStatus = PlayerDistanceStatus.Undetected;
            }
        }

        /// <summary>
        /// Wywołuje metode TryToReconnectToDeviceLoop, jeśli urządzenie nie jest połączone.
        /// </summary>
        private void TryToReconnectToDevice() {
            if (prevStatus == KinectStatus.OffWithError || timeToReconnectDeviceInSeconds <= 0) {
                TryToReconnectToDeviceLoop().Forget();
            }
        }

        /// <summary>
        /// Próbuje połączyć się z urządzeniem.
        /// </summary>
        /// <returns>UniTaskVoid</returns>
        private async UniTaskVoid TryToReconnectToDeviceLoop() {
            await UniTask.Delay((int)(timeToReconnectDeviceInSeconds * 1000));
            connection.StartThread();
        }
    }

    /// <summary>
    /// Typ wyliczeniowy informujący o statusie odległosci postaci ludzkiej od urządzenia.
    /// </summary>
    public enum PlayerDistanceStatus {
        TooClose,
        TooFar,
        Perfect,
        Undetected
    }
}